using UnityEngine;

namespace Player
{
    public class PlayerAnimatorScript : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] PlayerMovement playerMovement;
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void FixedUpdate()
        {
            animator.SetFloat(Speed, playerMovement.CurrentSpeed);
        }
    }
}
