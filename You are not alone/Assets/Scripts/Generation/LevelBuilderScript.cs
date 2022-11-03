using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Generation
{
    public class LevelBuilderScript : MonoBehaviour
    {
        [SerializeField] private LevelGenerationScript LevelGen;
        [SerializeField] private GameObject[] prefabs = {};
        
        public void BuildLevel()
        {
            //Create a plane to build the level on
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(LevelGen.Grid.Size.x/1.5f, 
                                                    1f, 
                                                    LevelGen.Grid.Size.y/1.5f);
            plane.transform.position = new Vector3((LevelGen.Grid.Size.x * LevelGen.BlockSize.x)/2f, 
                                                    -LevelGen.BlockSize.y/2f,
                                                    (LevelGen.Grid.Size.y * LevelGen.BlockSize.z)/2f);
            plane.transform.parent = transform;
            
            //Get all the block datas from the prefab array
            var allBlockGenData = new List<BlockGenData>();
            foreach (var prefab in prefabs)
            {
                var blockGenData = prefab.GetComponents<BlockGenData>();
                allBlockGenData.AddRange(blockGenData);
            }

            //Create the level
            for (int x = 0; x < LevelGen.Grid.Size.x; x++)
            {
                for (int y = 0; y < LevelGen.Grid.Size.y; y++)
                {
                    if (EnumHelpers.CellIsOccupied(LevelGen.Grid[x, y]))
                    {
                        Debug.Log("(" + x + ", " + y + ")");
                    }

                    var blockDataForThisPos = new List<BlockGenData>();
                    foreach (var blockGenData in allBlockGenData)
                    {
                        if (blockGenData.NeighbourCheck(LevelGen.Grid, new Vector2Int(x, y)))
                            blockDataForThisPos.Add(blockGenData); 
                    }
                    
                    if (blockDataForThisPos.Count == 0) continue;
                    
                    //Instantiate the gameobject from the Data and set it's position
                    var go = blockDataForThisPos[0].gameObject;
                    var instance = Instantiate(go, transform);
                    instance.transform.position = new Vector3(x * LevelGen.BlockSize.x, 
                                                                0f, 
                                                                y * LevelGen.BlockSize.z);
                    
                    //Set the parent of the block to the plane
                    instance.transform.parent = transform;
                }
            }
        }
        
        public void DestroyLevel()
        {
            // Loop through all the children of the level
            for (int i = transform.childCount-1; i >= 0; i--)
            {
                // Destroy the child
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        
        
        
        private class Block
        {
            GameObject prefab {get;}
            Vector3 rotation {get;}
            
            public Block(GameObject prefab, Vector3 rotation)
            {
                this.prefab = prefab;
                this.rotation = rotation;
            }
        }
    }
}
