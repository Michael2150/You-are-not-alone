using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelGenerationScript : MonoBehaviour
{
    public int seed;
    public Vector2 mapSize;
    public Vector3 tileSize;
    public List<GameObject> straightTiles;
    public List<GameObject> cornerTiles;
    //2d array of tiles
    public GameObject[,] tiles;

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            GenerateLevel();
        } else if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            ClearLevel();
        }
    }

    public void GenerateLevel()
    {
        
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
        Gizmos.color = new Color(1, 1, 1, 0.05f);
        for (var x = 0; x < mapSize.x; x++)
        {
            for (var y = 0; y < mapSize.y; y++)
            {
                Gizmos.DrawWireCube(new Vector3(x * tileSize.x, 0, y * tileSize.z), tileSize);
            }
        }
    }
}
