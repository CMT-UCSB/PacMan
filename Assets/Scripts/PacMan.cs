using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    public float speed = 4.0f;
    private Vector2 dir = Vector2.zero;
    private Vector2 nextDir;
    public Vector2 orientation; 

    public Node startingNode;
    private Node prevNode;
    private Node currNode;
    private Node nextNode;

    public Sprite idling;

    private int pelletsConsumed;

    private AudioSource AS;
    private bool playedChomp1 = false;
    public AudioClip chomp1;
    public AudioClip chomp2;

    private Node startPos;

    public bool canMove = true;

    public RuntimeAnimatorController play;
    public RuntimeAnimatorController die;

    void Start()
    {
        AS = transform.GetComponent<AudioSource>();
        Node node = GetNodeAtPosition(transform.localPosition);

        startPos = node;

        if (node != null)
        {
            currNode = node;
        }

        dir = Vector2.left;
        orientation = Vector2.left;
        ChangePosition(dir);
        SetDifficulty(GameBoard.level);
    }

    void SetDifficulty(int level)
    {
        if(level == 2)
        {
            speed = 7;
        }

        else if(level == 3)
        {
            speed = 8;
        }

        else if (level == 4)
        {
            speed = 9;
        }

        else if (level > 4)
        {
            speed = 10;
        }
    }

    public void MoveToStartingPostion()
    {
        transform.position = startPos.transform.position;

        transform.GetComponent<SpriteRenderer>().sprite = idling; 

        dir = Vector2.left;
        orientation = Vector2.left;

        RotateSprite();
    }

    public void Restart()
    {
        canMove = true;

        transform.GetComponent<Animator>().runtimeAnimatorController = play;
        transform.GetComponent<Animator>().enabled = true;

        currNode = startPos;

        nextDir = Vector2.left;

        ChangePosition(dir);
    }

    void Update()
    {
        if (canMove)
        {
            Keyboard();
            RotateSprite();
            Move();
            Idle();
            ConsumePellet();
        }
    }

    void PlayChomps()
    {
        if(playedChomp1)
        {
            AS.PlayOneShot(chomp2);
            playedChomp1 = false;
        }

        else
        {
            AS.PlayOneShot(chomp1);
            playedChomp1 = true;
        }
    }

    void Keyboard()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangePosition(Vector2.left);
        }

        else if (Input.GetKeyDown(KeyCode.L))
        {
            ChangePosition(Vector2.right);
        }

        else if (Input.GetKeyDown(KeyCode.I))
        {
            ChangePosition(Vector2.up);
        }

        else if (Input.GetKeyDown(KeyCode.K))
        {
            ChangePosition(Vector2.down);
        }
    }

    void Move()
    {
        if (nextNode != currNode && nextNode != null)
        {
            if(nextDir == dir * -1)
            {
                dir *= -1;

                Node tempNode = nextNode;
                nextNode = prevNode;
                prevNode = tempNode;
            }

            if (OverShot())
            {
                currNode = nextNode;
                transform.localPosition = currNode.transform.position;

                GameObject otherPortal = GetPortal(currNode.transform.position); 

                if(otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;

                    currNode = otherPortal.GetComponent<Node>();
                }

                Node movingTo = CheckNextMove(nextDir);

                if (movingTo != null)
                {
                    dir = nextDir;
                }

                if (movingTo == null)
                {
                    movingTo = CheckNextMove(dir);
                }

                if (movingTo != null)
                {
                    nextNode = movingTo;
                    prevNode = currNode;
                    currNode = null;
                }

                else
                {
                    dir = Vector2.zero;
                }
            }

            else
            {
                transform.localPosition += (Vector3)(dir * speed) * Time.deltaTime;
            }
        }
    }

    void RotateSprite()
    {
        if(dir == Vector2.left)
        {
            orientation = Vector2.left;
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        else if(dir == Vector2.right)
        {
            orientation = Vector2.right;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        else if(dir == Vector2.up)
        {
            orientation = Vector2.up;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }

        else if(dir == Vector2.down)
        {
            orientation = Vector2.down;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
    }

    void ChangePosition(Vector2 d)
    {
        if(d != dir)
        {
            nextDir = d;
        }

        if(currNode != null)
        {
            Node movingTo = CheckNextMove(d);

            if(movingTo != null)
            {
                dir = d;
                nextNode = movingTo;
                prevNode = currNode;
                currNode = null;
            }
        }
    }

    void MakeMove(Vector2 d)
    {
        Node nextNode = CheckNextMove(d);

        if (nextNode != null)
        {
            transform.localPosition = nextNode.transform.position;
            currNode = nextNode;
        }
    }

    Node CheckNextMove(Vector2 d)
    {
        Node moveTo = null;

        for (int i = 0; i < currNode.neighbors.Length; i++)
        {
            if (currNode.directions[i] == d)
            {
                moveTo = currNode.neighbors[i];
                break;
            }
        }

        return moveTo;
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject title = GameObject.Find("GameManager").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if (title != null)
        {
            return title.GetComponent<Node>();
        }

        return null;
    }

    float LengthNodes(Vector2 target)
    {
        Vector2 vec = target - (Vector2)prevNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool OverShot()
    {
        float nodeToTarget = LengthNodes(nextNode.transform.position);
        float nodeToSelf = LengthNodes(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    void Idle()
    {
        if(dir == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idling;
        }

        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("GameManager").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if(tile != null)
        {
            if(tile.GetComponent<Tile>() != null)
            { 
                if(tile.GetComponent<Tile>().isPortal)
                {
                    GameObject otherPortal = tile.GetComponent<Tile>().portalIn;
                    return otherPortal;
                }
            }

        }

        return null;
    }

    GameObject GetTileAtPosition (Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("GameManager").GetComponent<GameBoard>().board[tileX, tileY];

        if(tile != null)
        {
            return tile;
        }

        return null;
    }

    void ConsumePellet()
    {
        GameObject obj = GetTileAtPosition(transform.position);

        if (obj != null)
        {
            Tile tile = obj.GetComponent<Tile>();

            if (tile != null)
            {
                if (!tile.consumed && (tile.isPellet || tile.isPowerup))
                {
                    obj.GetComponent<SpriteRenderer>().enabled = false;
                    tile.consumed = true;

                    if (tile.isPellet)
                    {
                        GameBoard.score += 10;
                        GameObject.Find("GameManager").GetComponent<GameBoard>().consumedPellets++;
                    }

                    else if (tile.isPowerup)
                    {
                       GameBoard.score += 50;
                        GameObject.Find("GameManager").GetComponent<GameBoard>().consumedPellets++;

                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

                        foreach(GameObject go in ghosts)
                        {
                            go.GetComponent<Ghost>().FrightenedMode();
                        }
                    }

                    pelletsConsumed++;
                    PlayChomps();
                }

                if(tile.isBonusItem)
                {
                    ConsumedBounsItem(tile);
                }
            }
        }
    }

    void ConsumedBounsItem(Tile bonusItem)
    {
        GameBoard.score += bonusItem.pointValue;

        GameObject.Find("GameManager").transform.GetComponent<GameBoard>().didConsumeBonusItem(bonusItem.gameObject, bonusItem.pointValue);

        Destroy(bonusItem.gameObject); 
    }
}