using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPoint : MonoBehaviour
{

    public Weapons item = Weapons.bazooka;

    public float cooldown = 10;
    private float cdTimer;

    public GameObject[] ammo;

    public AudioClip[] audioClips;

    public AudioClip[] audioShotClips;

    private GameObject curAmmo;
    private AudioClip curAudio;
    private Color baseColor;

    // Use this for initialization
    void Start()
    {
        cdTimer = cooldown;

        //SpriteRenderer sr = GetComponent<SpriteRenderer>();
        //Color cl = new Color();

        AudioSource aSrc = GetComponent<AudioSource>();

        switch (item)
        {
            case Weapons.bazooka:
                {
                    curAmmo = ammo[0];
                    //ColorUtility.TryParseHtmlString("575A2CFF", out cl);
                    baseColor = Color.yellow;
                    aSrc.clip = audioClips[0];
                    curAudio = audioShotClips[0];
                    break;
                }
            case Weapons.shotgun:
                {
                    curAmmo = ammo[1];
                    //ColorUtility.TryParseHtmlString("442C5AFF", out cl);
                    baseColor = Color.blue;
                    aSrc.clip = audioClips[1];
                    curAudio = audioShotClips[1];
                    break;
                }
            case Weapons.flamethrower:
                {
                    curAmmo = ammo[2];
                    //ColorUtility.TryParseHtmlString("5A362CFF", out cl);
                    baseColor = Color.red;
                    aSrc.clip = audioClips[2];
                    curAudio = audioShotClips[2];
                    break;
                }
            case Weapons.firstAid:
                {
                    //ColorUtility.TryParseHtmlString("2C5A34FF", out cl);
                    aSrc.clip = audioClips[3];
                    baseColor = Color.green;
                    break;
                }
        }

        GetComponent<SpriteRenderer>().color = baseColor;

    }

    // Update is called once per frame
    void Update()
    {
        cdTimer += Time.deltaTime;

        if (cdTimer >= cooldown)
            GetComponent<SpriteRenderer>().color = baseColor;
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && cdTimer > cooldown)
        {
            if (item != Weapons.firstAid)
            {
                other.GetComponent<PlrController>().curWeapon = new WeaponSpecs(item, curAmmo, curAudio);
            }
            else
            {
                other.GetComponent<PlrController>().TakeHealth(50);
            }

            cdTimer = 0;

            GetComponent<AudioSource>().Play();

            Color dark = GetComponent<SpriteRenderer>().color;
            dark.r -= .7f;
            dark.g -= .7f;
            dark.b -= .7f;
            GetComponent<SpriteRenderer>().color = dark;
        }
    }

}
