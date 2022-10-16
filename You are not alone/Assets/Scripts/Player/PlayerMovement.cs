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
		
		[Header("Jumping")]
		public float jumpForce = 5f;
		public float gravity = -9.81f;

		[Header("Debug")]
		[SerializeField] private Vector3 _velocity;

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_inputManager = GetComponent<PlayerInputManager>();
		}

		private void Update()
		{
			//Calculate movement velocity from the input
			var input = _inputManager.MovementInput * 
			            (_inputManager.SprintInput ? runningSpeed : speed);
			_velocity = new Vector3(input.x, _velocity.y, input.y);
			_velocity = transform.TransformDirection(_velocity);
			
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
			
			//Apply the resulting velocity to the controller
			_controller.Move(_velocity * Time.deltaTime);
		}
		
		private void OnDrawGizmos()
		{
			//Draw sphere to show grounded area and if player is grounded
			Gizmos.color = _controller.isGrounded ? Color.green : Color.red;
			Gizmos.DrawSphere(transform.position + new Vector3(0, -0.55f, 0), 0.5f);
		}
	}
}