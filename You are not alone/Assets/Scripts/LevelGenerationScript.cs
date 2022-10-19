using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.Controls;

public class LevelGenerationScript : MonoBehaviour
{
    public int seed;
    public Vector2 mapSize;
    public Vector3 tileSize;

    public void GenerateLevel()
    {
        for (float x = 0; x < mapSize.x; x++)
        {
            for (float y = 0; y < mapSize.y; y++)
            {
                // Create a new tile
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // Set the position of the tile
                tile.transform.position = new Vector3(x * tileSize.x, 0, y * tileSize.z);
                // Set it as a child of the level
                tile.transform.parent = transform;
                // Set the name of the tile
                tile.name = "Tile " + x + ", " + y;
                // Set the scale of the tile
                tile.transform.localScale = tileSize*0.9f;
                //set the tile to static
                tile.isStatic = true;
            }
        }
        
        //Bake the lighting
        //Lightmapping.Bake();
    } 
    
    public void ClearLevel()
    {
        // Loop through all the children of the level
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            // Destroy the child
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Gizmos.DrawWireCube(new Vector3(x * tileSize.x, 0, y * tileSize.z), tileSize);
            }
        }
    }
}
