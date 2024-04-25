using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreativeSpore.SuperTilemapEditor;


namespace Svartalfheim
{

    public class PlayerController : MonoBehaviour
    {

        public STETilemap newTilemap;
        public STETilemap fogTilemap;
        //public GameObject chuckGrid;
        public GameObject gameObj;
        private GameManager gameManager;
        private FogOfWar fogOfWar;
        public Vector2 playerGridPos;

        public uint[] blockingTiles;

        public Text turnNum;
        public Text console;
        public Text stoneAmt;
        public Text coalAmt;
        public Text ironAmt;
        public Text goldAmt;

        public Canvas CraftingWindow;

        public GameObject roughStone;
        public GameObject coal;
        public GameObject ironOre;
        public GameObject goldCoins;

        public AudioSource audioSource;
        public AudioClip footstep;
        public AudioClip footstepGrass;
        public AudioClip footstepWater;
        public AudioClip miningChip;
        public AudioClip pickUp;

        private bool enemyCollision;
        private bool isMoving;
        private bool miningMode;
        public bool craftingWindow;

        private Vector3 origPos, targetPos;
        private float timeToMove = 0.2f;
        new SpriteRenderer renderer;

        private uint dirtTile = 0;
        private uint stoneTile = 63;
        private uint waterTile = 2;
        private uint grassTile = 31;
        private uint coalTile = 64;
        private uint ironTile = 94;
        private uint goldTile = 93;
        private uint bedrock = 96;

        private Vector3 lastPosition;

        public int[][] xyoffsets = new int[37][]{
                                                      new int[2] { -1, -3 }, new int[2] { 0, -3 }, new int[2] { 1, -3 },

                               new int[2] { -2, -2 }, new int[2] { -1, -2 }, new int[2] { 0, -2 }, new int[2] { 1, -2 }, new int[2] { 2, -2 },

        new int[2] { -3, -1 }, new int[2] { -2, -1 }, new int[2] { -1, -1 }, new int[2] { 0, -1 }, new int[2] { 1, -1 }, new int[2] { 2,  1 }, new int[2] { 3,  1 },

        new int[2] { -3,  0 }, new int[2] { -2,  0 }, new int[2] { -1,  0 }, new int[2] { 1,  0 }, new int[2] {-1,  1 }, new int[2] { 2,  0 }, new int[2] { 3,  0 },

        new int[2] { -3,  1 }, new int[2] { -2,  1 }, new int[2] {  0,  1 }, new int[2] { 1,  1 }, new int[2] { 0,  0 }, new int[2] { 2, -1 }, new int[2] { 3, -1 },

                               new int[2] { -2,  2 }, new int[2] { -1,  2 }, new int[2] { 0,  2 }, new int[2] { 1,  2 }, new int[2] { 2, 2 },

                                                      new int[2] { -1,  3 }, new int[2] { 0,  3 }, new int[2] { 1,  3 }
    };

        // Start is called before the first frame update
        void Start()
        {
            craftingWindow = false; // Crafting window is not open by default

            gameManager = gameObj.GetComponent<GameManager>(); // Get the GameManager script

            renderer = this.GetComponent<SpriteRenderer>(); // Get the sprite renderer

            // Set the blocking tiles
            blockingTiles[0] = stoneTile; 
            blockingTiles[1] = coalTile;
            blockingTiles[2] = ironTile;
            blockingTiles[3] = goldTile;
            blockingTiles[4] = bedrock;

            lastPosition = transform.position; // Set last position to the player's starting position

        }


