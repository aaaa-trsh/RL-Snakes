using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public Vector2 GridSize;
    Vector2 prevSize;
    public GameObject TilePrefab;
    public bool Gen;

    public List<List<SnakeTile>> tiles = new List<List<SnakeTile>>();

    void Awake()
    {
        if (Gen)
            Generate();
    }

    void Generate()
    {
        tiles.Clear();

        foreach (Transform t in GetComponentsInChildren<Transform>()) 
        {
            if (t != transform)
                Destroy(t.gameObject);
        }

        for (int x = 0; x < Mathf.FloorToInt(GridSize.x); x++)
        {
            List<SnakeTile> row = new List<SnakeTile>();

            for (int y = 0; y < Mathf.FloorToInt(GridSize.y); y++)
            {
                GameObject newTile = Instantiate(TilePrefab, new Vector2(x, y), Quaternion.identity, transform);
                row.Add(newTile.GetComponent<SnakeTile>());
            }

            tiles.Add(row);
        }
    }
}
