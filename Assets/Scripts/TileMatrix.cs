using UnityEngine;

public class TileMatrix : MonoBehaviour
{
    public bool isCollapsed;
    public Tile tile;
    public Vector3 Position;
    public Quaternion Rotation = Quaternion.Euler(-90, 0, 0);
    public Vector2Int index = new Vector2Int();

    public void SetTile()
    {
        Debug.Log("HERE");

    }
    public void InstantiatePrefab()
    {
        Instantiate(tile.Model, Position, Quaternion.Euler(-90, 0, 0), transform);
    }
}
