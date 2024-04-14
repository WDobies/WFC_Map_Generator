using System;
using UnityEngine;

[Serializable]
public class Tile: MonoBehaviour
{
    [Header("Allowed connections")]
    public ConnectionType up;
    public ConnectionType down;
    public ConnectionType left;
    public ConnectionType right;

    public GameObject Model => this.gameObject;

}

public enum ConnectionType
{
    None,
    Grass,
    Road,
    Water,
}


