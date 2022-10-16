using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyNav : MonoBehaviour
    {
        [Header("Movement")]
        public float speed;
        public float stoppingDistance;
        private Vector3 _newPosition;
        
        [Header("Attack")]
        public float attackRange;
        public float attackSpeed;
        public float attackDamage;
        public float attackCooldown;
        public float attackCooldownTimer;
        public float attackTimer;
        public bool isAttacking;
        
        [Header("References")]
        public GameObject player;
        public NavMeshAgent agent;
        
        [Header("Sight")]
        public float fieldOfView;
        public float viewDistance;
        public LayerMask layerToSee;

        private void Start()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");

            if (agent == null)
                agent = GetComponent<NavMeshAgent>();

            agent.speed = speed;
            agent.stoppingDistance = stoppingDistance;
        }

        private void Update()
        {
            //If the player is in the enemy's field of view and within the enemy's view distance, the enemy will move towards the player, else tha enemy will roam around
            if (inFOV)
            {
                Position = (player.transform.position);
            }
            else
            {
                //If the enemy is not moving, it will generate a new position to move to
                if (!agent.hasPath)
                {
                    Position += new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                } else if (agent.remainingDistance <= agent.stoppingDistance) //If the enemy is close enough to the position it is moving to, it will generate a new position to move to
                {
                    Position += new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                }
            }
        }

        private Vector3 Position
        {
            get
            {
                if (_newPosition == Vector3.zero)
                    _newPosition = transform.position;
                return _newPosition;
            }
            set
            {
                _newPosition = value;
                agent.SetDestination(_newPosition);
            }
        }
    
        public bool inFOV
        {
            get
            {
                //Check if the player is within the field of view
                Vector3 direction = player.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);
                if (angle > fieldOfView * 0.5f)
                    return false;

                //Check whether the player is within the view distance
                if (direction.magnitude > viewDistance)
                    return false;
                
                return true;
            }
        }
    
        public bool inAttackRange
        {
            get
            {
                var distance = Vector3.Distance(transform.position, player.transform.position);
                return distance < attackRange;
            }
        }

        private void OnDrawGizmos()
        {
            //Draw FOV and if the player is in it
            Gizmos.color = player ? inFOV ? Color.green : Color.red : Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
            Gizmos.DrawRay(transform.position, 
            Quaternion.AngleAxis(fieldOfView * 0.5f, transform.up) * transform.forward * viewDistance);
            Gizmos.DrawRay(transform.position, 
            Quaternion.AngleAxis(-fieldOfView * 0.5f, transform.up) * transform.forward * viewDistance);
            
            //Draw attack range and if the player is in it
            Gizmos.color = player ? inAttackRange ? Color.green : Color.red : Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            //Draw the position the enemy is moving to
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Position, 0.5f);
        }
    }
}
