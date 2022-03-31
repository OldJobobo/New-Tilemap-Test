using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public Tilemap groundMap;
    public Tilemap swapMap;
    public Tile[] tiles;
    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int StonePercentage;
    [Range(0, 100)]
    public int WaterPercentage;
    [Range(0, 100)]
    public int GrassPercentage;
    [Range(0, 100)]
    public int CoalPercentage;
    [Range(0, 10)]
    public int coalSmoothing;

    private int width = 100;
    private int height = 100;
    private int previous;
    private System.Random pseudoRandom;



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

        AddOre(swapMap);
        for (int i = 0; i < coalSmoothing; i++)
        {
            SmoothCoal();
        }
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
                if (rNumStone < StonePercentage)
                    rNum = 3;
                else
                    rNum = 0;

                if (currentCell.x == 0 || currentCell.x == width - 1 || currentCell.y == 0 || currentCell.y == height - 1)
                {
                    groundMap.SetTile(currentCell, tiles[3]);
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
                    if (rNumWater < WaterPercentage)
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
                        if (rNumGrass < GrassPercentage)
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
                    if (x > 1 || x < width - 2 || y > 1 || y < height - 2)
                    {
                        int neighborOreTiles = GetSurroundingTileCount(x, y, tiles[4], swapMap);

                        if (neighborOreTiles > 4)
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[4]);
                        else if (neighborOreTiles < 4)
                        {
                            swapMap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                        }
                    }
                }

            }
        }
        SwapTilemap(groundMap, swapMap);
    }


    int GetSurroundingTileCount(int gridX, int gridY, Tile tileType, Tilemap map)
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

    void AddOre(Tilemap tilemap)
    {
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

               if (tilemap.GetTile(new Vector3Int(x, y, 0)) == tiles[3] )
                {
                    if (x > 1 || x < width - 2 || y > 1 || y < height - 2)
                    {
                        int rNumCoal = pseudoRandom.Next(0, 100);

                        if (rNumCoal < CoalPercentage)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
