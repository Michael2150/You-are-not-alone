using System.Collections.Generic;
using Enemy;
using GameGlobals;
using GameGlobals.GameManager_Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private LevelGenerationScript levelGenerator;
        [SerializeField] private GameObject collectiblePrefab;
        [SerializeField] private Dictionary<Difficulty, int> collectibleCount = new() {
            {Difficulty.Easy, 5},
            {Difficulty.Medium, 10},
            {Difficulty.Hard, 15}
        };

        private void Start()
        {
            if (GameManager.Instance.isFirstTime)
            {
                SetupPlayer();
            }
        }

        public void SetupPlayer()
        {
            //If the player already exists in the scene, destroy it
            if (GameObject.FindGameObjectWithTag("Player") != null)
                DestroyImmediate(GameObject.FindGameObjectWithTag("Player"));

            //Spawn the player at a random room in the level
            List<Vector3> roomPositions = levelGenerator.GetRoomPositions();
            var playerPosition = roomPositions[Random.Range(0, roomPositions.Count)];
            
            //Create a collectible root
            var collectibleRoot = new GameObject("Collectibles");
            collectibleRoot.transform.parent = transform;
            //Remove player position from the list of room positions
            roomPositions.Remove(playerPosition);
            //Spawn the required amount of collectibles
            for (var i = 0; i < collectibleCount[GameManager.Instance.SessionData._difficulty]; i++)
            {
                var collectiblePosition = roomPositions[Random.Range(0, roomPositions.Count)];
                roomPositions.Remove(collectiblePosition);
                Instantiate(collectiblePrefab, collectiblePosition - Vector3.up, Quaternion.identity, collectibleRoot.transform);
            }
            
            //Set up the key cards
            GameManager.Instance.CollectionManager.ResetCollection();
            GameManager.Instance.CollectionManager.GetAllKeyShardsInScene();
            
            //Spawn the player
            var player = Instantiate(playerPrefab, playerPosition + Vector3.up, Quaternion.identity);
            
            //Set the player as the target for the enemy
            var enemy = GameObject.FindGameObjectWithTag("Enemy");
            enemy.GetComponent<EnemyNav>().SetPlayer(player);
        }
    }
}