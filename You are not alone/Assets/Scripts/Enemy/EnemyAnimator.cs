using UnityEngine;

namespace Enemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        private Animator _animator;
        private EnemyStateMachine _stateMachine;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _stateMachine = GetComponent<EnemyStateMachine>();
            _stateMachine.OnStateChanged += OnStateChanged;
        }
        
        private void OnStateChanged(EnemyStates state)
        {
            
        }
    }
}