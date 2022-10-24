using UnityEngine;

namespace GameGlobals
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public bool isFirstTime = true;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameManager>();
                return _instance;
            }
        }
    
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }



    }
}
