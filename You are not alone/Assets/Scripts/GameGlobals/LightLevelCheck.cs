using System.Collections;
using UnityEngine;

namespace GameGlobals
{
    public class LightLevelCheck : MonoBehaviour
    {
        public RenderTexture lightLevelTexture;
        public float checkRate = 1f; //Per Second
        private float _timer = 0f;
        [Range(0,1)] public float lightLevel;
        
        //Cache variables
        private RenderTexture _tempRenderTexture;
        private Texture2D _tempTexture;

        private void Start()
        {
            if (!lightLevelTexture)
            {
                lightLevel = 0;
                enabled = false;
            }
        }
        
        private void Update()
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                return;
            }

            _timer = checkRate;
            StartCoroutine(CheckLightLevel());
        }
        
        private IEnumerator CheckLightLevel()
        {
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
            var averageColor = _tempTexture.GetPixelBilinear(0.5f, 0.5f);
            //Set the light level to the average color's alpha value
            lightLevel = averageColor.a;
            //Release the temporary RenderTexture and Texture2D
            RenderTexture.ReleaseTemporary(_tempRenderTexture);
            Destroy(_tempTexture);
            //Wait for the next frame
            yield return null;
        }
    }
}
