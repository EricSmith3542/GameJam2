using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    public Transform player;
    public float chaseDistance;
    public float stunTime;
    public float attackRange;
    public float attackDelay;

    public enum AIState { idle, chase, stunned, attacking }

    public AIState aiState = AIState.idle;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Think());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Think()
    {
        while (true)
        {
            float dist = Vector3.Distance(player.position, transform.position);
            switch (aiState)
            {
                case AIState.idle:
                    if (dist <= chaseDistance)
                    {
                        aiState = AIState.chase;
                        anim.SetBool("chase", true);
                    }
                    navMeshAgent.SetDestination(transform.position);
                    break;
                case AIState.chase:
                    if (dist > chaseDistance)
                    {
                        aiState = AIState.idle;
                        anim.SetBool("chase", false);
                    }
                    else if(dist <= attackRange)
                    {
                        aiState = AIState.attacking;
                    }
                    navMeshAgent.SetDestination(player.position);
                    break;
                case AIState.stunned:
                    anim.SetBool("stunned", true);
                    navMeshAgent.isStopped = true;
                    yield return new WaitForSeconds(stunTime);
                    aiState = AIState.idle;
                    navMeshAgent.isStopped = false;
                    anim.SetBool("stunned", false);
                    break;
                case AIState.attacking:
                    //Damage the player here
                    anim.SetBool("attacking", true);
                    yield return new WaitForSeconds(attackDelay);
                    aiState = AIState.chase;
                    anim.SetBool("attacking", false);
                    break;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
