using System;
using System.Collections.Generic;
using GameGlobals;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private LevelGenerationScript levelGenerator;
        [SerializeField] private GameObject collectiblePrefab;

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
            //Spawn a collectible in the remaining rooms
            foreach (var roomPosition in roomPositions)
            {
                var obj = Instantiate(collectiblePrefab, roomPosition - Vector3.up, Quaternion.identity);
                obj.transform.parent = collectibleRoot.transform;
            }


            //Spawn the player
            Instantiate(playerPrefab, playerPosition + Vector3.up, Quaternion.identity);
        }
    }
}