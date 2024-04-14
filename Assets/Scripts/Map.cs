using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] int cellSize;

    public Tile[] tiles;
    private Dictionary<Vector2Int,GridTile> grid;

    private Vector2Int[] neighbours = new Vector2Int[]
    {
        new Vector2Int(0,  1),
        new Vector2Int(0, -1),
        new Vector2Int(1,  0),
        new Vector2Int(-1, 0),
    };

    public void GenerateMap()
    {
        DestroyMap();

        grid = new Dictionary<Vector2Int, GridTile>();

        Vector2Int mid = new Vector2Int(mapSize.x / 2, mapSize.y / 2);
        GridTile midTile = new GridTile();
        midTile.SelectedTile = tiles[UnityEngine.Random.Range(0, tiles.Length)];
        midTile.Updated = true;
        grid[mid] = midTile;

        CalculatePosibleTiles(mid);
        Spawn();
    }

    public void DestroyMap()
    {
        if (transform.childCount != 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
                else
#endif
                    Destroy(transform.GetChild(i).gameObject); ;
            }
        }
    }

    private void Spawn()
    {
        foreach (var gridTile in grid)
        {
            Vector3 newPos = new Vector3(gridTile.Key.x * cellSize, 0, gridTile.Key.y * cellSize);
            if (!gridTile.Value.Spawned && gridTile.Value.Updated)
            {
                gridTile.Value.Instantiate(newPos,transform, Quaternion.identity);
            }
        }
    }

    private void CalculatePosibleTiles(Vector2Int mid)
    {
        if (grid[mid].Spawned || !IsInsideGrid(mid)) return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(mid);

        while (queue.Count > 0)
        {
            Vector2Int currentTile = queue.Dequeue();

            foreach (var neighbourOffset in neighbours)
            {
                Vector2Int offset = currentTile + neighbourOffset;
                if (!grid.ContainsKey(offset) && IsInsideGrid(offset))
                {
                    GridTile newTile = new GridTile();
                    newTile.AvaliableTiles.AddRange(tiles);
                    grid[offset] = newTile;
                    CheckNeighbours(offset);
                    queue.Enqueue(offset);
                }
            }
        }
    }
    private void CheckNeighbours(Vector2Int pos)
    {
        foreach (var neighbourOffset in neighbours)
        {
            Vector2Int offset = pos + neighbourOffset;

            if (grid.TryGetValue(offset, out GridTile neighbourTile) && neighbourTile.Updated)
            {
                switch (neighbourOffset)
                {
                    case Vector2Int up when up == Vector2Int.up:
                        UpdateAvailableTiles(tiles.Where(x => x.down.Any(c => c == neighbourTile.SelectedTile.up[0])).ToArray(), pos);
                        break;

                    case Vector2Int down when down == Vector2Int.down:
                        UpdateAvailableTiles(tiles.Where(x => x.up.Any(c => c == neighbourTile.SelectedTile.down[0])).ToArray(), pos);
                        break;

                    case Vector2Int right when right == Vector2Int.right:
                        UpdateAvailableTiles(tiles.Where(x => x.left.Any(c => c == neighbourTile.SelectedTile.right[0])).ToArray(), pos);
                        break;

                    case Vector2Int left when left == Vector2Int.left:
                        UpdateAvailableTiles(tiles.Where(x => x.right.Any(c => c == neighbourTile.SelectedTile.left[0])).ToArray(), pos);
                        break;
                }
            }
        }
        if (grid[pos].AvaliableTiles.Count == 0)
        {
            Debug.Log("NO TILES TO FIT IN ON POSITION: "+ pos);
            grid[pos].SelectedTile = tiles[0];
        }
        else grid[pos].SelectedTile = grid[pos].AvaliableTiles[UnityEngine.Random.Range(0, grid[pos].AvaliableTiles.Count)];

        grid[pos].Updated = true;
    }

    private bool IsInsideGrid(Vector2Int vec)
    {
        if (vec.x >= 0 && vec.x < mapSize.x && vec.y >= 0 && vec.y < mapSize.y) return true;
        return false;
    }
    private void UpdateAvailableTiles(Tile[] availableTiles, Vector2Int pos)
    {
        var commonElements = grid[pos].AvaliableTiles.Intersect(availableTiles).ToList();
        grid[pos].AvaliableTiles.Clear();
        grid[pos].AvaliableTiles.AddRange(commonElements);
    }
}

public class GridTile
{
    public bool Spawned;
    public bool Updated;
    public Tile SelectedTile;
    public List<Tile> AvaliableTiles = new List<Tile>();

    public void Instantiate(Vector3 position, Transform parent, Quaternion rotation)
    {
        GameObject.Instantiate(SelectedTile.Model, position, Quaternion.identity, parent);
        Spawned = true;
    }
}
