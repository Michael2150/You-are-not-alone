using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateMachine : MonoBehaviour
    {
        private EnemyStates _currentState;
        private EnemyStates _previousState;
        public event Action<EnemyStates> OnStateChanged;
        
        private void Start()
        {
            CurrentState = EnemyStates.Idle;
        }
        
        public EnemyStates CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value) return;
                _previousState = _currentState;
                _currentState = value;
                OnStateChanged?.Invoke(_currentState);
            }
        }
    }
    
    public enum EnemyStates
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Retreat,
        Stunned,
    }
}