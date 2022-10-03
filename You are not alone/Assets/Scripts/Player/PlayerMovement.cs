using System;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Player")] [Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 5.0f;

		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 10.0f;

		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Header("Player Jumping")] [Tooltip("The strength of the player's jump")]
		public float JumpStrength = 1f;

		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.5f;

		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -9.8f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;

		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;

		[Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;

		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Player Crouching")] [Tooltip("If the character is crouching or not")]
		public bool Crouching = false;

		private PlayerInputManager _inputManager;
		private CharacterController _characterController;
		private float _currentSpeed = 0f;
		private float _jumpTimeout = 0f;

		private void Start()
		{
			_inputManager = GetComponent<PlayerInputManager>();
			_characterController = GetComponent<CharacterController>();
			
			//Remove the cursor
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			handleGrounded();
			handleMovement();
			handleCrouching();
		}

		private void handleGrounded()
		{
			//Check if the player is grounded
			Grounded = Physics.CheckSphere(transform.position + Vector3.up * GroundedOffset, GroundedRadius, GroundLayers);
		}
		
		private void handleMovement()
		{
			var horizontal = _inputManager.MovementInput.x;
			var vertical = _inputManager.MovementInput.y;
			var isSprinting = _inputManager.SprintInput;
			var isMoving = Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 0.0f;

			var targetSpeed = isMoving ? (isSprinting ? SprintSpeed : MoveSpeed) : 0.0f;
			_currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, SpeedChangeRate * Time.deltaTime);

			var moveDirection = transform.forward * vertical + transform.right * horizontal;
			var movement = moveDirection * _currentSpeed;

			_characterController.Move(movement * Time.deltaTime);
		}

		private void handleCrouching()
		{
			return;
		}

		private void OnDrawGizmos()
		{
			//Draw a sphere at the grounded check position, the color will change based on if the character is grounded or not
			handleGrounded();
			Gizmos.color = Grounded ? new Color(0f,1f,0f,0.2f) : new Color(1f,0f,0f,0.2f);
			Gizmos.DrawSphere(transform.position + Vector3.up * GroundedOffset, GroundedRadius);
		}
	}
}