using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolingState : StateMachineBehaviour
{
    float timer;
    public float patrolingTime = 10f;

    Transform player;
    NavMeshAgent agent;

    public float detectionAreaRadius = 18f;
    public float patrolSpeed = 2f;

    List<Transform> wayPointsList = new List<Transform>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        //making the enemy Patroling
        GameObject wayPointCluster = GameObject.FindGameObjectWithTag("WayPoints");
        foreach(Transform t in wayPointCluster.transform)
        {
            wayPointsList.Add(t);
        }

        Vector3 newPosition = wayPointsList[Random.Range(0, wayPointsList.Count)].position;
        agent.SetDestination(newPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!SoundManager.instance.ZombieChannel1.isPlaying)
        {
            SoundManager.instance.ZombieChannel1.clip = SoundManager.instance.ZombieWalk;
            SoundManager.instance.ZombieChannel1.PlayDelayed(1f);
        }
        //check if enemy arrived to new position
        //if yes, move to another position
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(wayPointsList[Random.Range(0, wayPointsList.Count)].position);
        }

        //increase the timer
        //if the patroling time is over, go back to the idle state
        timer += Time.deltaTime;
        if(timer > patrolingTime)
        {
            Debug.Log("Transitioning to Patrolling");
            animator.SetBool("isPatrolling", false);
        }

        //make transition to the chasing state
        //if player inside the enemy detection radius
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Stop the agent
        //setting the agent position to its destination
        agent.SetDestination(agent.transform.position);

        SoundManager.instance.ZombieChannel1.Stop();

    }
}
