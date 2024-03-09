using System;
using UnityEngine;

[Serializable]
public class MapTile
{
    [SerializeField] GameObject model;
    public TileType tileType;
    [Header("Allowed connections")]
    public ConnectionType up;
    public ConnectionType down;
    public ConnectionType left;
    public ConnectionType right;

    public GameObject Model => model;

}

public enum ConnectionType
{
    None,
    Grass,
    Road,
    Water,
}

public enum TileType
{
    None,
    Grass,
    GrassRoadUR,
    GrassRoadUL,
    GrassRoadUD,
    GrassRoadDR,
    GrassRoadDL,
}
