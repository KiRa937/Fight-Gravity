using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public int damage = 20;

    public GameObject shootingPlayer;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //const float degree = 3;

        transform.position = new Vector3(Mathf.Repeat(transform.position.x, 20), transform.position.y, transform.position.z);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(this.transform.right, GetComponent<Rigidbody2D>().velocity.normalized), degree);
        //transform.rotation = Quaternion.LookRotation(transform.right);
    }

    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.tag == "Player" && other.gameObject != shootingPlayer)
		{
			//other.collider.GetComponent<PlayerController>().GetKilled();
            other.collider.GetComponent<PlrController>().TakeDamage(damage, shootingPlayer.GetComponent<PlrController>());
		}

		Destroy(this.gameObject);
    }

}
