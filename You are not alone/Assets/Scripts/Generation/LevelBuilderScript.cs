using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Generation
{
    public class LevelBuilderScript : MonoBehaviour
    {
        [SerializeField] private LevelGenerationScript LevelGen;
        [SerializeField] private Material FloorMaterial;
        [SerializeField] private Material WallMaterial;
        [SerializeField] private GameObject[] prefabs = {};
        [SerializeField] private BlockGenData[] wall_block_rules = {};
        private GameObject walls_parent;
        
        public void BuildLevel()
        {
            CreateParentHolders();
            BuildWalls();
            BuildCeiling();
            GenerateNavMesh();
        }

        private void GenerateNavMesh()
        {
            //Add a navmesh surface to the floor and bake it
            var Floor = transform.Find("Floor").gameObject;
            var navMeshSurface = Floor.AddComponent<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
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

        private void CreateParentHolders()
        {
            //Delete the floor plane if it exists
            if (transform.Find("Floor"))
                DestroyImmediate(transform.Find("Floor").gameObject);

            //Create the floor plane
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Floor";
            plane.transform.localScale = new Vector3(LevelGen.Grid.Size.x/1.5f, 
                1f, 
                LevelGen.Grid.Size.y/1.5f);
            plane.transform.position = new Vector3((LevelGen.Grid.Size.x * LevelGen.BlockSize.x)/2f, 
                -LevelGen.BlockSize.y/2f,
                (LevelGen.Grid.Size.y * LevelGen.BlockSize.z)/2f);
            plane.transform.parent = transform;
            plane.isStatic = true;
            plane.GetComponent<MeshRenderer>().material = FloorMaterial;
            
            //Delete the walls parent if it exists
            if (transform.Find("Walls"))
                DestroyImmediate(transform.Find("Walls").gameObject);
            
            //Create the walls parent
            walls_parent = new GameObject("Walls");
            walls_parent.transform.parent = transform;
            walls_parent.isStatic = true;
        }

        private void BuildWalls()
        {
            //Loop through all the cells in the grid
            for (int x = 0; x < LevelGen.Grid.Size.x; x++)
            {
                for (int y = 0; y < LevelGen.Grid.Size.y; y++)
                {
                    CellState cell = LevelGen.Grid[x, y];
                        
                    if (EnumHelpers.CellIsOccupied(cell)) continue;
                    
                    var wallCanBePlaced = false;
                    foreach (var wallBlockRule in wall_block_rules)
                    {
                        if (wallBlockRule.NeighbourCheck(LevelGen.Grid, new Vector2Int(x, y), out var validDirections))
                        {
                            wallCanBePlaced = true;
                            break;
                        }
                    }
                    
                    if (!wallCanBePlaced) continue;
                    
                    //Place a block sized cube at the position
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.localScale = LevelGen.BlockSize;
                    wall.transform.position = new Vector3(x * LevelGen.BlockSize.x, 
                        0f, 
                        y * LevelGen.BlockSize.z);
                    wall.transform.parent = walls_parent.transform;
                    wall.isStatic = true;
                    wall.GetComponent<MeshRenderer>().material = WallMaterial;
                }
            }
        }
        
        private void BuildCeiling()
        {
            //Delete the floor plane if it exists
            if (transform.Find("Ceiling"))
                DestroyImmediate(transform.Find("Ceiling").gameObject);

            //Create the ceiling plane
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Ceiling";
            plane.transform.localScale = new Vector3(LevelGen.Grid.Size.x/1.5f, 
                1f, 
                LevelGen.Grid.Size.y/1.5f);
            plane.transform.position = new Vector3((LevelGen.Grid.Size.x * LevelGen.BlockSize.x)/2f, 
                LevelGen.BlockSize.y/2f,
                (LevelGen.Grid.Size.y * LevelGen.BlockSize.z)/2f);
            plane.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
            plane.transform.parent = transform;
            plane.isStatic = true;
            
            //Set the shadows to be 2 sided and be a static shadow caster
            plane.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            plane.GetComponent<MeshRenderer>().receiveShadows = false;
            
            //Duplicate the ceiling plane and move it up by 20 units
            GameObject ceiling = Instantiate(plane, plane.transform.position + new Vector3(0f, 20f, 0f), plane.transform.rotation);
            ceiling.name = "Higher Ceiling";
            ceiling.transform.parent = transform;
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
