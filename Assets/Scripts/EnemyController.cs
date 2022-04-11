using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

public class EnemyController : MonoBehaviour
{

    public GameObject player;
    public GameObject gameObj;
    public STETilemap tilemap;
    private Vector2 playerGridPos;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = gameObj.GetComponent<GameManager>();
       
    }

    public void MoveEnenmy()
    {
        /*
         Vector3 playerPos = player.transform.position;
        Vector2 playerGridPos = TilemapUtils.GetGridPosition(tilemap, (playerPos));

        Vector2 targetPos = playerGridPos;

        Vector2 newPos = TilemapUtils.GetGridWorldPos(tilemap, ((int)targetPos.x ), (int)targetPos.y);

        */
        Vector2 newPos = FindSpawn(tilemap);
      Vector2 spawnPos = TilemapUtils.GetGridWorldPos(tilemap, (int)newPos.x, (int)newPos.y);

        print(spawnPos);
        transform.position = spawnPos;
   

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2 FindSpawn(STETilemap inMap)
    {
        bool isBlocking = true;

        Vector2 spawnPos = new Vector2(0, 0);
       
        while (isBlocking)
        {
            int randomX = gameManager.pseudoRandom.Next(2, gameManager.width);
            int randomY = gameManager.pseudoRandom.Next(2, gameManager.height);

            Vector2 testPos = new Vector2(randomX, randomY);

            if (inMap.GetTileData(testPos) == gameManager.bedrock || inMap.GetTileData(testPos) == gameManager.stoneTile || inMap.GetTileData(testPos) == gameManager.coalTile
                || inMap.GetTileData(testPos) == gameManager.ironTile || inMap.GetTileData(testPos) == gameManager.goldTile)
            {
                isBlocking = true;
            }
            else
            {
                isBlocking = false;
                spawnPos = testPos;
            }
        }
        return spawnPos;
    }

}
