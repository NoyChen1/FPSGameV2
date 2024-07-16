using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackingState : StateMachineBehaviour
{
    Transform player;
    NavMeshAgent agent;

    float stopAttackingDistance = 2.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAtPlayer();

        if (!SoundManager.instance.ZombieChannel1.isPlaying)
        {
            SoundManager.instance.ZombieChannel1.PlayOneShot(SoundManager.instance.ZombieAttack);
        }
        //check if enemy should stop attacking
        float distanceFromThePlayer = Vector3.Distance(player.position, animator.transform.position);
        if(distanceFromThePlayer > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
    }


    private void LookAtPlayer()
    {
        Vector3 direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation =  Quaternion.Euler(0, yRotation, 0);

        SoundManager.instance.ZombieChannel1.Stop();

    }
}
