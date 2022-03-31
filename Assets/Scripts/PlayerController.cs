using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Tilemap groundMap;
    public Tile wallTile;
    //public Tile[] blockingTiles;

    private bool isMoving;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.2f;


   

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (Input.GetKey(KeyCode.A) && !isMoving)
        {
            if (!CheckNeighbor(Vector3.left))
                StartCoroutine(MovePlayer(Vector3.right));
        }
        if (Input.GetKey(KeyCode.D) && !isMoving)
        {
            if (!CheckNeighbor(Vector3.right))
                StartCoroutine(MovePlayer(Vector3.left));
        }
        if (Input.GetKey(KeyCode.W) && !isMoving)
        {
            if (!CheckNeighbor(Vector3.up))
                StartCoroutine(MovePlayer(Vector3.down));
        }
        if (Input.GetKey(KeyCode.S) && !isMoving)
        {
            if (!CheckNeighbor(Vector3.down))
            StartCoroutine(MovePlayer(Vector3.up));
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
        Vector3 playerPos = transform.position;
        Vector3Int targetCell = groundMap.WorldToCell(playerPos + direction);

        if (groundMap.GetTile(targetCell) == wallTile) 
            return true;
        else
            return false;
    }
}
