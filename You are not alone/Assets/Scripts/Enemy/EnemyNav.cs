using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyNav : MonoBehaviour
    {
        public float speed;
        public float stoppingDistance;
        public float retreatDistance;
        public float attackRange;
        public float attackSpeed;
        public float attackDamage;
        public float attackCooldown;
        public float attackCooldownTimer;
        public float attackTimer;
        public bool isAttacking;
    
        public GameObject player;
        public NavMeshAgent agent;
        private Vector3 _newPosition;
    
        public float fieldOfView;
        public float viewDistance;
        public float hearingDistance;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
        
        }

        
        private Vector3 Position
        {
            get => _newPosition;
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
                if (angle > fieldOfView)
                    return false;

                //Check whether the player is within the view distance
                if (direction.magnitude > viewDistance)
                    return false;
                
                //Check every object between the enemy and the player while ignoring the enemy layer
                GameObject closestObject = null;
                float closestDistance = Mathf.Infinity;
                RaycastHit[] hits = new RaycastHit[] { };
                var enemyLayer = LayerMask.NameToLayer("Enemy");
                for (int i = 0; i < Physics.RaycastNonAlloc(transform.position, direction.normalized, hits, direction.magnitude); i++)
                {
                    RaycastHit hit = hits[i];
                    
                    if (hit.collider.gameObject.layer == enemyLayer)
                        continue;

                    if (closestObject)
                    {
                        if (!(hit.distance < closestDistance)) 
                            continue;
                        
                        closestObject = hit.collider.gameObject;
                        closestDistance = hit.distance;
                    } else {
                        closestObject = hit.collider.gameObject;
                        closestDistance = hit.distance;
                    }
                }
                
                //If there is nothing between the enemy and the player, return true
                if (closestObject == null) 
                    return false;
                
                //If the closest object is the player, return true
                if (closestObject.CompareTag("Player"))
                    return true;
                return false;
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
            var transform1 = transform;
            Gizmos.DrawRay(transform1.position, transform1.forward * viewDistance);
            Gizmos.DrawRay(transform1.position, Quaternion.AngleAxis(fieldOfView * 0.5f, transform1.up) * transform1.forward * viewDistance);
            Gizmos.DrawRay(transform1.position, Quaternion.AngleAxis(-fieldOfView * 0.5f, transform1.up) * transform1.forward * viewDistance);
        
            //Draw attack range and if the player is in it
            Gizmos.color = player ? inAttackRange ? Color.green : Color.red : Color.red;
            Gizmos.DrawWireSphere(transform1.position, attackRange);
        }
    }
}
