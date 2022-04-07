using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

public class TileInfo : MonoBehaviour
{
    
    
    private int health = 0;



    // Start is called before the first frame update
    void Start()
    {
               
        string name = this.gameObject.name;
        //print(name);

        if (name == "StoneTile")
        {
            SetHealth(4);
        } 
        else if (name == "CoalTile")
        {
            SetHealth(6);
        }
        else if (name == "IronTile")
        {
            SetHealth(8);
        }
        else if (name == "GoldTile")
        {
            SetHealth(10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }

    public int GetHealth()
    {
        return health;
    }
}
