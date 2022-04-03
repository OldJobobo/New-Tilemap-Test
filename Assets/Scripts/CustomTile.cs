using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="New CustomTile", menuName = "Custom Tile")]
public class CustomTile : TileBase
{
    public TileBase tile;
    public string type;
    public int health;
     

}
