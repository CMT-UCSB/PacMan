using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 5.9f;
    public float normalSpeed = 5.9f;
    public float frightenedSpeed = 2.9f;
    public float consumedSpeed = 15f;
    private float prevSpeed;

    public Node startPos;
    public Node home;
    public Node ghostHouse;

    private Node prevNode;
    private Node currNode;
    private Node nextNode;
    
    private Vector2 dir;
    private Vector2 nextDir;

    private GameObject PM;

    public bool inGhostHouse = false;

    public int pinkyRelease = 5;
    public int inkyRelease = 14;
    public int clydeRelease = 21;
    public float ghostRelease = 0;

    public int frightenedDuration = 10;
    public int startBlinking = 7;

    private float frightenedTimer = 0;
    private float blinkingTimer = 0;
    private bool frightenedWhite = false;

    public int scatter1 = 7;
    public int chase1 = 20;
    public int scatter2 = 7;
    public int chase2 = 20;
    public int scatter3 = 5;
    public int chase3 = 20;
    public int scatter4 = 5;
    public int chase4 = 20;

    public enum GhostName
    {
        Blinky,
        Pinky,
        Inky,
        Clyde
    }

    public GhostName ghost = GhostName.Blinky;

    public int modeChange = 1;
    private float modeChangeTimer = 0;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightened,
        Consumed
    }

    Mode currMode = Mode.Scatter;
    Mode prevMode;

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostBlue;
    public RuntimeAnimatorController ghostWhite;

    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    private AudioSource bgAudio;

    public bool canMove = true;

    void Start()
    {
        SetDifficulty(GameBoard.level);
        bgAudio = GameObject.Find("GameManager").transform.GetComponent<AudioSource>();

        PM = GameObject.FindGameObjectWithTag("PacMan");

        Node node = GetNodeAtPosition(transform.localPosition);

        if(node != null)
        {
            currNode = node;
        }

        if(inGhostHouse)
        {
            dir = Vector2.up;
            nextNode = currNode.neighbors[0];
        }

        else
        {
            dir = Vector2.left;
            nextNode = ChooseNextNode();
        }

        prevNode = currNode;
    }

    void SetDifficulty(int level)
    {
        if (level == 2)
        {
            scatter1 = 7;
            scatter2 = 7;
            scatter3 = 5;
            scatter4 = 1;

            chase1 = 20;
            chase2 = 20;
            chase3 = 1033;

            frightenedDuration = 9;
            startBlinking = 6;

            pinkyRelease = 4;
            inkyRelease = 12;
            clydeRelease = 18;

            moveSpeed = 6.9f;
            normalSpeed = 6.9f;
            frightenedSpeed = 3.9f;
            consumedSpeed = 18f;
        }

        else if (level == 3)
        {
            scatter1 = 7;
            scatter2 = 7;
            scatter3 = 5;
            scatter4 = 1;

            chase1 = 20;
            chase2 = 20;
            chase3 = 1033;

            frightenedDuration = 8;
            startBlinking = 5;

            pinkyRelease = 3;
            inkyRelease = 10;
            clydeRelease = 15;

            moveSpeed = 7.9f;
            normalSpeed = 7.9f;
            frightenedSpeed = 4.9f;
            consumedSpeed = 20f;
        }

        else if (level == 4)
        {
            scatter1 = 7;
            scatter2 = 7;
            scatter3 = 5;
            scatter4 = 1;

            chase1 = 20;
            chase2 = 20;
            chase3 = 1033;

            frightenedDuration = 7;
            startBlinking = 4;

            pinkyRelease = 2;
            inkyRelease = 8;
            clydeRelease = 13;

            moveSpeed = 8.9f;
            normalSpeed = 8.9f;
            frightenedSpeed = 4.9f;
            consumedSpeed = 22f;
        }

        else if (level > 4)
        {
            scatter1 = 5;
            scatter2 = 5;
            scatter3 = 5;
            scatter4 = 1;

            chase1 = 20;
            chase2 = 20;
            chase3 = 1037;

            frightenedDuration = 6;
            startBlinking = 3;

            pinkyRelease = 2;
            inkyRelease = 6;
            clydeRelease = 10;

            moveSpeed = 9.9f;
            normalSpeed = 9.9f;
            frightenedSpeed = 6.9f;
            consumedSpeed = 24f;
        }
    }

    public void MoveToStartingPosition()
    {
        if (transform.name != "Blinky")
        {
            inGhostHouse = true;
        }

        transform.position = startPos.transform.position;

        if(inGhostHouse)
        {
            dir = Vector2.up;
        }

        else
        {
            dir = Vector2.left;
        }

        UpdateAnimation();
    }

    public void Restart()
    {
        canMove = true;

        currMode = Mode.Scatter;

        moveSpeed = normalSpeed;
        prevSpeed = 0f;

        ghostRelease = 0;
        modeChange = 1;
        modeChangeTimer = 0;

        if(ghost != GhostName.Blinky)
        {
            inGhostHouse = true;
        }

        currNode = startPos;

        if(inGhostHouse)
        {
            dir = Vector2.up;
            nextNode = ChooseNextNode();
        }

        else
        {
            dir = Vector2.left;
            nextNode = ChooseNextNode();
        }

        prevNode = currNode;
        UpdateAnimation();
    }

    void Update()
    {
        if(canMove)
        {
            Move();
            ModeUpdate();
            ReleaseGhosts();
            CheckCollision();
            CheckGhostHouse();
        }
    }

    void CheckGhostHouse()
    {
        if(currMode == Mode.Consumed)
        {
            GameObject tile = GetTileAtPosition(transform.position);

            if(tile != null)
            {
                if(tile.transform.GetComponent<Tile>() != null)
                {
                    if(tile.GetComponent<Tile>().isGhostHouse)
                    {
                        moveSpeed = normalSpeed;

                        Node node = GetNodeAtPosition(transform.position);

                        if(node != null)
                        {
                            currNode = node;

                            dir = Vector2.up;

                            nextNode = currNode.neighbors[0];
                            prevNode = currNode;

                            currMode = Mode.Chase;

                            UpdateAnimation();
                        }
                    }
                }
            }
        }
    }

    void CheckCollision()
    {
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);
        Rect pacmanRect = new Rect(PM.transform.position, PM.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);

        if(ghostRect.Overlaps(pacmanRect))
        {
            if(currMode == Mode.Frightened)
            {
                Consumed();
            }

            else
            {
                if(currMode != Mode.Consumed)
                {
                    GameObject.Find("GameManager").transform.GetComponent<GameBoard>().StartDeath();
                }
            }
        }
    }

    void Consumed()
    {
        GameBoard.score += GameBoard.ghostCombo;

        currMode = Mode.Consumed;
        prevSpeed = moveSpeed;
        moveSpeed = consumedSpeed;

        UpdateAnimation();

        GameObject.Find("GameManager").transform.GetComponent<GameBoard>().StartConsumed(this.GetComponent<Ghost>());

        GameBoard.ghostCombo *= 2;
    }

    void UpdateAnimation()
    {
        if (currMode != Mode.Frightened && currMode != Mode.Consumed)
        {
            if (dir == Vector2.up)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
            }

            else if (dir == Vector2.down)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
            }

            else if (dir == Vector2.left)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }

            else if (dir == Vector2.right)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
            }

            else
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            }
        }

        else if(currMode == Mode.Frightened)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
        }

        else if(currMode == Mode.Consumed)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = null;

            if(dir == Vector2.up)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            }

            else if (dir == Vector2.down)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            }

            else if (dir == Vector2.left)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }

            else if (dir == Vector2.right)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
        }
    }

    void Move()
    {
        if(nextNode != currNode && nextNode != null && !inGhostHouse)
        {
            if(OverShot())
            {
                currNode = nextNode;
                transform.localPosition = currNode.transform.position;

                GameObject otherPortal = GetPortal(currNode.transform.position);

                if(otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currNode = otherPortal.GetComponent<Node>();
                }

                nextNode = ChooseNextNode();
                prevNode = currNode;
                currNode = null;

                UpdateAnimation();
            }

            else
            {
                transform.localPosition += (Vector3)dir * moveSpeed * Time.deltaTime;
            }
        }
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("GameManager").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if(tile != null)
        {
            if(tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>();
            }
        }

        return null;
    }

    void ChangeMode(Mode m)
    {
        if(currMode == Mode.Frightened)
        {
            moveSpeed = prevSpeed;
        }

        if(m == Mode.Frightened)
        {
            prevSpeed = moveSpeed;
            moveSpeed = frightenedSpeed;
        }

        if(currMode != m)
        {
            prevMode = currMode;
            currMode = m;
        }

        UpdateAnimation();
    }

    public void FrightenedMode()
    {
        if(currMode != Mode.Consumed)
        {
            GameBoard.ghostCombo = 200;

            frightenedTimer = 0;
            bgAudio.clip = GameObject.Find("GameManager").transform.GetComponent<GameBoard>().backgroundFrightened;
            bgAudio.Play();
            ChangeMode(Mode.Frightened);
        }
    }

    Vector2 GetBlinkyTargetTile()
    {
        Vector2 pmPos = PM.transform.localPosition;
        Vector2 target = new Vector2(Mathf.RoundToInt(pmPos.x), Mathf.RoundToInt(pmPos.y));

        return target;
    }

    Vector2 GetPinkyTargetTile()
    {
        Vector2 pmPos = PM.transform.localPosition;
        Vector2 pmOr = PM.GetComponent<PacMan>().orientation;

        int pmPosX = Mathf.RoundToInt(pmPos.x);
        int pmPosY = Mathf.RoundToInt(pmPos.y);

        Vector2 pmTile = new Vector2(pmPosX, pmPosY);
        Vector2 target = pmTile + (4 * pmOr);

        return target;
    }

    Vector2 GetInkyTargetTile()
    {
        Vector2 pmPos = PM.transform.localPosition;
        Vector2 pmOr = PM.GetComponent<PacMan>().orientation;

        int pmPosX = Mathf.RoundToInt(pmPos.x);
        int pmPosY = Mathf.RoundToInt(pmPos.y);

        Vector2 pmTile = new Vector2(pmPosX, pmPosY);
        Vector2 target = pmTile + (2 * pmPos);

        Vector2 tempPos = GameObject.Find("Blinky").transform.localPosition;

        int blinkyPosX = Mathf.RoundToInt(tempPos.x);
        int blinkyPosY = Mathf.RoundToInt(tempPos.y);

        tempPos = new Vector2(blinkyPosX, blinkyPosY);

        float distance = Distance(tempPos, target);
        distance *= 2;

        target = new Vector2(tempPos.x + distance, tempPos.y + distance);

        return target;
    }

    Vector2 GetClydeTargetTile()
    {
        Vector2 pmPos = PM.transform.localPosition;

        float distance = Distance(transform.localPosition, pmPos);
        Vector2 target = Vector2.zero;

        if(distance >= 8)
        {
            target = new Vector2(Mathf.RoundToInt(pmPos.x), Mathf.RoundToInt(pmPos.y));
        }

        else if(distance < 8)
        {
            target = home.transform.position;
        }

        return target;
    }

    Vector2 GetTargetTile()
    {
        Vector2 target = Vector2.zero;

        if(ghost == GhostName.Blinky)
        {
            target = GetBlinkyTargetTile();
        }

        else if(ghost == GhostName.Pinky)
        {
            target = GetPinkyTargetTile();
        }

        else if (ghost == GhostName.Inky)
        {
            target = GetInkyTargetTile();
        }

        else if (ghost == GhostName.Clyde)
        {
            target = GetClydeTargetTile();
        }

        return target;
    }

    Vector2 GetRandomTile()
    {
        int x = Random.Range(0, 28);
        int y = Random.Range(0, 36);

        return new Vector2(x, y);
    }

    void ReleasePinky()
    {
        if(ghost == GhostName.Pinky && inGhostHouse)
        {
            inGhostHouse = false;
        }
    }

    void ReleaseInky()
    {
        if(ghost == GhostName.Inky && inGhostHouse)
        {
            inGhostHouse = false;
        }
    }

    void ReleaseClyde()
    {
        if (ghost == GhostName.Clyde && inGhostHouse)
        {
            inGhostHouse = false;
        }
    }

    void ReleaseBlinky()
    {
        if(ghost == GhostName.Blinky && inGhostHouse)
        {
            inGhostHouse = false;
        }
    }

    void ReleaseGhosts()
    {
        ghostRelease += Time.deltaTime;

        if(ghostRelease > pinkyRelease)
        {
            ReleasePinky();
        }

        if(ghostRelease > inkyRelease)
        {
            ReleaseInky();
        }

        if(ghostRelease > clydeRelease)
        {
            ReleaseClyde();
        }
    }

    Node ChooseNextNode()
    {
        Vector2 target = Vector2.zero;

        if(currMode == Mode.Chase)
        {
            target = GetTargetTile();
        }

        else if(currMode == Mode.Scatter)
        {
            target = home.transform.position;
        }

        else if(currMode == Mode.Frightened)
        {
            target = GetRandomTile();
        }

        else if(currMode == Mode.Consumed)
        {
            target = ghostHouse.transform.position;
            Debug.Log("Ghost House = " + target);
        }

        Node movingTo = null;

        Node[] foundNodes = new Node[4];
        Vector2[] foundDirections = new Vector2[4];

        int nodeCtr = 0;

        for(int i = 0; i < currNode.neighbors.Length; i++)
        {
            if(currNode.directions[i] != -dir)
            {
                if (currMode != Mode.Consumed)
                {
                    GameObject tile = GetTileAtPosition(currNode.transform.position);

                    if (tile.transform.GetComponent<Tile>().isGhostHouseEntrance)
                    {
                        if (currNode.directions[i] != Vector2.down)
                        {
                            foundNodes[nodeCtr] = currNode.neighbors[i];
                            foundDirections[nodeCtr] = currNode.directions[i];
                            nodeCtr++;
                        }
                    }

                    else
                    {
                        foundNodes[nodeCtr] = currNode.neighbors[i];
                        foundDirections[nodeCtr] = currNode.directions[i];
                        nodeCtr++;
                    }
                }

                else
                {
                    foundNodes[nodeCtr] = currNode.neighbors[i];
                    foundDirections[nodeCtr] = currNode.directions[i];
                    nodeCtr++;
                }
            }
        }

        if(foundNodes.Length == 1)
        {
            movingTo = foundNodes[0];
            dir = foundDirections[0];
        }

        if(foundNodes.Length > 1)
        {
            float leastDis = 10000f;

            for(int j = 0; j < foundNodes.Length; j++)
            {
                if(foundDirections[j] != Vector2.zero)
                {
                    float distance = Distance(foundNodes[j].transform.position, target);

                    if(distance < leastDis)
                    {
                        leastDis = distance;
                        movingTo = foundNodes[j];
                        dir = foundDirections[j];
                    }
                }
            }
        }

        Debug.Log(movingTo);
        return movingTo;
    }

    void ModeUpdate()
    {
        if(currMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;

            if(modeChange == 1)
            {
                if(currMode == Mode.Scatter && modeChangeTimer > scatter1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if(currMode == Mode.Chase && modeChangeTimer > chase1)
                {
                    modeChange = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }

            if (modeChange == 2)
            {
                if (currMode == Mode.Scatter && modeChangeTimer > scatter2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currMode == Mode.Chase && modeChangeTimer > chase2)
                {
                    modeChange = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }

            if (modeChange == 3)
            {
                if (currMode == Mode.Scatter && modeChangeTimer > scatter3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currMode == Mode.Chase && modeChangeTimer > chase3)
                {
                    modeChange = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }

            if (modeChange == 4)
            {
                if (currMode == Mode.Scatter && modeChangeTimer > scatter4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
            }
        }

        else if(currMode == Mode.Frightened)
        {
            frightenedTimer += Time.deltaTime;

            if(frightenedTimer >= frightenedDuration)
            {
                bgAudio.clip = GameObject.Find("GameManager").transform.GetComponent<GameBoard>().backgroundNormal;
                bgAudio.Play();

                frightenedTimer = 0;
                ChangeMode(prevMode);
            }

            if(frightenedTimer >= startBlinking)
            {
                blinkingTimer += Time.deltaTime;

                if(blinkingTimer >= 0.1f)
                {
                    blinkingTimer = 0f;

                    if(frightenedWhite)
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
                        frightenedWhite = false;
                    }

                    else
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostWhite;
                        frightenedWhite = true;
                    }
                }
            }
        }
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("GameManager").transform.GetComponent<GameBoard>().board[tileX, tileY];

        if(tile != null)
        {
            return tile;
        }

        return null;
    }

    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("GameManager").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if(tile != null)
        {
            if(tile.GetComponent<Tile>().isPortal)
            {
                GameObject otherPortal = tile.GetComponent<Tile>().portalIn;
                return otherPortal;
            }
        }

        return null;
    }

    float LengthNode(Vector2 target)
    {
        Vector2 vec = target - (Vector2)prevNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool OverShot()
    {
        float nodeToTarget = LengthNode(nextNode.transform.position);
        float nodeToSelf = LengthNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float Distance(Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y = posB.y;

        return Mathf.Sqrt((dx * dx) + (dy * dy));
    }
}
