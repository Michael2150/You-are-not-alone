using System;
using System.Collections;
using Generation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace GameGlobals.GameManager_Components
{
    [Serializable]
    public class SessionData
    {
        //Keep track of the current session time
        [SerializeField]  float sessionStartTime = 0.0f;
        [SerializeField]  float sessionEndTime = 0.0f;
        [SerializeField]  int _seed = 0;
        public bool DidWin { get; private set; } = false;
        
        public event Action<float, string> OnSessionLoadUpdate;

        public void StartSession()
        {
            //Start the session with a random seed value between 0 and max int
            Random.InitState(NewSeed);
        }
        public void StartSession(int seed)
        {
            //Seed the random number generator
            Seed = seed;
            
            LoadSession();
            GameManager.Instance.CollectionManager.ResetCollection();
            GameManager.Instance.CollectionManager.GetAllKeyShardsInScene();
            sessionStartTime = Time.time;
        }

        void LoadSession()
        {
            //Loading Scene
            OnSessionLoadUpdate?.Invoke(0, "Loading Scene");
            SceneManager.LoadSceneAsync("Level_Gen", LoadSceneMode.Single);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                //Check if the scene is the level gen scene
                if (scene.name != "Level_Gen") return;

                //Get the gameobject with the level generator tag and call the generate level function
                GameObject levelGenerator = GameObject.FindGameObjectWithTag("LevelGenerator");
                
                //Generating Level
                OnSessionLoadUpdate?.Invoke(0.5f, "Generating Level with seed: " + Seed);
                var genScript = levelGenerator.GetComponent<LevelGenerationScript>();
                genScript.ClearLevel();
                genScript.GenerateLevel(Seed);

                //Building Level
                OnSessionLoadUpdate?.Invoke(0.75f, "Building Level");
                var buildScript = levelGenerator.GetComponent<LevelBuilderScript>();
                buildScript.DestroyLevel();
                buildScript.BuildLevel();
                
                //Setup Player
                OnSessionLoadUpdate?.Invoke(0.9f, "Setting up Player");
                var playerSetup = levelGenerator.GetComponent<PlayerSetup>();
                playerSetup.SetupPlayer();
                
                //Loading Complete
                OnSessionLoadUpdate?.Invoke(1, "Loading Complete");
                //Destroy the current scene and move to the one just loaded
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_Gen"));
            };
        }

        public void EndSession(bool Win)
        {
            sessionEndTime = Time.time;
            DidWin = Win;
        }

        public int NewSeed => Random.Range(1, int.MaxValue);
        public int Seed
        {
            get => _seed;
            private set
            {
                _seed = value;
                Random.InitState(_seed);
            }
        }

        public float SessionStartTime => sessionStartTime;
        public float SessionEndTime => sessionEndTime;
        public float SessionTime => sessionEndTime - sessionStartTime;
    }
}