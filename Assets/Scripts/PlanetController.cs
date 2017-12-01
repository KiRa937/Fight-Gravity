using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{

    public float gravityForce = 1;

    public bool foundation = false;
    public float maxDegrees = 2;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
            Vector2 force;
            if (foundation)
                force = new Vector2(0, transform.position.y - other.transform.position.y);
            else
                force = transform.position - other.transform.position;

            rigidbody.AddForce(force.normalized * 1 / force.magnitude * gravityForce);


            if (!foundation)
                other.transform.rotation = Quaternion.RotateTowards(other.transform.rotation, Quaternion.FromToRotation(this.transform.up, -force.normalized), maxDegrees);
            else
            {
                Quaternion poles = Quaternion.Euler(0, 0, 90 + Mathf.Sign(force.y) * 90);
                other.transform.rotation = Quaternion.RotateTowards(other.transform.rotation, poles, maxDegrees);
            }
            //other.transform.rotation = Quaternion.Lerp(other.transform.rotation, Quaternion.FromToRotation(Vector2.down, force.normalized), 20*Time.deltaTime);
            //other.transform.rotation = Quaternion.RotateTowards(other.transform.rotation, Quaternion.FromToRotation(other.transform.up, -force.normalized), maxDegrees);
        }

        if (other.tag == "Projectile")
        {
            Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
            Vector2 force;
            if (foundation)
                force = new Vector2(0, transform.position.y - other.transform.position.y);
            else
                force = transform.position - other.transform.position;

            rigidbody.AddForce(force.normalized * 1 / force.magnitude * gravityForce / 5);
        }
    }
}
