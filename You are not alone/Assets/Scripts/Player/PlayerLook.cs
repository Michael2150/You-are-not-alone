using System;
using UnityEngine;

namespace Player
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 2.0f;

        private float _cinemachineTargetPitch = 0.0f;
        private float _rotationVelocity = 0.0f;
        private PlayerInputManager _inputManager;
        
        private void Start()
        {
            _inputManager = GetComponent<PlayerInputManager>();
            
            // Lock and hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
            _cinemachineTargetPitch += _inputManager.CameraInput.y * RotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _inputManager.CameraInput.x * RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                    //return _playerInput.currentControlScheme == "KeyboardMouse";
                    return true;
#else
				    return false;
#endif
            }
        }
    }
}