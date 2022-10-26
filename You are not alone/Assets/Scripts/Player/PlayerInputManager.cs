using UnityEngine;

namespace Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Player Input")]
        private PlayerInputSystem _playerInputSystem;
        [SerializeField] private Vector2 movementInput;
        [SerializeField] private Vector2 cameraInput;
        [SerializeField] private bool jumpInput;
        [SerializeField] private bool crouchInput;
        [SerializeField] private bool sprintInput;

        public void Awake()
        {
            _playerInputSystem = new PlayerInputSystem();
        }

        private void Start()
        {
            //Subscribe to the started events
            _playerInputSystem.Player.Move.started += ctx => movementInput = ctx.ReadValue<Vector2>();
            _playerInputSystem.Player.Look.started += ctx => cameraInput = ctx.ReadValue<Vector2>();
            _playerInputSystem.Player.Jump.started += ctx => jumpInput = true;
            _playerInputSystem.Player.Crouch.started += ctx => crouchInput = true;
            _playerInputSystem.Player.Sprint.started += ctx => sprintInput = true;
        
            //Subscribe to the performed events
            _playerInputSystem.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            _playerInputSystem.Player.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

            //Subscribe to the canceled events
            _playerInputSystem.Player.Move.canceled += ctx => movementInput = Vector2.zero;
            _playerInputSystem.Player.Look.canceled += ctx => cameraInput = Vector2.zero;
            _playerInputSystem.Player.Jump.canceled += ctx => jumpInput = false;
            _playerInputSystem.Player.Crouch.canceled += ctx => crouchInput = false;
            _playerInputSystem.Player.Sprint.canceled += ctx => sprintInput = false;
        }
    
        //Getters for the input values
        public Vector2 MovementInput => movementInput.normalized;
        public Vector2 CameraInput => cameraInput;
        public bool JumpInput => jumpInput;
        public bool CrouchInput => crouchInput;
        public bool SprintInput => sprintInput;

        private void OnEnable()
        {
            _playerInputSystem.Enable();
        }
    
        private void OnDisable()
        {
            _playerInputSystem = null;
        }

        private void OnDrawGizmos()
        {
            //Draw the movement input
            Gizmos.color = Color.blue;
            var RayDir = new Vector3(movementInput.x, 0, movementInput.y) * (SprintInput ? 4 : 2);
            Gizmos.DrawRay(transform.position, RayDir);
        }
    }
}