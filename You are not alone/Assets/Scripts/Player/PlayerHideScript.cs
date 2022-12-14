using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGlobals
{
    public class PlayerHideScript : MonoBehaviour
    {
        [Header("Player Hiding")]
        [SerializeField] private bool _isHiding = false;
        [SerializeField] private List<GameObject> hidingSpots = new List<GameObject>();

        [Header("Light level check")]
        public bool isCheckingForLightLevel = false;
        public RenderTexture lightLevelTexture;
        public float checkRate = 1f; //Per Second
        private float lightCheckTimer = 0f;
        [Range(0,1)] public float lightLevel;
        
        //Cache variables
        private RenderTexture _tempRenderTexture;
        private Texture2D _tempTexture;
        

        private void Start()
        {
            if (lightLevelTexture) return;
            lightLevel = 0;
            isCheckingForLightLevel = false;
        }

        private void FixedUpdate()
        {
            CheckLightLevel();
        }

        private void CheckLightLevel()
        {
            if (lightCheckTimer > 0)
            {
                lightCheckTimer -= Time.fixedDeltaTime;
                return;
            }

            lightCheckTimer = checkRate;
            
            if (!isCheckingForLightLevel) return;
            
            //Create a temporary RenderTexture to store the light level
            _tempRenderTexture = RenderTexture.GetTemporary(lightLevelTexture.width, lightLevelTexture.height, 0, RenderTextureFormat.ARGB32);
            //Copy the light level texture to the temporary RenderTexture
            Graphics.Blit(lightLevelTexture, _tempRenderTexture);
            //Create a new Texture2D to store the pixels of the temporary RenderTexture
            _tempTexture = new Texture2D(_tempRenderTexture.width, _tempRenderTexture.height, TextureFormat.ARGB32, false);
            //Read the pixels of the temporary RenderTexture into the Texture2D
            RenderTexture.active = _tempRenderTexture;
            _tempTexture.ReadPixels(new Rect(0, 0, _tempRenderTexture.width, _tempRenderTexture.height), 0, 0);
            _tempTexture.Apply();
            //Get the average color of the Texture2D
            Color[] pixels = _tempTexture.GetPixels();
            double[] averageColor = new double[3];
            for (var i = 0; i < pixels.Length; i+=5)
            {
                averageColor[0] += pixels[i].r;
                averageColor[1] += pixels[i].g;
                averageColor[2] += pixels[i].b;                
            }
            averageColor[0] /= pixels.Length;
            averageColor[1] /= pixels.Length;
            averageColor[2] /= pixels.Length;
            //Set the light level to the average color
            lightLevel = (float)((averageColor[0] + averageColor[1] + averageColor[2]) / 3);
            //Release the temporary RenderTexture and Texture2D
            RenderTexture.ReleaseTemporary(_tempRenderTexture);
            Destroy(_tempTexture);
        }

        public bool IsHiding
        {
            get => _isHiding;
            private set => _isHiding = value;
        }
        public void AddHidingSpot(GameObject hidingSpot)
        {
            hidingSpots.Add(hidingSpot);
            IsHiding = true;
        }
        public void RemoveHidingSpot(GameObject hidingSpot)
        {
            hidingSpots.Remove(hidingSpot);
            IsHiding = hidingSpots.Count > 0;
        }
    }
}
