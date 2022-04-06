using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CreativeSpore.SuperTilemapEditor;

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public Tilemap groundMap;
    public Tilemap swapMap;
    public UnityEngine.Tilemaps.Tile[] tiles;

    private CustomTile[,] tileData;
    public CustomTile customTile;
    public GameObject player;
    public string seed;
    public bool useRandomSeed;
    public STETilemap newTilemap;
    public STETilemap newSwapmap;



    [Range(0, 100)]
    public int stonePercentage;
    [Range(0, 100)]
    public int waterPercentage;
    [Range(0, 100)]
    public int grassPercentage;
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

    private int width = 200;
    private int height = 200;
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



    // Start is called before the first frame update
    void Start()
    {
        GenerateMapSeed();

        SwapTilemap(newSwapmap, newTilemap);

        for (int i = 0; i < 5; i++)
        {
            SmoothWalls();
        }

        for (int i = 0; i < 4; i++)
        {
            SmoothWater();
        }

        for (int i = 0; i < 3; i++)
        {
            SmoothGrass();
        }

        AddCoal(newSwapmap);
              
        for (int i = 0; i < coalSmoothing; i++)
        {
            SmoothCoal();
        }

        AddIron(newSwapmap);

        for (int i = 0; i < ironSmoothing; i++)
        {
            SmoothIron();
        }

        AddGold(newSwapmap);

        playerSpawn = FindPlayerSpawn(newTilemap);

        SpawnPlayer(playerSpawn);
    }

    void GenerateMapSeed()
    {
       


        if (useRandomSeed)
        { seed = Time.time.ToString(); }

        pseudoRandom = new System.Random(seed.GetHashCode());

        Vector3Int currentCell = Vector3Int.zero;

        currentCell.x = 0;
        currentCell.y = 0;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
               
                int rNum = 0;

                int rNumStone = pseudoRandom.Next(0, 100);
                if (rNumStone < stonePercentage)
                    rNum = 3;
                else
                    rNum = 0;

                if (currentCell.x == 0 || currentCell.x == width - 1 || currentCell.y == 0 || currentCell.y == height - 1)
                {
                    groundMap.SetTile(currentCell, tiles[3]);
                    newTilemap.SetTileData(x, y, stoneTile);
                }
                else if(rNum == 3)
                {
                    newTilemap.SetTileData(x, y, stoneTile);

                } else
                {
                    groundMap.SetTile(currentCell, tiles[rNum]);
                    newTilemap.SetTileData(x, y, dirtTile);
                }

                currentCell.x = x;
                currentCell.y = y;

            }

        }
    }

    void SmoothWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile , newSwapmap);
                int rNumWater = pseudoRandom.Next(0, 100);

                if (neighborWallTiles > 4)
                {
                    swapMap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                    newSwapmap.SetTileData(x, y, 63);
                }
                else if (neighborWallTiles < 4)
                {
                    if (rNumWater < waterPercentage)
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[2]);
                        newSwapmap.SetTileData(x, y, waterTile);
                    }
                    else
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                        newSwapmap.SetTileData(x, y, grassTile);
                    }
                    }
                    

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
    }

    void SmoothWater()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) != tiles[3])
                {
                    int neighborWaterTiles = GetSurroundingTileCount(x, y, waterTile, newSwapmap);
                    int rNumGrass = pseudoRandom.Next(0, 100);

                    if (neighborWaterTiles > 4)
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[2]);
                        newSwapmap.SetTileData(x, y, waterTile);
                    }
                    else if (neighborWaterTiles < 4)
                    {
                        if (rNumGrass < grassPercentage)
                        {
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                            newSwapmap.SetTileData(x, y, grassTile);
                        }
                        else
                        {
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                            newSwapmap.SetTileData(x, y, dirtTile);
                        }
                    }
                }

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
    }

    void SmoothGrass()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) != tiles[3] && swapMap.GetTile(new Vector3Int(x, y, 0)) != tiles[2])
                {
                    int neighborGrassTiles = GetSurroundingTileCount(x, y, grassTile, newTilemap);

                    if (neighborGrassTiles > 4)
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                        newSwapmap.SetTileData(x, y, grassTile);
                    }

                    else if (neighborGrassTiles < 4)
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                        newSwapmap.SetTileData(x, y, dirtTile);
                    }
                }

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
    }

    void SmoothCoal()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[3] || swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[4])
                {
                   
                        int neighborCoalTiles = GetSurroundingTileCount(x, y, coalTile, newTilemap);
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);

                    if (neighborCoalTiles > 3 && neighborCoalTiles < 5 && neighborWallTiles > 5)
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[4]);
                        else if (neighborCoalTiles < 3 || neighborCoalTiles >= 5)
                        {
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                        }
                   
                }

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
    }

    void SmoothIron()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[3] || swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[5] )
                {

                    int neighborIronTiles = GetSurroundingTileCount(x, y, ironTile, newTilemap);
                    int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);

                    if (neighborIronTiles > 3 && neighborIronTiles < 5 && neighborWallTiles > 5)
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[5]);
                    else if (neighborIronTiles < 3 || neighborIronTiles >= 5)
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                    }

                }

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
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
                        //if (map.GetTile(new Vector3Int(neighborX, neighborY, 0)) == tileType)
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

    void SwapTilemap(STETilemap origMap, STETilemap swapMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                //origMap.SetTile(new Vector3Int(x, y, 0), swapMap.GetTile(new Vector3Int(x, y, 0)));
                origMap.SetTileData(x, y, swapMap.GetTileData(x, y));
                //origMap.UpdateMesh();
                //swapMap.UpdateMesh();
            }
        }
    }

    void AddCoal(STETilemap tilemap)
    {
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

               if (tilemap.GetTileData(x, y) == stoneTile )
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < coalPercentage)
                            {
                                tilemap.SetTileData(x, y, coalTile);
                            }
                            else
                            {
                                tilemap.SetTileData(x, y, stoneTile);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
    }

    void AddIron(STETilemap tilemap)
    {


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (tilemap.GetTileData(x, y) == stoneTile)
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < ironPercentage)
                            {
                                tilemap.SetTileData(x, y, ironTile);

                            }
                            else
                            {
                                tilemap.SetTileData(x, y, stoneTile);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
    }

    void AddGold(STETilemap tilemap)
    {


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (tilemap.GetTileData(x, y) == stoneTile)
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < goldPercentage)
                            {
                                tilemap.SetTileData(x, y, goldTile);

                            }
                            else
                            {
                                tilemap.SetTileData(x, y, stoneTile);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(newTilemap, newSwapmap);
    }

    Vector2 FindPlayerSpawn(STETilemap tilemap)
    {
        bool isBlocking = true;
        
        Vector2 spawnPos = new Vector2(0, 0);

        while (isBlocking)
        {
            int randomX = Random.Range(2, 99);
            int randomY = Random.Range(2, 99);

            Vector2 testPos = new Vector2(randomX, randomY);

            //if (tilemap.GetTile(testPos) == tiles[3] || tilemap.GetTile(testPos) == tiles[4] || tilemap.GetTile(testPos) == tiles[5] || tilemap.GetTile(testPos) == tiles[6])
            if (tilemap.GetTileData(testPos) == stoneTile || tilemap.GetTileData(testPos) == coalTile 
                || tilemap.GetTileData(testPos) == ironTile || tilemap.GetTileData(testPos) == goldTile)
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

    void SpawnPlayer(Vector2 spawnPos)
    {

        Vector3 playerSpawnPoint = TilemapUtils.GetGridWorldPos(newTilemap, (int)spawnPos.x, (int)spawnPos.y);


         playerSpawnPoint = new Vector3(playerSpawnPoint.x + .5f, playerSpawnPoint.y + .5f, 0);
        player.transform.position = playerSpawnPoint;

        Debug.Log("Player spawned to: (" + spawnPos.x + ", " + spawnPos.y + ")");
        player.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = groundMap.WorldToCell(mousePos);
            UnityEngine.Tilemaps.Tile clickedTile = groundMap.GetTile<UnityEngine.Tilemaps.Tile>(gridPos);

           
            print(clickedTile.gameObject.GetComponent<CustomTile>().GetTileType()  + " : " + clickedTile.gameObject.GetComponent<CustomTile>().GetHealth() );
            
        }
    }


}
