using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            print("hit " + collision.gameObject.name + " !");
            createBulletImpactEffect(collision);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            print("hit Wall");
            createBulletImpactEffect(collision);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Beer"))
        {
            print("hit a Beer Bottle");
            collision.gameObject.GetComponent<BeerBottle>().Explosion();
        }

        if (collision.gameObject.CompareTag("Enemy")) 
        {
            print("hit a Zombie");
            if (!collision.gameObject.GetComponent<Enemy>().isDead)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDemage(bulletDamage);
            }
            CreateBloodSprayEffect(collision);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("MilitaryEnv"))
        {
            print("hit Military Service");
            createBulletImpactEffect(collision);
            Destroy(gameObject);
        }
    }

    private void CreateBloodSprayEffect(Collision objectHitted)
    {
        ContactPoint contact = objectHitted.contacts[0];
        GameObject bloodSprayImpact = Instantiate(GlobalRefrences.instance.bloodSprayEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal));
        bloodSprayImpact.transform.SetParent(objectHitted.gameObject.transform);
    }

    void createBulletImpactEffect(Collision objectHitted)
    {
        ContactPoint contact = objectHitted.contacts[0];
        GameObject hole = Instantiate(GlobalRefrences.instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectHitted.gameObject.transform);

    }
}
