using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public bool isActiveWeapon;
    public int weaponDamage;

    [Header("Shooting")]
    public bool isShooting;
    public bool readyToShoot;
    public bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float ADSSpreadIntensity;


    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    //Muzzle and sound
    public GameObject muzzleEffect;
    internal Animator animator; //is accesable to othe scripts

    [Header("Loading")]
    public float reloadTime;
    public int magazineSize;
    public int bulletsLeft;
    public bool isReloading;


    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    bool isADS;
    public enum WeaponModel
    {
        Pistol1911,
        Uzi
    }

    public WeaponModel thisWeapon;
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShottingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        
        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;

    }
    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon)
        {

            if (Input.GetMouseButtonDown(1)) // as long as holding right mouse button
            {
                EnterADS();
            }
            if (Input.GetMouseButtonUp(1)) // leaving right mouse button
            {
                ExitADS();

            }


            //if the weapon is active it wont get outlined
            GetComponent<Outline>().enabled = false;

            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.instance.emptyMagazineSound.Play();
            }
            if (currentShottingMode == ShootingMode.Auto)
            {
                //holding the left mouse
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShottingMode == ShootingMode.Single ||
                currentShottingMode == ShootingMode.Burst)
            {
                //clicking the left mouse
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && 
                bulletsLeft < magazineSize &&
                !isReloading &&
                WeaponManager.instance.CheckAmmoLeftFor(thisWeapon) > 0)
            {
                Reload();
            }

            //automatic Reload when the magazine is empty
            //if magazine is empty & we're reloading & ready to shoot &  we're not shooting
            /*
             if (bulletsLeft <= 0 && !isReloading && readyToShoot && !isShooting)
             {
                 Reload();
             }
            */


            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                Fire();
            }

        }

        /*
        if (Input.GetKeyDown(KeyCode.Mouse0)) //left mouse button
        {
            Fire();
        }
        */
    }


    private void EnterADS()
    {
        animator.SetTrigger("EnterADS");
        isADS = true;
        HUDManager.instance.crossHair.SetActive(false);
        spreadIntensity = ADSSpreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("ExitADS");
        isADS = false;
        HUDManager.instance.crossHair.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }
    private void Fire()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("RecoilADS");
        }
        else
        {
            animator.SetTrigger("Recoil");
        }
       
        SoundManager.instance.playShootingSound(thisWeapon);


        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        //Instantiate the buller
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        bullet.transform.forward = shootingDirection;
        //Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        //Destroy the bullet
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        //checking if we are done shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        //Burst mode
        if (currentShottingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("Fire", shootingDelay);
        }
    }


    private void Reload()
    {
        SoundManager.instance.playReloadSound(thisWeapon);
        animator.SetTrigger("Reload");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        int bulletsToDecrease;
        if (bulletsLeft + WeaponManager.instance.CheckAmmoLeftFor(thisWeapon) > magazineSize)
        {
            bulletsToDecrease = magazineSize - bulletsLeft;
            bulletsLeft = magazineSize;
            WeaponManager.instance.DecreaseWeaponAmount(bulletsToDecrease, thisWeapon);
        }
        else
        {
            bulletsToDecrease = WeaponManager.instance.CheckAmmoLeftFor(thisWeapon);
            bulletsLeft += bulletsToDecrease;
            WeaponManager.instance.DecreaseWeaponAmount(bulletsToDecrease, thisWeapon);
        }

        isReloading = false;   
    }
    private void ResetShot()
    {
        allowReset = true;
        readyToShoot = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        //shooting from the middle of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            //hit target
            targetPoint = hit.point;
        }
        else
        {
            //shooting in the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
