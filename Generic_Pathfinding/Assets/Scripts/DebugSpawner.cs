using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class simply puts a cube marker in the world space indicating
/// where the NON-walkable tiles are
/// </summary>
public class DebugSpawner : MonoBehaviour
{

    public List<GameObject> boardObjects;

    public void Awake()
    {
        boardObjects = new List<GameObject>();
    }

    public void SpawnMap(TileGrid grid)
    {
        Clear();
        for (int i = 0; i < grid.tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < grid.tileMap.GetLength(1); j++)
            {
                if (!grid.tileMap[i, j].walkable)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<Renderer>().material.color = Color.black;
                    cube.transform.localScale = new Vector3(1, 2, 1);
                    cube.transform.position = grid.GetWorldPoint(new Vector3(i, 0.5f, j));
                    cube.transform.SetParent(grid.transform);
                    boardObjects.Add(cube);
                }
            }
        }
    }

    private void Clear()
    {
        foreach (GameObject go in boardObjects)
        {
            GameObject.Destroy(go);
        }
    }
}
