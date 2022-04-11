using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

public class FogOfWar : MonoBehaviour
{

    public GameObject player;
    public GameObject gameObj;
    private GameManager gameManager;
    public STETilemap fogTilemap;
    public STETilemap tilemap;
    public Vector2 playerGridPos;
    public int[][] xyoffsets = new int[37][]{
                                                      new int[2] { -1, -3 }, new int[2] { 0, -3 }, new int[2] { 1, -3 },
                               new int[2] { -2, -2 }, new int[2] { -1, -2 }, new int[2] { 0, -2 }, new int[2] { 1, -2 }, new int[2] { 2,  2 },
        new int[2] { -3, -1 }, new int[2] { -2, -1 }, new int[2] { -1, -1 }, new int[2] { 0, -1 }, new int[2] { 1, -1 }, new int[2] { 2,  1 }, new int[2] { 3,  1 },
        new int[2] { -3,  0 }, new int[2] { -2,  0 }, new int[2] { -1,  0 }, new int[2] { 1,  0 }, new int[2] {-1,  1 }, new int[2] { 2,  0 }, new int[2] { 3,  0 },
        new int[2] { -3,  1 }, new int[2] { -2,  1 }, new int[2] {  0,  1 }, new int[2] { 1,  1 }, new int[2] { 0,  0 }, new int[2] { 2, -1 }, new int[2] { 3, -1 },
                               new int[2] { -2,  2 }, new int[2] { -1,  2 }, new int[2] { 0,  2 }, new int[2] { 1,  2 }, new int[2] { 2, -2 },
                                                      new int[2] { -1,  3 }, new int[2] { 0,  3 }, new int[2] { 1,  3 }};

    // Start is called before the first frame update
    void Awake()
    {

        Vector3 playerPos = player.transform.position;
        playerGridPos = TilemapUtils.GetGridPosition(tilemap, (playerPos));


        gameManager = gameObj.GetComponent<GameManager>();

        


        for (int x = 0; x < gameManager.width; x++)
        {
            for (int y = 0; y < gameManager.height; y++)
            {
                Vector2 gridPos = new Vector2(x, y);
                Vector2 diff = (playerGridPos - gridPos);

                if (gridPos != playerGridPos)
                {

                    fogTilemap.SetTileData(x, y, 0);

                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 playerPos = player.transform.position;
        playerGridPos = TilemapUtils.GetGridPosition(tilemap, (playerPos));
        /*
        for (float i =  - 2; i <= playerGridPos.x + 2; i++)
        {
            for (float j = playerGridPos.y - 2; j <= playerGridPos.y + 2; j++)
            {
                if(i != playerGridPos.x + 2 && j != playerGridPos.y + 2)
                    fogTilemap.SetTileData((int)i, (int)j, 1);
            }
        }
        */
        foreach (int[] xyoffset in xyoffsets)
        {
            int i = (int)playerGridPos.x + xyoffset[0];
            int j = (int)playerGridPos.y + xyoffset[1];
            // process tile (i,j)
            fogTilemap.SetTileData(i, j, 1);
        }



    }
}
