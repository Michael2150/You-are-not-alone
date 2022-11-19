using System;
using GameGlobals.GameManager_Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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


        [SerializeField] private CollectionManager collectionManager = new CollectionManager();
        public CollectionManager CollectionManager => collectionManager;
        
        [SerializeField] private SessionData sessionData = new SessionData();
        public SessionData SessionData => sessionData;

        private void Start()
        {
            SessionData.OnSessionLoadUpdate += ((complete, msg) => Debug.Log(msg) );
        }
        
        public void OnCollectionComplete()
        {
            SessionData.EndSession(true);
            
            //Load the Game Over Scene
            SceneManager.LoadScene((int)Scenes.GAME_OVER);
        }
    }
}
