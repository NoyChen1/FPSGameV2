using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;

    //Shooting
    public bool isShooting;
    public bool readyToShoot;
    public bool allowReset = true;
    public float shootingDelay = 2f;

    //Burst
    public int bulletPerBurst = 3;
    public int burstBulletsLeft;

    //Spread
    public float spreadIntensity;

    //Bullet prop
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;


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
        burstBulletsLeft = bulletPerBurst;
    }
    // Update is called once per frame
    void Update()
    {
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
        if(readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletPerBurst;
            Fire();
        }
        /*
        if (Input.GetKeyDown(KeyCode.Mouse0)) //left mouse button
        {
            Fire();
        }
        */
    }

    private void Fire()
    {
        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        
        //Instantiate the buller
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        
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

    private void ResetShot()
    {
        allowReset = true;
        readyToShoot = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        //shooting from the middle of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
