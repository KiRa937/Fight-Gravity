using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlrController : NetworkBehaviour
{
    public int maxHealth = 100;

    public Text healthText;
    public Text KDText;

    [SyncVar(hook = "OnChangeHealth")]
    private int curHealth;
    void OnChangeHealth(int curHealth)
    {
        healthText.text = "Health: " + curHealth;
    }

    [SyncVar(hook = "OnChangeKills")]
    private int kills;
    void OnChangeKills(int kills)
    {
        string[] ss = KDText.text.Split('/');
        KDText.text = kills + " /" + ss[1];
    }

    [SyncVar(hook = "OnChangeDeaths")]
    private int deaths;
    void OnChangeDeaths(int deaths)
    {
        string[] ss = KDText.text.Split('/');
        KDText.text = ss[0] + "/ " + deaths;
    }

    public float moveSpeed;
    private float activeMoveSpeed;

    public GameObject bullet;
    public AudioClip shotClip;

    public GameObject explosion;

    private bool canMove;

    private Rigidbody2D myRigidbody;
    public float jumpSpeed;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    private bool isGrounded;
    private Vector3 mouseDir;

    public GameObject plrCam;
    private GameObject thisCam;

    private NetworkStartPosition[] spawnPoints;

    public WeaponSpecs curWeapon;
    private float cdTimer;

    private Animator anim;

    //private GameController controller;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            activeMoveSpeed = moveSpeed;

            curHealth = maxHealth;

            canMove = true;

            spawnPoints = FindObjectsOfType<NetworkStartPosition>();

            curWeapon = new WeaponSpecs(Weapons.shotgun, bullet, shotClip);
            //curWeapon = new WeaponSpecs(Weapons.bazooka, projectile);

            cdTimer = curWeapon.cooldown;

            anim = GetComponent<Animator>();
        }

        curHealth = maxHealth;

        curWeapon = new WeaponSpecs(Weapons.shotgun, bullet, shotClip);
        //curWeapon = new WeaponSpecs(Weapons.bazooka, projectile);

        cdTimer = curWeapon.cooldown;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            plrCam.gameObject.SetActive(false);

            //if (this.GetComponent<NetworkView>().isMine == false)
            //Destroy(GetComponentInChildren<Camera>());

            return;
        }
        else
        {
            thisCam = plrCam;
        }

        transform.position = new Vector3(Mathf.Repeat(transform.position.x, 20), transform.position.y, transform.position.z);
        //transform.position = new Vector3(Mathf.Repeat(transform.position.x, 20), Mathf.Repeat(transform.position.y, 18), transform.position.z);

        mouseDir = GetComponentInChildren<Camera>().transform.rotation * (Input.mousePosition - GetComponentInChildren<Camera>().WorldToScreenPoint(transform.position));
        mouseDir.z = 0;
        mouseDir = mouseDir.normalized;
        cdTimer += Time.deltaTime;

        TurnCharacter((Input.mousePosition - GetComponentInChildren<Camera>().WorldToScreenPoint(transform.position)));

        if (isGrounded && canMove)
        {
            Vector3 locVel;
            if (Input.GetAxisRaw("Horizontal") > 0f)
            {
                locVel = transform.InverseTransformDirection(myRigidbody.velocity);
                locVel.x = activeMoveSpeed;
                myRigidbody.velocity = transform.TransformDirection(locVel);

                //transform.eulerAngles = new Vector2(transform.eulerAngles.x,0);
                //transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);


            }
            else if (Input.GetAxisRaw("Horizontal") < 0f)
            {
                locVel = transform.InverseTransformDirection(myRigidbody.velocity);
                locVel.x = -activeMoveSpeed;
                myRigidbody.velocity = transform.TransformDirection(locVel);

                //transform.eulerAngles = new Vector2(transform.eulerAngles.x,180);                
                //transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            }
            else
            {
                locVel = transform.InverseTransformDirection(myRigidbody.velocity);
                locVel.x = 0;
                myRigidbody.velocity = transform.TransformDirection(locVel);
            }

            if (Input.GetButtonDown("Jump"))
            {
                /*locVel = transform.InverseTransformDirection(myRigidbody.velocity);
                locVel.y = jumpSpeed;
                myRigidbody.velocity = transform.TransformDirection(locVel);*/
                myRigidbody.AddRelativeForce(Vector2.up * jumpSpeed);
            }

        }

        anim.SetFloat("Speed", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
        anim.SetBool("Grounded", isGrounded);

        if (Input.GetButton("Fire1") && cdTimer > curWeapon.cooldown)
        {
            GetComponent<AudioSource>().clip = curWeapon.shotAudio;
            CmdFireWeapon(/*transform.position,*/ mouseDir, this.gameObject);
            myRigidbody.AddForce(-mouseDir * curWeapon.recoil);
            cdTimer = 0;
        }

    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.Raycast(transform.position, -transform.up, groundCheckRadius, whatIsGround);
    }

    [Command]
    void CmdFireWeapon(/*Vector3 playerPos,*/ Vector3 mouseFireDir, GameObject plr)
    {
        const float wpnRadius = 0.4f;

        Vector3 playerPos = transform.position;

        //plr.GetComponent<AudioSource>().Play();        

        RpcPlayWeapon(plr);

        GameObject bul = Instantiate(curWeapon.ammo, playerPos + mouseFireDir * wpnRadius, Quaternion.FromToRotation(Vector2.right, mouseFireDir));
        bul.GetComponent<Rigidbody2D>().velocity = mouseFireDir * curWeapon.bulletSpeed;
        bul.GetComponent<ProjectileController>().shootingPlayer = this.gameObject;

        NetworkServer.Spawn(bul);
        Destroy(bul, curWeapon.bulletTime);

        if (curWeapon.type == Weapons.shotgun)
        {
            Quaternion shootRot = Quaternion.Euler(0, 0, 20);

            bul = Instantiate(curWeapon.ammo, playerPos + shootRot * mouseFireDir * wpnRadius, Quaternion.FromToRotation(Vector2.right, shootRot * mouseFireDir));
            bul.GetComponent<Rigidbody2D>().velocity = shootRot * mouseFireDir * curWeapon.bulletSpeed;
            bul.GetComponent<ProjectileController>().shootingPlayer = this.gameObject;

            NetworkServer.Spawn(bul);
            Destroy(bul, curWeapon.bulletTime);

            shootRot = Quaternion.Euler(0, 0, -20);

            bul = Instantiate(curWeapon.ammo, playerPos + shootRot * mouseFireDir * wpnRadius, Quaternion.FromToRotation(Vector2.right, shootRot * mouseFireDir));
            bul.GetComponent<Rigidbody2D>().velocity = shootRot * mouseFireDir * curWeapon.bulletSpeed;
            bul.GetComponent<ProjectileController>().shootingPlayer = this.gameObject;

            NetworkServer.Spawn(bul);
            Destroy(bul, curWeapon.bulletTime);
        }
    }

    [ClientRpc]
    void RpcPlayWeapon(GameObject plr)
    {
        if(!plr.GetComponent<AudioSource>().isPlaying)
        plr.GetComponent<AudioSource>().Play();
    }

    void TurnCharacter(Vector3 mouse)
    {
        if (mouse.x > 0)
            GetComponent<SpriteRenderer>().flipX = false;

        if (mouse.x < 0)
            GetComponent<SpriteRenderer>().flipX = true;

        CmdTurnChar(GetComponent<SpriteRenderer>().flipX, this.gameObject);
        
    }

     [Command]
     void CmdTurnChar(bool flip, GameObject plr)
     {
         plr.GetComponent<SpriteRenderer>().flipX = flip;

         RpcTurnChar(flip, plr);
     }
     
     [ClientRpc]
     void RpcTurnChar(bool flip, GameObject plr)
     {
         plr.GetComponent<SpriteRenderer>().flipX = flip;
     }

    /*[Command]
    void CmdFireShotgun(Vector3 playerPos, Vector3 mouseFireDir, GameObject plr)
    {
        const float bulletSpeed = 10;
        const float timeToLive = 1.5f;


        Quaternion shootRot = Quaternion.Euler(0, 0, 20);
        GameObject bul = Instantiate(bullet, playerPos + mouseFireDir * 0.3f, Quaternion.FromToRotation(Vector2.up, mouseFireDir));
        //bul.GetComponent<Rigidbody2D>().AddForce(mouseFireDir * 4);
        bul.GetComponent<Rigidbody2D>().velocity = mouseFireDir * bulletSpeed;
        bul.GetComponent<ProjectileController>().shootingPlayer = this.gameObject;

        NetworkServer.Spawn(bul);
        Destroy(bul, timeToLive);

        bul = Instantiate(bullet, playerPos + shootRot * mouseFireDir * 0.3f, Quaternion.FromToRotation(Vector2.up, shootRot * mouseFireDir));
        //bul.GetComponent<Rigidbody2D>().AddForce(shootRot * mouseFireDir * 4);
        bul.GetComponent<Rigidbody2D>().velocity = shootRot * mouseFireDir * bulletSpeed;
        bul.GetComponent<ProjectileController>().shootingPlayer = this.gameObject;

        NetworkServer.Spawn(bul);
        Destroy(bul, timeToLive);

        shootRot = Quaternion.Euler(0, 0, -20);

        bul = Instantiate(bullet, playerPos + shootRot * mouseFireDir * 0.3f, Quaternion.FromToRotation(Vector2.up, shootRot * mouseFireDir));
        //bul.GetComponent<Rigidbody2D>().AddForce(shootRot * mouseFireDir * 4);
        bul.GetComponent<Rigidbody2D>().velocity = shootRot * mouseFireDir * bulletSpeed;
        bul.GetComponent<ProjectileController>().shootingPlayer = this.gameObject;

        NetworkServer.Spawn(bul);
        Destroy(bul, timeToLive);
    }

    [Command]
    void CmdFireRocket(Vector3 playerPos, Vector3 mouseFireDir)
    {

        GameObject bul = Instantiate(projectile, playerPos + mouseFireDir * 0.3f, Quaternion.FromToRotation(Vector2.up, mouseFireDir));
        //bul.GetComponent<Rigidbody2D>().AddForce(mouseDir * 0.5f);
        bul.GetComponent<Rigidbody2D>().velocity = mouseFireDir * 7;
        bul.GetComponent<ProjectileController>().shootingPlayer = this.gameObject;

        NetworkServer.Spawn(bul);
        Destroy(bul, 5);
    }*/

    public void TakeDamage(int dmg, PlrController shotBy)
    {
        if (!isServer)
        {
            return;
        }

        curHealth -= dmg;
        //Debug.Log(CurHealth);

        if (curHealth <= 0)
        {
            curHealth = maxHealth;
            deaths++;
            shotBy.kills++;

            //GetKilled();
            RpcRespawn();
            //Debug.Log("DEAD!");
        }
    }

    public void TakeHealth(int health)
    {
        if (!isServer)
        {
            return;
        }

        curHealth += health;

        if (curHealth > maxHealth)
            curHealth = maxHealth;
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            //transform.position = new Vector3(10f, 2.5f, 0f);

            GameObject expl = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(expl, .3f);

            Vector3 spawnPoint = new Vector3(10f, 2.5f, 0f);
            Quaternion spawnRotation = Quaternion.Euler(0, 0, 0);

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int r = Random.Range(0, spawnPoints.Length);
                spawnPoint = spawnPoints[r].transform.position;
                spawnRotation = spawnPoints[r].transform.rotation;
            }

            transform.position = spawnPoint;
            transform.rotation = spawnRotation;
        }
    }
    public void GetKilled()
    {
        this.transform.DetachChildren();
        Destroy(this.gameObject);
    }
}
