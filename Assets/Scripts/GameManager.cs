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
    public CreativeSpore.SuperTilemapEditor.Tile[] steTiles;
    private CustomTile[,] tileData;
    public CustomTile customTile;
    public GameObject player;
    public string seed;
    public bool useRandomSeed;
    public STETilemap newTilemap;

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

    private int width = 100;
    private int height = 100;
    private int previous;
    private System.Random pseudoRandom;
    private Vector3Int playerSpawn = new Vector3Int(0, 0, 0);



    // Start is called before the first frame update
    void Start()
    {
        GenerateMapSeed();

        SwapTilemap(swapMap, groundMap);

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

        AddCoal(swapMap);
              
        for (int i = 0; i < coalSmoothing; i++)
        {
            SmoothCoal();
        }

        AddIron(swapMap);

        for (int i = 0; i < ironSmoothing; i++)
        {
            SmoothIron();
        }

        AddGold(swapMap);

        playerSpawn = FindPlayerSpawn(groundMap);

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
                   // groundMap.GetInstantiatedObject
                    
                    
                   
                   
                }
                else
                {
                    groundMap.SetTile(currentCell, tiles[rNum]);
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
                int neighborWallTiles = GetSurroundingTileCount(x, y, tiles[3], groundMap);
                int rNumWater = pseudoRandom.Next(0, 100);

                if (neighborWallTiles > 4)
                    swapMap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                else if (neighborWallTiles < 4)
                {
                    if (rNumWater < waterPercentage)
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[2]);
                    else
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                }
                    

            }
        }
        SwapTilemap(groundMap, swapMap);
    }

    void SmoothWater()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) != tiles[3])
                {
                    int neighborWaterTiles = GetSurroundingTileCount(x, y, tiles[2], swapMap);
                    int rNumGrass = pseudoRandom.Next(0, 100);

                    if (neighborWaterTiles > 4)
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[2]);
                    else if (neighborWaterTiles < 4)
                    {
                        if (rNumGrass < grassPercentage)
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                        else
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                    }
                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }

    void SmoothGrass()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) != tiles[3] && swapMap.GetTile(new Vector3Int(x, y, 0)) != tiles[2])
                {
                    int neighborGrassTiles = GetSurroundingTileCount(x, y, tiles[1], swapMap);
                   
                    if (neighborGrassTiles > 4)
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                    else if (neighborGrassTiles < 4)
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                    }
                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }

    void SmoothCoal()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[3] || swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[4])
                {
                   
                        int neighborOreTiles = GetSurroundingTileCount(x, y, tiles[4], swapMap);
                        int neighborWallTiles = GetSurroundingTileCount(x, y, tiles[3], groundMap);

                    if (neighborOreTiles > 3 && neighborOreTiles < 5 && neighborWallTiles > 5)
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[4]);
                        else if (neighborOreTiles < 3 || neighborOreTiles >= 5)
                        {
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                        }
                   
                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }

    void SmoothIron()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[3] || swapMap.GetTile(new Vector3Int(x, y, 0)) == tiles[5] )
                {

                    int neighborIronTiles = GetSurroundingTileCount(x, y, tiles[5], swapMap);
                    int neighborWallTiles = GetSurroundingTileCount(x, y, tiles[3], groundMap);

                    if (neighborIronTiles > 3 && neighborIronTiles < 5 && neighborWallTiles > 5)
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[5]);
                    else if (neighborIronTiles < 3 || neighborIronTiles >= 5)
                    {
                        swapMap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                    }

                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }


    int GetSurroundingTileCount(int gridX, int gridY, UnityEngine.Tilemaps.Tile tileType, Tilemap map)
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
                        if (map.GetTile(new Vector3Int(neighborX, neighborY, 0)) == tileType)
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

    void SwapTilemap(Tilemap origMap, Tilemap swapMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                origMap.SetTile(new Vector3Int(x, y, 0), swapMap.GetTile(new Vector3Int(x, y, 0)));

            }
        }
    }

    void AddCoal(Tilemap tilemap)
    {
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

               if (tilemap.GetTile(new Vector3Int(x, y, 0)) == tiles[3] )
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, tiles[3], groundMap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < coalPercentage)
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[4]);

                            }
                            else
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }

    void AddIron(Tilemap tilemap)
    {


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == tiles[3])
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, tiles[3], groundMap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < ironPercentage)
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[5]);

                            }
                            else
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }

    void AddGold(Tilemap tilemap)
    {


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == tiles[3])
                {
                    if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                    {
                        int neighborWallTiles = GetSurroundingTileCount(x, y, tiles[3], groundMap);
                        if (neighborWallTiles > 5)
                        {
                            int rNumCoal = pseudoRandom.Next(0, 100);

                            if (rNumCoal < goldPercentage)
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[6]);

                            }
                            else
                            {
                                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                            }
                        }
                    }
                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }

    Vector3Int FindPlayerSpawn(Tilemap tilemap)
    {
        bool isBlocking = true;
        
        Vector3Int spawnPos = new Vector3Int(0, 0, 0);

        while (isBlocking)
        {
            int randomX = Random.Range(2, 99);
            int randomY = Random.Range(2, 99);

            Vector3Int testPos = new Vector3Int(randomX, randomY, 0);

            if (tilemap.GetTile(testPos) == tiles[3] || tilemap.GetTile(testPos) == tiles[4] || tilemap.GetTile(testPos) == tiles[5] || tilemap.GetTile(testPos) == tiles[6])
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

    void SpawnPlayer(Vector3Int spawnPos)
    {

        Vector3 playerSpawnPoint = groundMap.CellToWorld(spawnPos);
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
