using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using CreativeSpore.SuperTilemapEditor;

public class PlayerController : MonoBehaviour
{
    public Tilemap groundMap;
    public STETilemap newTilemap;
    //public Tile[] blockingTiles;
    public uint[] blockingTiles;
    public Vector2 playerGridPos;
    
    
    public Text console;
    public GameObject roughStone;

    private bool isMoving;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.2f;
    new SpriteRenderer renderer;

    private uint dirtTile = 0;
    private uint stoneTile = 63;
    //private uint waterTile = 2;
    //private uint grassTile = 31;
    private uint coalTile = 64;
    private uint ironTile = 94;
    private uint goldTile = 93;


    // Start is called before the first frame update
    void Start()
    {

        playerGridPos = TilemapUtils.GetGridPosition(newTilemap, (transform.position));

        print("playerGridPos = " + playerGridPos);

        renderer = this.GetComponent<SpriteRenderer>();

        blockingTiles[0] = stoneTile;
        blockingTiles[1] = coalTile;
        blockingTiles[2] = ironTile; 
        blockingTiles[3] = goldTile;
      
    }

    void FixedUpdate()
    {


        if (Input.GetKey(KeyCode.A) && !isMoving)
        {
            
            if (!CheckNeighbor(Vector3.left))
            {
                renderer.flipX = false;
                StartCoroutine(MovePlayer(Vector3.right));
            } else
            {
                DoTurn(Vector3.left);
            }
        }
        if (Input.GetKey(KeyCode.D) && !isMoving)
        {
            if (!CheckNeighbor(Vector3.right))
            {
                renderer.flipX = true;
                StartCoroutine(MovePlayer(Vector3.left));
            }
            else
            {
                DoTurn(Vector3.right);
            }
        }
        if (Input.GetKey(KeyCode.W) && !isMoving)
        {
            if (!CheckNeighbor(Vector3.up))
            { StartCoroutine(MovePlayer(Vector3.down)); }
            else
            {
                DoTurn(Vector3.up);
            }
        }
        
        if (Input.GetKey(KeyCode.S) && !isMoving)
        {
            if (!CheckNeighbor(Vector3.down))
            { StartCoroutine(MovePlayer(Vector3.up)); }
            else
            {
                DoTurn(Vector3.down);
            }
        }
        

    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime = 0;

        origPos = newTilemap.transform.position;
        targetPos = origPos + direction;

        while (elapsedTime < timeToMove)
        {
            newTilemap.transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        newTilemap.transform.position = targetPos;

        isMoving = false;

    }

    bool CheckNeighbor(Vector3 direction)
    {
        bool isBlocking = false;
        Vector3 playerPos = newTilemap.transform.InverseTransformPoint(transform.position);
        
        Vector2 targetCell = TilemapUtils.GetGridPosition(newTilemap, (playerPos + direction));

        uint testTile = newTilemap.GetTileData(targetCell);
        
        print(targetCell + " :: " + testTile);

        foreach (uint tile in blockingTiles)
        {

            if (tile == newTilemap.GetTileData(targetCell))
            { isBlocking = true; return isBlocking; }
            else
            { isBlocking = false; }
        }
        return isBlocking;
    }

    void DoTurn(Vector3 direction)
    {
        Vector3 playerPos = newTilemap.transform.InverseTransformPoint(transform.position);
        Vector2 targetCell = TilemapUtils.GetGridPosition(newTilemap, (playerPos + direction));

        if (newTilemap.GetTileData(targetCell) == stoneTile)
        {

             newTilemap.SetTileData(targetCell, dirtTile);
            newTilemap.UpdateMesh();
            Vector2 offset = new Vector2(-0.5f, -0.5f);
            Vector2 targetPos = newTilemap.transform.TransformPoint(TilemapUtils.GetTileCenterPosition(newTilemap, (int)targetCell.x, (int)targetCell.y)); 

             GameObject stoneObj = Instantiate(roughStone, targetPos + offset, Quaternion.identity);
             stoneObj.transform.SetParent(newTilemap.transform);
             console.text = console.text + "\nYou mined Stone.";

            

        }

    }
   
}
