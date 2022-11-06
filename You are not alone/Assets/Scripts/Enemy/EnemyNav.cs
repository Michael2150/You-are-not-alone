using System;
using System.Collections.Generic;
using System.Linq;
using Generation;
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
        private Vector3 _target;
        public Vector2Int _currentPositionInGrid = Vector2Int.zero;
        public List<Vector2Int> _neighbours;
        
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
        public LevelGenerationScript levelGen;
        
        [Header("Sight")]
        public float fieldOfView;
        public float viewDistance;
        public LayerMask layerToSee;
        private bool _islevelGenNull;

        private void Start()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");

            if (agent == null)
                agent = GetComponent<NavMeshAgent>();
            
            if (levelGen == null)
                levelGen = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerationScript>();
            
            _islevelGenNull = levelGen == null;   
            
            agent.speed = speed;
            agent.stoppingDistance = stoppingDistance;
        }

        private void Update()
        {
            //If the player is in the enemy's field of view and within the enemy's view distance, the enemy will move towards the player, else tha enemy will roam around
            if (InFOV)
            {
                TargetPosition = (player.transform.position);
            }
            else
            {
                if (_islevelGenNull) return;
                _currentPositionInGrid = levelGen.GetGridPosition(transform.position);
                if (agent.hasPath) return;
                if (agent.remainingDistance > agent.stoppingDistance) return;
                _neighbours = levelGen.RoomGraph.Neighbours(_currentPositionInGrid);
                if (_neighbours.Count == 0) return;
                var randomNeighbour = _neighbours[Random.Range(0, _neighbours.Count)];
                TargetPosition = levelGen.GetPositionInGrid(randomNeighbour);
            }
        }

        private Vector3 TargetPosition
        {
            get
            {
                if (_target == Vector3.zero)
                    _target = transform.position;
                return _target;
            }
            set
            {
                _target = value;
                agent.SetDestination(_target);
            }
        }
    
        public bool InFOV
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
            Gizmos.color = player ? InFOV ? Color.green : Color.red : Color.yellow;
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
            Gizmos.DrawSphere(TargetPosition, 0.5f);
        }
    }
}