        void FixedUpdate()
        {
            if (!isMoving)
            {
                lastPosition = transform.position;
            }

            if (Input.GetKey(KeyCode.A) && !isMoving)
            {

                if (!CheckNeighbor(Vector3.left))
                {
                    renderer.flipX = false;
                    StartCoroutine(MovePlayer(Vector3.right, GetNeighbor(Vector3.left)));
                    DoTurn();
                }
            }
            if (Input.GetKey(KeyCode.D) && !isMoving)
            {
                if (!CheckNeighbor(Vector3.right))
                {
                    renderer.flipX = true;
                    StartCoroutine(MovePlayer(Vector3.left, GetNeighbor(Vector3.right)));
                    DoTurn();
                }

            }
            if (Input.GetKey(KeyCode.W) && !isMoving)
            {
                if (!CheckNeighbor(Vector3.up))
                {
                    StartCoroutine(MovePlayer(Vector3.down, GetNeighbor(Vector3.up)));
                    DoTurn();
                }

            }

            if (Input.GetKey(KeyCode.S) && !isMoving)
            {
                if (!CheckNeighbor(Vector3.down))
                {
                    StartCoroutine(MovePlayer(Vector3.up, GetNeighbor(Vector3.down)));
                    DoTurn();
                }

            }

            /*
            if (Input.GetKeyDown(KeyCode.G))
            {
                miningMode = true;
                //console.text += "\nMining Mode Activated.";
            }
            */

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!craftingWindow)
                {
                    craftingWindow = true;
                }
                else
                {
                    craftingWindow = false;
                }


            }

