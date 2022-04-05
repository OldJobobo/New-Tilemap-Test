using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[CreateAssetMenu(fileName = "New CustomTile", menuName = "Custom Tile")]
public class CustomTile : Tile
{


    public Sprite m_Sprites;

    public Sprite m_Preview;
   
    public string tileType;
    public int health;

    public int GetHealth()
    {
        return health;
    }

    public string GetTileType()
    {
        return tileType;
    }
    


#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/CustomTile")]
    public static void CreateCustomTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Custom Tile", "New Custom Tile", "Asset", "Save Custom Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomTile>(), path);
    }
#endif

}
