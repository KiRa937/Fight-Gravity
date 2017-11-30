using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpecs
{
    public Weapons type;

    public GameObject ammo;
    public float cooldown;
    public float recoil;
    public float bulletTime;
	public float bulletSpeed;
    public AudioClip shotAudio;

    public WeaponSpecs(WeaponSpecs other)
    {
        type = other.type;
        ammo = other.ammo;
        cooldown = other.cooldown;
        recoil = other.recoil;
        bulletTime = other.bulletTime;
		bulletSpeed = other.bulletSpeed;
        shotAudio = other.shotAudio;
    }

    public WeaponSpecs(Weapons ctype, GameObject cammo, AudioClip cclip)
    {
        type = ctype;
		ammo = cammo;
        shotAudio = cclip;

        switch (type)
        {
            case Weapons.shotgun:
                {
                    bulletSpeed = 10;
					bulletTime = 1.5f;
					recoil = 500;
					cooldown = .7f;
					break;
                }
            case Weapons.bazooka:
                {
                    bulletSpeed = 7;
					bulletTime = 5f;
					recoil = 50;
					cooldown = 1f;
					break;
                }
            case Weapons.flamethrower:
                {
                    bulletSpeed = 0;
					bulletTime = .05f;
					recoil = 20;
					cooldown = .1f;
                    break;
                }
        }
    }

}

public enum Weapons
{
    shotgun,
    bazooka,
    flamethrower,
    firstAid
}