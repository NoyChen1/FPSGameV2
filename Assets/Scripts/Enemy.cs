using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private NavMeshAgent navAgent;
    public EnemyState state;

    //public bool isDead;
    public float deadBodyDelay = 20f;

    public enum EnemyState
    {
        None,
        Dead
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

 
    public void TakeDemage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            int randomValue = Random.Range(0, 2); // 0 or 1

            if(randomValue == 0)
            {
                animator.SetTrigger("Die1");
            }
            else
            {
                animator.SetTrigger("Die2");
            }

            state = EnemyState.Dead;
            //isDead = true;

            //Death sound
            SoundManager.instance.ZombieChannel2.PlayOneShot(SoundManager.instance.ZombieDeath);
        }
        else
        {
            animator.SetTrigger("Damage");
            
            //zombie hurts
            SoundManager.instance.ZombieChannel2.PlayOneShot(SoundManager.instance.ZombieHurt);

        }

        if (state == EnemyState.Dead) 
        {
            StartCoroutine(DestroyBody());
        }
    }

    private IEnumerator DestroyBody()
    {
        yield return new WaitForSeconds(deadBodyDelay);
        // Destroy(this.gameObject);
        ObjectPoolManager.returnObjectToPool(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 2.5f); //attacking / stop attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 18f); //start chasing area

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 21f); //stop chasing
    }
}
