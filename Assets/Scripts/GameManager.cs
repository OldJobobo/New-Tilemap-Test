using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CreativeSpore.SuperTilemapEditor;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public string seed;
    public bool useRandomSeed;
    public STETilemap newTilemap;
    public STETilemap swapMap;
    public STETilemap tmPrefab;
    public GameObject chuckGrid;

    [Range(0, 100)]
    public int stonePercentage;
    [Range(0, 10)]
    public int stoneSmoothing;
    [Range(0, 100)]
    public int waterPercentage;
    [Range(0, 10)]
    public int waterSmoothing;
    [Range(0, 100)]
    public int grassPercentage;
    [Range(0, 10)]
    public int grassSmoothing;
    [Range(0, 100)]
    public int coalPercentage;
    [Range(0, 10)]
    public int coalSmoothing;
    [Range(0, 100)]
    public int ironPercentage;
    [Range(0, 10)]
    public int ironSmoothing;
    [Range(0, 100)]
    public int goldPercentage;
    [Range(0, 10)]
    public int goldSmoothing;

    private int width = 180;
    private int height = 180;
    private int previous;
    private System.Random pseudoRandom;
    private Vector2 playerSpawn = new Vector2(0, 0);
   

    private uint dirtTile = 0;
    private uint stoneTile = 63;
    private uint waterTile = 2;
    private uint grassTile = 31;
    private uint coalTile = 64;
    private uint ironTile = 94;
    private uint goldTile = 93;
    private uint bedrock = 96;



    // Start is called before the first frame update
    void Start()
    {
        //SceneManager.LoadScene("Title Screen");

       StartNewGame();
    }

    void StartNewGame()
    {
        newTilemap = GenerateMapChunk(newTilemap);

        playerSpawn = FindPlayerSpawn(newTilemap);

        SpawnPlayer(newTilemap, playerSpawn);

        newTilemap.gameObject.SetActive(true);
    }


    STETilemap GenerateMapChunk(STETilemap inMap)
    {
        STETilemap chunk = GenerateMapSeed(inMap);

        SwapTilemap(swapMap, chunk);

        for (int i = 0; i < stoneSmoothing; i++)
        {
            SmoothStone(chunk);
        }

        for (int i = 0; i < waterSmoothing; i++)
        {
            SmoothWater(chunk);
        }

        for (int i = 0; i < grassSmoothing; i++)
        {
            SmoothGrass(chunk);
        }

        AddCoal(chunk);

        for (int i = 0; i < coalSmoothing; i++)
        {
            SmoothCoal(chunk);
        }

        AddIron(chunk);

        for (int i = 0; i < ironSmoothing; i++)
        {
            SmoothIron(chunk);
        }

        AddGold(chunk);

        for (int i = 0; i < goldSmoothing; i++)
        {
            SmoothGold(chunk);
        }
        swapMap.ClearMap();

        return chunk;
    }

    STETilemap GenerateMapSeed(STETilemap inMap)
    {
        
        inMap.gameObject.SetActive(false);

        
        //seed += Time.time.ToString() + Time.deltaTime.ToString(); 

        pseudoRandom = new System.Random(seed.GetHashCode());

        Vector3Int currentCell = Vector3Int.zero;

        currentCell.x = 0;
        currentCell.y = 0;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int bedrockAmt = 20;
                int rNum = 0;

                int rNumStone = pseudoRandom.Next(0, 100);
                if (rNumStone < stonePercentage)
                    rNum = 3;
                else
                    rNum = 0;

                if (currentCell.x <= (bedrockAmt + 2) || currentCell.x >= width - (bedrockAmt + 3)
                    || currentCell.y <= (bedrockAmt + 1) || currentCell.y >= height - (bedrockAmt + 4))
                {
                    inMap.SetTileData(x, y, bedrock);
                }
                else
                if (currentCell.x <= (bedrockAmt + 7) || currentCell.x >= width - (bedrockAmt + 8)
                    || currentCell.y <= (bedrockAmt + 6) || currentCell.y >= height - (bedrockAmt + 9))
                {
                    inMap.SetTileData(x, y, stoneTile);
                }
                else if (rNum == 3)
                {
                    inMap.SetTileData(x, y, stoneTile);
                }
                else
                {
                    inMap.SetTileData(x, y, dirtTile);
                }

                currentCell.x = x;
                currentCell.y = y;
            }

        } return inMap;
    }

    void SmoothStone(STETilemap inMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, inMap);
                int rNumWater = pseudoRandom.Next(0, 100);

                if (inMap.GetTileData(x, y) != bedrock)
                {
                    if (neighborWallTiles > 4)
                    {

                        swapMap.SetTileData(x, y, 63);
                    }
                    else if (neighborWallTiles < 4)
                    {
                        if (rNumWater < waterPercentage)
                        {

                            swapMap.SetTileData(x, y, waterTile);
                        }
                        else
                        {

                            swapMap.SetTileData(x, y, grassTile);
                        }
                    }
                }
            }
        }
        SwapTilemap(inMap, swapMap);
    }

    void SmoothWater(STETilemap inMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inMap.GetTileData(x, y) != stoneTile && inMap.GetTileData(x, y) != bedrock)
                {
                    int neighborWaterTiles = GetSurroundingTileCount(x, y, waterTile, inMap);
                    int rNumGrass = pseudoRandom.Next(0, 100);

                    if (neighborWaterTiles > 4)
                    {
                        
                        swapMap.SetTileData(x, y, waterTile);
                    }
                    else if (neighborWaterTiles < 4)
                    {
                        if (rNumGrass < grassPercentage)
                        {

                            swapMap.SetTileData(x, y, grassTile);
                        }
                        else
                        {

                            swapMap.SetTileData(x, y, dirtTile);
                        }
                    }
                }
            }
        }
        SwapTilemap(inMap, swapMap);
    }

    void SmoothGrass(STETilemap inMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inMap.GetTileData(x, y) != bedrock && inMap.GetTileData(x, y) != stoneTile && inMap.GetTileData(x, y) != waterTile)
                {
                    int neighborGrassTiles = GetSurroundingTileCount(x, y, grassTile, newTilemap);

                    if (neighborGrassTiles > 4)
                    {
                        swapMap.SetTileData(x, y, grassTile);
                    }

                    else if (neighborGrassTiles < 4)
                    {
                        swapMap.SetTileData(x, y, dirtTile);
                    }
                }

            }
        }
        SwapTilemap(inMap, swapMap);
    }

    void SmoothCoal(STETilemap inMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inMap.GetTileData(x, y) != bedrock && inMap.GetTileData(x, y) == stoneTile || inMap.GetTileData(x, y) == coalTile)
                {
                   
                        int neighborCoalTiles = GetSurroundingTileCount(x, y, coalTile, newTilemap);
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);

                    if (neighborCoalTiles > 3 && neighborCoalTiles < 5 && neighborWallTiles > 5)
                        swapMap.SetTileData(x, y, coalTile);
                        else if (neighborCoalTiles < 3 || neighborCoalTiles >= 5)
                        {
                            swapMap.SetTileData(x, y, stoneTile);
                        }
                }
            }
        }
        SwapTilemap(inMap, swapMap);
    }

    void SmoothIron(STETilemap inMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inMap.GetTileData(x, y) != bedrock && inMap.GetTileData(x, y) == stoneTile || inMap.GetTileData(x, y) == ironTile)
                {

                    int neighborIronTiles = GetSurroundingTileCount(x, y, ironTile, newTilemap);
                    int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);

                    if (neighborIronTiles > 3 && neighborIronTiles < 5 && neighborWallTiles > 5)
                        swapMap.SetTileData(x, y, ironTile);
                    else if (neighborIronTiles < 3 || neighborIronTiles >= 5)
                    {
                        swapMap.SetTileData(x, y, stoneTile);
                    }
                }
            }
        }
        SwapTilemap(inMap, swapMap);
    }
    void SmoothGold(STETilemap inMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inMap.GetTileData(x, y) != bedrock && inMap.GetTileData(x, y) == stoneTile || inMap.GetTileData(x, y) == goldTile)
                {

                    int neighborGoldTiles = GetSurroundingTileCount(x, y, goldTile, newTilemap);
                    int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);

                    if (neighborGoldTiles > 3 && neighborGoldTiles < 5 && neighborWallTiles > 4)
                        swapMap.SetTileData(x, y, goldTile);
                    else if (neighborGoldTiles < 3 || neighborGoldTiles >= 4)
                    {
                        swapMap.SetTileData(x, y, stoneTile);
                    }
                }
            }
        }
        SwapTilemap(inMap, swapMap);
    }

    int GetSurroundingTileCount(int gridX, int gridY, uint tileType, STETilemap map)
    {
        int count = 0;
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        
                        if(map.GetTileData(neighborX, neighborY) == tileType)
                        {
                            count++;
                        }
                    }
                }
                else
                {
                    count++;
                }
            }
        }
        return count;
    }

    void SwapTilemap(STETilemap outMap, STETilemap inMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                outMap.SetTileData(x, y, inMap.GetTileData(x, y));
            }
        }
    }

    void AddCoal(STETilemap inMap)
    {
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (inMap.GetTileData(x, y) != bedrock && inMap.GetTileData(x, y) == stoneTile )
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, inMap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < coalPercentage)
                            {
                                swapMap.SetTileData(x, y, coalTile);
                            }
                            else
                            {
                                swapMap.SetTileData(x, y, stoneTile);
                            }
                        }
                    }
                }
            }
        }
        SwapTilemap(inMap, swapMap);
    }

    void AddIron(STETilemap inMap)
    {


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (inMap.GetTileData(x, y) != bedrock && inMap.GetTileData(x, y) == stoneTile)
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, inMap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < ironPercentage)
                            {
                                swapMap.SetTileData(x, y, ironTile);

                            }
                            else
                            {
                                swapMap.SetTileData(x, y, stoneTile);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(inMap, swapMap);
    }

    void AddGold(STETilemap inMap)
    {


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (inMap.GetTileData(x, y) != bedrock && inMap.GetTileData(x, y) == stoneTile)
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, inMap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < goldPercentage)
                            {
                                swapMap.SetTileData(x, y, goldTile);

                            }
                            else
                            {
                                swapMap.SetTileData(x, y, stoneTile);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(inMap, swapMap);
    }

    Vector2 FindPlayerSpawn(STETilemap inMap)
    {
        bool isBlocking = true;
        
        Vector2 spawnPos = new Vector2(0, 0);

        while (isBlocking)
        {
            int randomX = Random.Range(2, width);
            int randomY = Random.Range(2, height);

            Vector2 testPos = new Vector2(randomX, randomY);

            if (inMap.GetTileData(testPos) == bedrock || inMap.GetTileData(testPos) == stoneTile || inMap.GetTileData(testPos) == coalTile 
                || inMap.GetTileData(testPos) == ironTile || inMap.GetTileData(testPos) == goldTile)
            {
                isBlocking = true;
            } else
            {
                isBlocking = false;
                spawnPos = testPos;
            }
        }
        return spawnPos;
    }

    void SpawnPlayer(STETilemap spawnMap, Vector2 spawnPos)
    {
        Vector3 playerSpawnPoint = TilemapUtils.GetTileCenterPosition(newTilemap, (int)(spawnPos.x ), (int)(spawnPos.y));

        playerSpawnPoint = new Vector3(playerSpawnPoint.x, playerSpawnPoint.y, 0);
        player.transform.position = playerSpawnPoint;
        //player.GetComponent<PlayerController>().playerGridPos = playerSpawnPoint;
       
        Debug.Log("Player spawned to: (" + spawnPos.x + ", " + spawnPos.y + ")");
        player.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 gridPos = new Vector2(TilemapUtils.GetMouseGridX(newTilemap, Camera.main), TilemapUtils.GetMouseGridY(newTilemap, Camera.main));

            uint clickedTile = newTilemap.GetTileData(gridPos);

            if (clickedTile == stoneTile || clickedTile == coalTile || clickedTile == ironTile || clickedTile == goldTile)
            {
                GameObject tileObject = newTilemap.GetTileObject((int)gridPos.x, (int)gridPos.y);
                int h = tileObject.GetComponent<TileInfo>().GetHealth();
               
                Vector2 pos = new Vector2(TilemapUtils.GetMouseGridX(newTilemap, Camera.main), TilemapUtils.GetMouseGridY(newTilemap, Camera.main));
                player.GetComponent<PlayerController>().DoMine(pos);
            }
          
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }


}
