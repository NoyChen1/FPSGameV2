using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalRefrences : MonoBehaviour
{
    public static GlobalRefrences instance { get; set; }
    
    public GameObject bulletImpactEffectPrefab;
    
    public GameObject grenadeExplosionEffect;
    public GameObject smokeGrenadeEffect;
    
    public GameObject bloodSprayEffect;

    public int zombiesKilled = 0;


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
}
