using UnityEngine;

//This script is attached to the camera and will randomly sway around the first direction it is pointed in
namespace Menu
{
    public class CameraSwayScript : MonoBehaviour
    {
        public float intensity = 0.1f;   
        public float speed = 1f;
        private Quaternion originalRotation;
        public bool x_enabled = true;
        public bool y_enabled = true;
        public bool z_enabled = true;

        // Start is called before the first frame update
        void Start()
        {
            originalRotation = transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            //Use noise to create a random value using the speed and intensity
            float x = Mathf.PerlinNoise(Time.time * speed, 0) * intensity;
            float y = Mathf.PerlinNoise(Time.time * speed, 1) * intensity;
            float z = Mathf.PerlinNoise(Time.time * speed, 2) * intensity;
            
            //Create a new rotation based on the original rotation and the random values
            Quaternion newRotation = originalRotation;
            if (x_enabled)
                newRotation.x += x;
            if (y_enabled)
                newRotation.y += y;
            if (z_enabled)
                newRotation.z += z;
            
            //Apply the new rotation
            transform.rotation = newRotation;
        }
    }
}
