using System;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        
        [Header("Clamp angles")]
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;
        
        [Header("Camera sensitivity")]
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 2.0f;
        [Tooltip("Camera shake intensity")]
        public Vector2 ShakeIntensity = new Vector2(0.1f, 0.1f);

        public Vector2 rotation = Vector2.zero;
        public Vector2 _shakeOffset = Vector2.zero;
        private Vector2 _playerHorzVelocity = Vector2.zero;

        private PlayerInputManager _inputManager;
        private PlayerMovement _playerMovement;
        
        private void Start()
        {
            _inputManager = GetComponent<PlayerInputManager>();
            _playerMovement = GetComponent<PlayerMovement>();

            var eulerAngles = transform.eulerAngles;
            rotation.y = eulerAngles.x;
            rotation.x = eulerAngles.y;
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            //Add the mouse input to the rotation
            rotation.x += _inputManager.CameraInput.y * RotationSpeed;
            rotation.y += _inputManager.CameraInput.x * RotationSpeed;

            //Clamp the rotation
            rotation.x = ClampAngle(rotation.x, BottomClamp, TopClamp);
            rotation.y = ClampAngle(rotation.y, -360, 360);
            
            //Calculate the shake offset
            _playerHorzVelocity = new Vector2(_playerMovement._velocity.x, _playerMovement._velocity.z);
            var t = Time.time * _playerHorzVelocity.magnitude;
            _shakeOffset = new Vector2(
                Mathf.Sin(t) * Mathf.Cos(t),
                Mathf.Sin(t)
            ) * ShakeIntensity;
            
            //Add the shake offset to the rotation
            rotation += _shakeOffset;
            
            //Rotate the camera
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(rotation.x, 0.0f, 0.0f);
            //Rotate the player
            transform.rotation = Quaternion.Euler(0.0f, rotation.y, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin , float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}