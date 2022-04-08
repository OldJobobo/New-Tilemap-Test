using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

public class TileInfo : MonoBehaviour
{

    [SerializeField]
    private SimpleFlash flashEffect;

    private int health = 0;



    // Start is called before the first frame update
    void Awake()
    {
               
        string name = this.gameObject.name;
        


        if (name.Contains("Stone"))
        {
            SetHealth(4);
        } 
        else if (name.Contains("Coal"))
        {
            SetHealth(6);
        }
        else if (name.Contains("Iron"))
        {
            SetHealth(8);
        }
        else if (name.Contains("Gold"))
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

    public void Flash()
    {
        flashEffect.Flash();
    }
}