            if (craftingWindow)
            {
                CraftingWindow.gameObject.SetActive(true);
            }
            else if (!craftingWindow)
            {
                CraftingWindow.gameObject.SetActive(false);
            }
        }

        public void DoMine(Vector2 targetPos)
        {

            //if (miningMode)
            //{
            Vector3 playerPos = newTilemap.transform.InverseTransformPoint(transform.position);
            Vector2 playerGridPos = TilemapUtils.GetGridPosition(newTilemap, (playerPos));

            Vector2 targetCell = TilemapUtils.GetGridPosition(newTilemap, (targetPos));
            uint targetTile = newTilemap.GetTileData(targetCell);

            Vector2 diff = (playerGridPos - targetCell);


            if ((diff.x == 1.0 || diff.x == -1.0 || diff.x == 0.0) && (diff.y == 1.0 || diff.y == -1.0 || diff.y == 0.0))
            {
                if (targetTile == stoneTile || targetTile == coalTile || targetTile == ironTile || targetTile == goldTile)
                {
                    audioSource.PlayOneShot(miningChip, 0.9F);

                    GameObject tileObject = newTilemap.GetTileObject((int)targetCell.x, (int)targetCell.y);
                    int tileHealth = tileObject.GetComponent<TileInfo>().GetHealth();

                    if (tileHealth > 1)
                    {
                        tileHealth = tileHealth - 1;
                        tileObject.GetComponent<TileInfo>().SetHealth(tileHealth);
                        tileObject.GetComponent<TileInfo>().Flash();
                        miningMode = false;
                        string clone = "(Clone)";
                        string tileName = tileObject.name;
                        string name = tileName.Replace(clone, "");
                        console.text = console.text + ("\nYou chip away at " + name + ".");
                    }
                    else if (tileHealth <= 1)
                    {
                        Vector2 dropPos = newTilemap.transform.TransformPoint(TilemapUtils.GetTileCenterPosition(newTilemap, (int)targetCell.x, (int)targetCell.y));
                        //Vector2 dropPos = TilemapUtils.GetTileCenterPosition(newTilemap, (int)targetCell.x, (int)targetCell.y);

                        Vector2 offset = new Vector2(-0.5f, -0.5f);

                        if (tileObject.name.Contains("Stone"))
                        {

                            GameObject stoneObj = Instantiate(roughStone, dropPos, Quaternion.identity);
                            string clone = "(Clone)";
                            string objName = stoneObj.name;
                            string name = objName.Replace(clone, "");
                            stoneObj.name = objName;
                            stoneObj.transform.SetParent(newTilemap.transform);
                            miningMode = false;
                            console.text = console.text + "\nYou mined Stone.";
                        }
                        else
                        if (tileObject.name.Contains("Coal"))
                        {
                            GameObject coalObj = Instantiate(coal, dropPos, Quaternion.identity);
                            string clone = "(Clone)";
                            string objName = coalObj.name;
                            string name = objName.Replace(clone, "");
                            coalObj.name = objName;
                            coalObj.transform.SetParent(newTilemap.transform);
                            miningMode = false;
                            console.text = console.text + "\nYou mined Coal.";
                        }
                        else
                        if (tileObject.name.Contains("Iron"))
                        {
                            GameObject ironObj = Instantiate(ironOre, dropPos, Quaternion.identity);
                            string clone = "(Clone)";
                            string objName = ironObj.name;
                            string name = objName.Replace(clone, "");
                            ironObj.name = objName;
                            ironObj.transform.SetParent(newTilemap.transform);
                            miningMode = false;
                            console.text = console.text + "\nYou mined Iron Ore.";
                        }
                        else
                        if (tileObject.name.Contains("Gold"))
                        {
                            GameObject goldObj = Instantiate(goldCoins, dropPos, Quaternion.identity);
                            string clone = "(Clone)";
                            string objName = goldObj.name;
                            string name = objName.Replace(clone, "");
                            goldObj.name = objName;
                            goldObj.transform.SetParent(newTilemap.transform);
                            miningMode = false;
                            console.text = console.text + "\nYou mined Gold.";
                        }
                        newTilemap.SetTileData(targetCell, dirtTile);
                        newTilemap.UpdateMesh();


                    }
                }
                DoTurn();
            }
            else
            {
                console.text = console.text + "\nYou are too far away to mine that.";
            }
            // }
        }



        private IEnumerator MovePlayer(Vector3 direction, uint groundType)
        {
            if (!enemyCollision)
            {
                isMoving = true;
                float elapsedTime = 0;
                origPos = newTilemap.transform.position;
                targetPos = origPos + direction;
                Vector2 targetCell = TilemapUtils.GetGridPosition(newTilemap, (targetPos));

                if (groundType == dirtTile)
                {
                    audioSource.PlayOneShot(footstep, 0.5F);
                    yield return new WaitForSeconds(.2f);
                }
                else if (groundType == grassTile)
                {
                    audioSource.PlayOneShot(footstepGrass, 0.5F);
                    yield return new WaitForSeconds(.2f);
                }
                else if (groundType == waterTile)
                {
                    console.text += "\nYou got wet!";
                    audioSource.PlayOneShot(footstepWater, 0.6F);
                    yield return new WaitForSeconds(.2f);
                }
                else if (groundType == stoneTile || groundType == coalTile || groundType == ironTile || groundType == goldTile)
                {
                    if (miningMode)
                    { DoMine(targetCell); }
                }
                


                while (elapsedTime < timeToMove)
                {
                    newTilemap.transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                newTilemap.transform.position = targetPos;
                Vector2 gridPos = TilemapUtils.GetGridWorldPos(newTilemap, (int)targetPos.x, (int)targetPos.y);
                Debug.Log("Player moved to " + gridPos.x + " , " + gridPos.y);
                isMoving = false;
            }
            else
            {
                console.text = console.text + "\nYou can't move there, there is an enemy in the way!";
            }
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
                    if (tile == newTilemap.GetTileData(targetCell))
                    { isBlocking = true; return isBlocking; }
                    else
                    { isBlocking = false; }
                }
            }
            return isBlocking;

        }

        uint GetNeighbor(Vector3 direction)
        {
            Vector3 playerPos = newTilemap.transform.InverseTransformPoint(transform.position);
            Vector2 targetCell = TilemapUtils.GetGridPosition(newTilemap, (playerPos + direction));

            uint testTile = newTilemap.GetTileData(targetCell);

            return testTile;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

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
            else if (name.Contains("Enemy"))
            {
                Debug.Log("Enemy Collision");
                enemyCollision = true;
                transform.position = lastPosition;
            }
            else if (!name.Contains("Enemy"))
            {
                enemyCollision = false;
            }
        }

        void DoTurn()
        {

            //fogOfWar = fogTilemap.GetComponent<FogOfWar>();

            //fogOfWar.SetExploredTiles();
           // fogOfWar.UpdateFog();

            //Update Turn number.
            int tNum;
            int.TryParse(turnNum.text, out tNum); // get the current turn number and output it to tNum string.
            tNum++; // increment tNum by 1.
            turnNum.text = tNum.ToString(); // set the turnNum text to the new value of tNum.


        }

    }
}
