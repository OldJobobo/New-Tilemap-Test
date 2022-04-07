using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CreativeSpore.SuperTilemapEditor;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public string seed;
    public bool useRandomSeed;
    public STETilemap newTilemap;
    public STETilemap newSwapmap;
    public STETilemap tmPrefab;
    public STETilemap[,] chunks = new STETilemap[3, 3];
    public GameObject chuckGrid;

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

    private int width = 60;
    private int height = 60;
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

        
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tmPrefab = GenerateMapChunk(tmPrefab);
                chunks[x, y] = Instantiate(newTilemap, new Vector2(0, 0), Quaternion.identity);
                chunks[x, y].name = "chunk" + x.ToString() + y.ToString();
                chunks[x, y].transform.parent = chuckGrid.transform;
                    
            }
        }
        chunks[0, 0].transform.position += new Vector3(-60, 60, 0);
        chunks[0, 1].transform.position += new Vector3(-60, 0, 0);
        chunks[0, 2].transform.position += new Vector3(-60, -60, 0);
        chunks[1, 0].transform.position += new Vector3(0, 60, 0);
        chunks[1, 1].transform.position += new Vector3(0, 0, 0);
        chunks[1, 2].transform.position += new Vector3(0, -60, 0);
        chunks[2, 0].transform.position += new Vector3(60, 60, 0);
        chunks[2, 1].transform.position += new Vector3(60, 0, 0);
        chunks[2, 2].transform.position += new Vector3(60, -60, 0);
        //newTilemap = GenerateMapChunk();
        playerSpawn = FindPlayerSpawn(chunks[1,1]);

        SpawnPlayer(playerSpawn);
    }

    STETilemap GenerateMapChunk(STETilemap inMap)
    {
       

        GenerateMapSeed();

        SwapTilemap(inMap, newTilemap);

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

        newTilemap.gameObject.SetActive(true);

        newSwapmap.ClearMap();

        return newTilemap;
    }

    void GenerateMapSeed()
    {

        newTilemap.gameObject.SetActive(false);

        
         seed += Time.time.ToString() + Time.deltaTime.ToString(); 

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

                if (currentCell.x <= 1 || currentCell.x >= width - 2 || currentCell.y <= 1 || currentCell.y >= height - 3)
                {
                    
                    newTilemap.SetTileData(x, y, stoneTile);
                }
                else if(rNum == 3)
                {
                    newTilemap.SetTileData(x, y, stoneTile);

                } else
                {
                   
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
                int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile , newTilemap);
                int rNumWater = pseudoRandom.Next(0, 100);

                if (neighborWallTiles > 4)
                {
                   
                    newSwapmap.SetTileData(x, y, 63);
                }
                else if (neighborWallTiles < 4)
                {
                    if (rNumWater < waterPercentage)
                    {
                        
                        newSwapmap.SetTileData(x, y, waterTile);
                    }
                    else
                    {
                        
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
                if (newSwapmap.GetTileData(x, y) != stoneTile)
                {
                    int neighborWaterTiles = GetSurroundingTileCount(x, y, waterTile, newTilemap);
                    int rNumGrass = pseudoRandom.Next(0, 100);

                    if (neighborWaterTiles > 4)
                    {
                        
                        newSwapmap.SetTileData(x, y, waterTile);
                    }
                    else if (neighborWaterTiles < 4)
                    {
                        if (rNumGrass < grassPercentage)
                        {
                            
                            newSwapmap.SetTileData(x, y, grassTile);
                        }
                        else
                        {
                           
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
                if (newSwapmap.GetTileData(x, y) != stoneTile && newSwapmap.GetTileData(x, y) != waterTile)
                {
                    int neighborGrassTiles = GetSurroundingTileCount(x, y, grassTile, newTilemap);

                    if (neighborGrassTiles > 4)
                    {
                        newSwapmap.SetTileData(x, y, grassTile);
                    }

                    else if (neighborGrassTiles < 4)
                    {
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
                if (newSwapmap.GetTileData(x, y) == stoneTile || newSwapmap.GetTileData(x, y) == coalTile)
                {
                   
                        int neighborCoalTiles = GetSurroundingTileCount(x, y, coalTile, newTilemap);
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);

                    if (neighborCoalTiles > 3 && neighborCoalTiles < 5 && neighborWallTiles > 5)
                        newSwapmap.SetTileData(x, y, coalTile);
                        else if (neighborCoalTiles < 3 || neighborCoalTiles >= 5)
                        {
                            newSwapmap.SetTileData(x, y, stoneTile);
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
                if (newSwapmap.GetTileData(x, y) == stoneTile || newSwapmap.GetTileData(x, y) == ironTile)
                {

                    int neighborIronTiles = GetSurroundingTileCount(x, y, ironTile, newTilemap);
                    int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, newTilemap);

                    if (neighborIronTiles > 3 && neighborIronTiles < 5 && neighborWallTiles > 5)
                        newSwapmap.SetTileData(x, y, ironTile);
                    else if (neighborIronTiles < 3 || neighborIronTiles >= 5)
                    {
                        newSwapmap.SetTileData(x, y, stoneTile);
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
                origMap.SetTileData(x, y, swapMap.GetTileData(x, y));
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
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, tilemap);
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
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, tilemap);
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
                        int neighborWallTiles = GetSurroundingTileCount(x, y, stoneTile, tilemap);
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

        Vector3 playerSpawnPoint = TilemapUtils.GetTileCenterPosition(newTilemap, (int)(spawnPos.x ), (int)(spawnPos.y));


         playerSpawnPoint = new Vector3(playerSpawnPoint.x, playerSpawnPoint.y, 0);
        player.transform.position = playerSpawnPoint;
       player.GetComponent<PlayerController>().playerGridPos = playerSpawnPoint;
        //print(playerSpawnPoint);

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

                print("You clicked  " + gridPos + "\nTile Health = " + h);

                Vector2 pos = new Vector2(TilemapUtils.GetMouseGridX(newTilemap, Camera.main), TilemapUtils.GetMouseGridY(newTilemap, Camera.main));
                player.GetComponent<PlayerController>().DoMine(pos);
            }
          
        }
    }


}
