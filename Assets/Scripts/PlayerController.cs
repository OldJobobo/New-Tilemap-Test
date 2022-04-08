using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using CreativeSpore.SuperTilemapEditor;

public class PlayerController : MonoBehaviour
{
    
    public STETilemap newTilemap;
    public GameObject chuckGrid;
   
    public uint[] blockingTiles;
    public Vector2 playerGridPos;
    
    
    public Text console;
    public Text stoneAmt;
    public Text coalAmt;
    public Text ironAmt;
    public Text goldAmt;

    public GameObject roughStone;
    public GameObject coal;
    public GameObject ironOre;
    public GameObject goldCoins;

    public AudioSource audioSource;
    public AudioClip footStep;
    public AudioClip miningChip;
    public AudioClip pickUp;


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
    private uint bedrock = 96;



    // Start is called before the first frame update
    void Start()
    {

        playerGridPos = TilemapUtils.GetGridPosition(newTilemap, (transform.position));

        renderer = this.GetComponent<SpriteRenderer>();

        blockingTiles[0] = stoneTile;
        blockingTiles[1] = coalTile;
        blockingTiles[2] = ironTile; 
        blockingTiles[3] = goldTile;
        blockingTiles[4] = bedrock;
      
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

    public void DoMine(Vector2 targetPos)
    {
        Vector3 playerPos = newTilemap.transform.InverseTransformPoint(transform.position);
        Vector2 playerGridPos = TilemapUtils.GetGridPosition(newTilemap, (playerPos));

        Vector2 targetCell = TilemapUtils.GetGridPosition(newTilemap, (targetPos));
        uint targetTile = newTilemap.GetTileData(targetCell);

        Vector2 diff = (playerGridPos - targetCell);
        print(diff);

        if ((diff.x == 1.0 || diff.x == -1.0 || diff.x == 0.0) && (diff.y == 1.0 || diff.y == -1.0 || diff.y == 0.0))
        {
            if (targetTile == stoneTile || targetTile == coalTile || targetTile == ironTile || targetTile == goldTile)
            {
                audioSource.PlayOneShot(miningChip, 0.9F);

                GameObject tileObject = newTilemap.GetTileObject((int)targetCell.x, (int)targetCell.y);
                int tileHealth = tileObject.GetComponent<TileInfo>().GetHealth();

                if (tileHealth > 0)
                {
                    tileHealth = tileHealth - 1;
                    tileObject.GetComponent<TileInfo>().SetHealth(tileHealth);
                    tileObject.GetComponent<TileInfo>().Flash();
                    console.text = console.text + ("\nYou chip away at " + tileObject.name + ".");
                }
                else
                {
                    Vector2 dropPos = newTilemap.transform.TransformPoint(TilemapUtils.GetTileCenterPosition(newTilemap, (int)targetCell.x, (int)targetCell.y));
                    //Vector2 dropPos = TilemapUtils.GetTileCenterPosition(newTilemap, (int)targetCell.x, (int)targetCell.y);

                    Vector2 offset = new Vector2(-0.5f, -0.5f);

                    if (tileObject.name.Contains("Stone"))
                    {
                        audioSource.PlayOneShot(miningChip, 0.9F);
                        GameObject stoneObj = Instantiate(roughStone, dropPos, Quaternion.identity);
                        stoneObj.transform.SetParent(newTilemap.transform);
                        
                        console.text = console.text + "\nYou mined Stone.";
                    }else
                    if (tileObject.name.Contains("Coal"))
                    {
                        GameObject coalObj = Instantiate(coal, dropPos, Quaternion.identity);
                        coalObj.transform.SetParent(newTilemap.transform);
                        
                        console.text = console.text + "\nYou mined Coal.";
                    }
                    else
                    if (tileObject.name.Contains("Iron"))
                    {
                        GameObject ironObj = Instantiate(ironOre, dropPos, Quaternion.identity);
                        ironObj.transform.SetParent(newTilemap.transform);
                        
                        console.text = console.text + "\nYou mined Iron Ore.";
                    }
                    else
                    if (tileObject.name.Contains("Gold"))
                    {
                        GameObject goldObj = Instantiate(goldCoins, dropPos, Quaternion.identity);
                        goldObj.transform.SetParent(newTilemap.transform);
                        
                        console.text = console.text + "\nYou mined Gold.";
                    }
                    newTilemap.SetTileData(targetCell, dirtTile);
                    newTilemap.UpdateMesh();
                    
                   
                }
            }
        }
        else
        {
            console.text = console.text + "\nYou are too far away to mine that.";
        }
    }
    


    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime = 0;

        origPos = newTilemap.transform.position;
        targetPos = origPos + direction;

        audioSource.PlayOneShot(footStep, 0.7F);
        audioSource.PlayOneShot(footStep, 0.5F);
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
        if (!isMoving)
        {
            
            Vector3 playerPos = newTilemap.transform.InverseTransformPoint(transform.position);

            Vector2 targetCell = TilemapUtils.GetGridPosition(newTilemap, (playerPos + direction));

            uint testTile = newTilemap.GetTileData(targetCell);

            foreach (uint tile in blockingTiles)
            {
                //print("Testing: " + tile);
                if (tile == newTilemap.GetTileData(targetCell))
                { isBlocking = true;  return isBlocking; }
                else
                { isBlocking = false; }
            }
        }
            return isBlocking;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //console.text = console.text + "\nRock On!!! You picked up." ;
        string name = collision.gameObject.name;

        if (name.Contains("Stone"))
        {
            
            int stoneValue = 0;
            int.TryParse(stoneAmt.text, out stoneValue);
            stoneValue += 1;
            stoneAmt.text = stoneValue.ToString();
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(pickUp, 0.7F);
            console.text = console.text + "\nRock On!!! You picked up Rough Stone.";
        }
        else if (name.Contains("Coal"))
        {
            int coalValue = 0;
            int.TryParse(coalAmt.text, out coalValue);
            coalValue += 1;
            coalAmt.text = coalValue.ToString();
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(pickUp, 0.7F);
            console.text = console.text + "\nRock On!!! You picked up Coal.";
        }
        else if (name.Contains("Iron"))
        {
            int ironValue = 0;
            int.TryParse(ironAmt.text, out ironValue);
            ironValue += 1;
            ironAmt.text = ironValue.ToString();
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(pickUp, 0.7F);
            console.text = console.text + "\nRock On!!! You picked up Iron Ore.";
        }
        else if (name.Contains("Gold"))
        {
            int goldValue = 0;
            int.TryParse(goldAmt.text, out goldValue);
            goldValue += 1;
            goldAmt.text = goldValue.ToString();
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(pickUp, 0.7F);
            console.text = console.text + "\nRock On!!! You picked up Gold.";
        }
    }

    void DoTurn(Vector3 direction)
    {
      
           
        
    }
   
}
