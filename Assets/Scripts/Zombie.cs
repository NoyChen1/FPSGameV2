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
        
        if (this.GetComponent<Enemy>().isDead)
        {
            ZombieHand.gameObject.SetActive(false);
        }
    }
}
