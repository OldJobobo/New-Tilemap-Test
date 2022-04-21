using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

[System.Serializable]
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
            SetHealth(2);
        } 
        else if (name.Contains("Coal"))
        {
            SetHealth(4);
        }
        else if (name.Contains("Iron"))
        {
            SetHealth(6);
        }
        else if (name.Contains("Gold"))
        {
            SetHealth(8);
        }
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
