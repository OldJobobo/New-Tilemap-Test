using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Tilemap groundMap;
    public Tile[] blockingTiles;
    public Tile dirtTile;
    public Text console;
    public GameObject roughStone;

    private bool isMoving;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.2f;
    new SpriteRenderer renderer;




    // Start is called before the first frame update
    void Start()
    {
        renderer = this.GetComponent<SpriteRenderer>();

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

        origPos = groundMap.transform.position;
        targetPos = origPos + direction;

        while (elapsedTime < timeToMove)
        {
            groundMap.transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        groundMap.transform.position = targetPos;

        isMoving = false;

    }

    bool CheckNeighbor(Vector3 direction)
    {
        bool isBlocking = false;
        Vector3 playerPos = transform.position;
        Vector3Int targetCell = groundMap.WorldToCell(playerPos + direction);
        foreach (Tile tile in blockingTiles)
        {
            if (tile == groundMap.GetTile(targetCell))
            { isBlocking = true; return isBlocking; }
            else
            { isBlocking = false; }
        }
        return isBlocking;
    }

    void DoTurn(Vector3 direction)
    {
        Vector3 playerPos = transform.position;
        Vector3Int targetCell = groundMap.WorldToCell(playerPos + direction);
        
        if (groundMap.GetTile(targetCell) == blockingTiles[0])
        {
            //console.text = console.text + "\n Stone.";
            groundMap.SetTile(targetCell, dirtTile);
            GameObject stoneObj = Instantiate(roughStone, groundMap.CellToWorld(targetCell), Quaternion.identity);
            stoneObj.transform.SetParent(groundMap.transform);
        }

    }
   
}
