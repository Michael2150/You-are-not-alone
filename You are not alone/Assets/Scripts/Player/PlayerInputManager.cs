using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Player Input")]
    private PlayerInputSystem playerInputSystem;
    [SerializeField] private Vector2 movementInput;
    [SerializeField] private Vector2 cameraInput;
    [SerializeField] private bool jumpInput;
    [SerializeField] private bool crouchInput;
    [SerializeField] private bool sprintInput;

    public void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
    }

    private void Start()
    {
        //Subscribe to the started events
        playerInputSystem.Player.Move.started += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerInputSystem.Player.Look.started += ctx => cameraInput = ctx.ReadValue<Vector2>();
        playerInputSystem.Player.Jump.started += ctx => jumpInput = true;
        playerInputSystem.Player.Crouch.started += ctx => crouchInput = true;
        playerInputSystem.Player.Sprint.started += ctx => sprintInput = true;
        
        //Subscribe to the performed events
        playerInputSystem.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerInputSystem.Player.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

        //Subscribe to the canceled events
        playerInputSystem.Player.Move.canceled += ctx => movementInput = Vector2.zero;
        playerInputSystem.Player.Look.canceled += ctx => cameraInput = Vector2.zero;
        playerInputSystem.Player.Jump.canceled += ctx => jumpInput = false;
        playerInputSystem.Player.Crouch.canceled += ctx => crouchInput = false;
        playerInputSystem.Player.Sprint.canceled += ctx => sprintInput = false;
    }
    
    //Getters for the input values
    public Vector2 MovementInput => movementInput;
    public Vector2 CameraInput => cameraInput;
    public bool JumpInput => jumpInput;
    public bool CrouchInput => crouchInput;
    public bool SprintInput => sprintInput;

    private void OnEnable()
    {
        playerInputSystem.Enable();
    }
    
    private void OnDisable()
    {
        playerInputSystem = null;
    }

    private void OnDrawGizmos()
    {
        //Draw the movement input
        Gizmos.color = Color.blue;
        var RayDir = new Vector3(movementInput.x, 0, movementInput.y) * (SprintInput ? 4 : 2);
        Gizmos.DrawRay(transform.position, RayDir);
    }
}