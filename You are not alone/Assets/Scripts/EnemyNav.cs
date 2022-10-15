using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    //The enemy should be able to hear and see the player. If the player is detected the enemy should follow the player until the player escapes
    //the enemy's field of view or the enemy loses the player's sound. The enemy should be able to attack the player if the player is within
    //the enemy's attack range. The enemy should be able to move around the map and avoid obstacles.
    
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;
    public float attackRange;
    public float attackSpeed;
    public float attackDamage;
    public float attackCooldown;
    public float attackCooldownTimer;
    public float attackTimer;
    
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    public float fieldOfView;
    public float viewDistance;
    public float hearingDistance;
    public float hearingRange;
    
    public bool playerDetected;
    public bool playerInAttackRange;
    public bool playerInHearingRange;
    public bool playerInFieldOfView;
    public bool playerInSight;
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Check if the player is in the enemy's field of view
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        playerInFieldOfView = angle < fieldOfView * 0.5f;

        //Check if the player is in the enemy's line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, viewDistance))
        {
            playerInSight = hit.collider.CompareTag("Player");
        }

        //Check if the player is in the enemy's hearing range
        if (Vector3.Distance(transform.position, player.position) < hearingDistance)
        {
            playerInHearingRange = true;
        }
        else
        {
            playerInHearingRange = false;
        }

        //Check if the player is in the enemy's attack range
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            playerInAttackRange = true;
        }
        else
        {
            playerInAttackRange = false;
        }

        //Check if the player is detected
        if (playerInFieldOfView && playerInSight || playerInHearingRange)
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }

        //If the player is detected the enemy should follow the player
        if (playerDetected)
        {
            agent.SetDestination(player.position);
            //animator.SetBool(IsWalking, true);
        }
        else
        {
            //animator.SetBool(IsWalking, false);
        }

        //If the player is in the enemy's attack range the enemy should attack the player
        if (playerInAttackRange)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            //animator.SetBool(IsAttacking, true);
        }
        else
        {
            //animator.SetBool(IsAttacking, false);
        }

        //If the player is in the enemy's attack range the enemy should attack the player
        if (playerInAttackRange)
        {
            agent.SetDestination(transform.position);
            transform.LookAt(player);
        }
    }

    void Attack()
    {
        //Attack the player
        Debug.Log("Attack");
    }

    private void OnDrawGizmos()
    {
        //Draw the enemy's field of view
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView, transform.up) * transform.forward * viewDistance;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView, transform.up) * transform.forward * viewDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        //Draw the enemy's line of sight
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);

        //Draw the enemy's hearing range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingDistance);

        //Draw the enemy's attack range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
