using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] int cellSize;

    public MapTile[] tile;
    private Tile[,] grid;

    private Vector2Int[] neighbours = new Vector2Int[]
    {
        new Vector2Int(0,  1),
        new Vector2Int(0, -1),
        new Vector2Int(1,  0),
        new Vector2Int(-1, 0),
    };

    void Start()
    {
        grid = new Tile[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                grid[x, y] = new Tile();
                grid[x, y].AvaliableTiles.AddRange(tile);
            }
        }

        Vector2Int mid = new Vector2Int(mapSize.x / 2, mapSize.y / 2);

        grid[mid.x, mid.y].Updated = true;
        grid[mid.x, mid.y].SelectedTile = tile[UnityEngine.Random.Range(0,tile.Length)];

        CalculatePosibleTiles(mid);

        Spawn();

    }
    private void Spawn()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (grid[x, y] != null)
                {
                    Vector3 newPos = new Vector3(x * cellSize, 0, y * cellSize);

                    if (!grid[x, y].Spawned && grid[x, y].Updated)
                    {
                        var r = grid[x, y].SelectedTile;
                        Instantiate(r.Model, newPos, Quaternion.identity, transform);
                        grid[x, y].Spawned = true;
                    }
                }
            }
        }
    }
    private void CalculatePosibleTiles(Vector2Int mid)
    {
        if (grid[mid.x, mid.y].Spawned || !IsInsideGrid(mid)) return;

        for (int i = 0; i < neighbours.Length; i++)
        {
            Vector2Int offset = new Vector2Int(mid.x + neighbours[i].x, mid.y + neighbours[i].y);
            if (IsInsideGrid(offset))
            {
                if (!grid[offset.x, offset.y].Updated)
                {
                    CheckNeighbours(offset);
                    CalculatePosibleTiles(offset);
                }
            }
        }
    }
    private void CheckNeighbours(Vector2Int pos)
    {
        for (int i = 0; i < neighbours.Length; i++)
        {
            Vector2Int offset = new Vector2Int(pos.x + neighbours[i].x, pos.y + neighbours[i].y);
            MapTile[] availableTiles;
            if (IsInsideGrid(offset) && grid[offset.x, offset.y].Updated)
            {
                switch (i)
                {
                    case 0:
                        availableTiles = Array.FindAll(tile, x => x.down == grid[offset.x, offset.y].SelectedTile.up);
                        UpdateAvaliableTiles(availableTiles, pos);
                        break;

                    case 1:
                        availableTiles = Array.FindAll(tile, x => x.up == grid[offset.x, offset.y].SelectedTile.down);
                        UpdateAvaliableTiles(availableTiles, pos);
                        break;

                    case 2:
                        availableTiles = Array.FindAll(tile, x => x.left == grid[offset.x, offset.y].SelectedTile.right);
                        UpdateAvaliableTiles(availableTiles, pos);
                        break;

                    case 3:
                        availableTiles = Array.FindAll(tile, x => x.right == grid[offset.x, offset.y].SelectedTile.left);
                        UpdateAvaliableTiles(availableTiles, pos);
                        break;

                    default:
                        break;
                }
            }
        }
        if (grid[pos.x, pos.y].AvaliableTiles.Count == 0)
        {
            Debug.Log("NO TILES TO FIT IN");
            grid[pos.x, pos.y].SelectedTile = tile[0];
        }
        else grid[pos.x, pos.y].SelectedTile = grid[pos.x, pos.y].AvaliableTiles[UnityEngine.Random.Range(0, grid[pos.x, pos.y].AvaliableTiles.Count)];

        grid[pos.x, pos.y].Updated = true;
    }

    private bool IsInsideGrid(Vector2Int vec)
    {
        if (vec.x >= 0 && vec.x < mapSize.x && vec.y >= 0 && vec.y < mapSize.y) return true;
        return false;
    }
    private void UpdateAvaliableTiles(MapTile[] availableTiles, Vector2Int pos)
    {
        var commonElements = grid[pos.x, pos.y].AvaliableTiles.Intersect(availableTiles).ToList();
        grid[pos.x, pos.y].AvaliableTiles.Clear();
        grid[pos.x, pos.y].AvaliableTiles.AddRange(commonElements);
    }
}

public class Tile
{
    public bool Spawned;
    public bool Updated;
    public MapTile SelectedTile;
    public List<MapTile> AvaliableTiles = new List<MapTile>();
}
