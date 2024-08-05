using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public ZombieHand ZombieHand;
    public int zombieDamage;


    private void Start()
    {
        ZombieHand.damage = zombieDamage;
    }

    private void Update()
    {
        
        if (this.GetComponent<Enemy>().state == Enemy.EnemyState.Dead)
        {
            ZombieHand.gameObject.SetActive(false);
        }
    }
}
