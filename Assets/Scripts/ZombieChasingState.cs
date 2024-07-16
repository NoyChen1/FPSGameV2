using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChasingState : StateMachineBehaviour
{

    Transform player;
    NavMeshAgent agent;

    public float chasingSpeed = 6f;
    public float stopChasingDistance = 21f;
    public float attackingDistance = 2.5f;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = chasingSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!SoundManager.instance.ZombieChannel1.isPlaying)
        {
            SoundManager.instance.ZombieChannel1.PlayOneShot(SoundManager.instance.ZombieAttack);
        }

        agent.SetDestination(player.position);
        animator.transform.LookAt(player);

        float distanceFromThePlayer = Vector3.Distance(player.position, animator.transform.position);

        //checking if agent should stop chasing
        if(distanceFromThePlayer > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
        }

        //checking if agent should attacking
        if(distanceFromThePlayer < attackingDistance)
        {
            animator.SetBool("isAttacking", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
        SoundManager.instance.ZombieChannel1.Stop();
    }
}
