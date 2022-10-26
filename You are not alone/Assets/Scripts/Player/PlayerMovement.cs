using System;
using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		//References
		public CharacterController _controller;
		private PlayerInputManager _inputManager;
		
		[Header("Movement")]
		public float speed = 5f;
		public float runningSpeed = 10f;
		public float crouchSpeed = 2f;

		[Header("Jumping")]
		public float jumpForce = 5f;
		public float gravity = -9.81f;
		
		[Header("Crouching")]
		public float crouchHeight = 1f;
		public float crouchSpeedMultiplier = 0.5f;

		[Header("Debug")]
		[SerializeField] public Vector3 _velocity;
		[SerializeField] private float _currentSpeed;
		[SerializeField] private bool _crouching;
		
		//Cache variables
		private Camera _playerCamera;
		private float _defaultHeight;

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_inputManager = GetComponent<PlayerInputManager>();
			_velocity = Vector3.zero;
			_playerCamera = Camera.main;
			_defaultHeight = _controller.height;
		}

		private void Update()
		{
			//Movement
			CalculateMovement();
			CalculateJumping();
			//Apply the resulting velocity to the controller
			_controller.Move(_velocity * Time.deltaTime);
			
			//Crouching
			HandleCrouch();
		}

		private void CalculateMovement()
		{
			//Calculate movement velocity from the input
			var input = _inputManager.MovementInput * 
							(_inputManager.CrouchInput ? crouchSpeed : 
				            (_inputManager.SprintInput ? runningSpeed : speed));
			
			var targetVelocity = new Vector3(input.x, _velocity.y, input.y);
			targetVelocity = transform.TransformDirection(targetVelocity);
			_velocity = targetVelocity;
			_currentSpeed = _velocity.magnitude;
		}
		
		private void CalculateJumping()
		{
			//Calculate Jumping & Gravity
			if (_controller.isGrounded && _inputManager.JumpInput)
			{
				_velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
			} 
			else
			{
				if (_controller.isGrounded)
					_velocity.y = 0;
				else
					_velocity.y += gravity * Time.deltaTime;
			}
		}
		
		private void HandleCrouch()
		{
			//Crouching
			if (_inputManager.CrouchInput == _crouching) return;
			
			_crouching = _inputManager.CrouchInput;
		}

		private void OnDrawGizmos()
		{
			//Draw sphere to show grounded area and if player is grounded
			Gizmos.color = _controller.isGrounded ? Color.green : Color.red;
			Gizmos.DrawSphere(transform.position + new Vector3(0, -0.55f, 0), 0.5f);
		}
	}
}