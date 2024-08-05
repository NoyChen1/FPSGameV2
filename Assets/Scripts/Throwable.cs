using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Throwable : MonoBehaviour
{

    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;

    float countDown;
    bool hasExploded = false;
    
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        None,
        Grenade,
        Smoke_Grenade
    }

    public ThrowableType throwableType;
    private int grenadeDamage = 100;

    private void Start()
    {
        countDown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown) // if thrown, start the countdown
        {
            countDown -= Time.deltaTime;
            if(countDown <= 0f && !hasExploded) // countDown over && granade didn't exploded yet
            {
                Exploded();
                hasExploded = true;
            }
        }
    }

    private void Exploded()
    {
        GetThroableEffect();
        Destroy(gameObject);
    }

    private void GetThroableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.Smoke_Grenade:
                SmokeGrenadeEffect();
                break;

            default:
                return;
        }
    }

    private void SmokeGrenadeEffect()
    {

        //visual effect
        GameObject smokeEffect = GlobalRefrences.instance.smokeGrenadeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);


        //play Sound
        SoundManager.instance.ThrowablesChannel.PlayOneShot(SoundManager.instance.GrenadeSound);

        //physical effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //blind enemies
            }
        }
    }

    private void GrenadeEffect()
    {

        //visual effect
        GameObject explosionEffect = GlobalRefrences.instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);


        //play Sound
        SoundManager.instance.ThrowablesChannel.PlayOneShot(SoundManager.instance.GrenadeSound);

        //physical effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            if(objectInRange.CompareTag("Enemy"))
            {
                //make sure that dead enemy can't day twice
                if (objectInRange.GetComponent<Enemy>().state != Enemy.EnemyState.Dead)
                {
                    objectInRange.GetComponent<Enemy>().TakeDemage(grenadeDamage);
                }
            }

            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            /*
            if (objectInRange.GetComponent<Enemy>())
            {
                objectInRange.GetComponent<Enemy>().TakeDemage(grenadeDamage);
            }
            */
        }

    }
}
