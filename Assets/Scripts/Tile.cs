using System;
using UnityEngine;

[Serializable]
public class Tile
{
    [SerializeField] private GameObject model;

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


