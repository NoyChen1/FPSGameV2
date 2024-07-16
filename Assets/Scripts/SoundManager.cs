using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; set; }

    [Header("Audio Channels")]
    public AudioSource emptyMagazineSound;

    public AudioSource ShoottingChannel;
    public AudioSource ReloadingChannel;
    public AudioSource ThrowablesChannel;
    public AudioSource ZombieChannel1;
    public AudioSource ZombieChannel2;
    public AudioSource PlayerChannel;

    [Header("Shooting")]
    public AudioClip M1911Shot;
    public AudioClip UziShot;


    [Header("Reload")]
    public AudioClip M1911Reload;
    public AudioClip UziReload;

    [Header("Throwables")]
    public AudioClip GrenadeSound;

    [Header("Zombie Sounds")]
    public AudioClip ZombieWalk;
    public AudioClip ZombieChase;
    public AudioClip ZombieAttack;
    public AudioClip ZombieHurt;
    public AudioClip ZombieDeath;

    [Header("Player Sound")]
    public AudioClip PlayerHurt;
    public AudioClip PlayerDeath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void playShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                ShoottingChannel.PlayOneShot(M1911Shot);
                break;
            case WeaponModel.Uzi:
                ShoottingChannel.PlayOneShot(UziShot);
                break;

        }
    }

    public void playReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                ReloadingChannel.PlayOneShot(M1911Reload);
                break;
            case WeaponModel.Uzi:
                ReloadingChannel.PlayOneShot(UziReload);
                break;

        }
    }
}
