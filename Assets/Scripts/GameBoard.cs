using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour 
{
    private static int boardWidth = 28;
    private static int boardLength = 36;

    private bool didStartDeath = false;
    private bool didStartConsumed = false;

    public int totalPellets = 244;
    public int consumedPellets = 0;
    public static int score = 0;

    public static int level = 1;
    public int lives = 3;

    public bool blink = false;
    public float blinkTime = 0.1f;
    private float blinkTimer = 0;

    public AudioClip backgroundNormal;
    public AudioClip backgroundFrightened;
    public AudioClip backgroundDeath;
    public AudioClip ghostDeath;

    public Sprite mazeBlue;
    public Sprite mazeWhite;

    public GameObject[,] board = new GameObject[boardWidth, boardLength];

    public Image[] levelImages;

    public Text player;
    public Text ready;
    public Text highScore;
    public Text highScoreText;
    public Text playerOneScore;
    public Text scoreText;
    public Text ghostScore;

    public Image playerLives2;
    public Image playerLives3;

    private bool increaseLevel = false;

    public static int ghostCombo;

    public bool didSpawnBonus1;
    public bool didSpawnBonus2;

    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach(GameObject o in objects)
        {
            Vector2 pos = o.transform.position;

            if(o.name != "Pacman" && o.tag != "Home" && o.tag != "Ghost" && o.tag != "UIElement" && o.name != "Intersections" && o.name != "Maze" && o.name != "Pellets") //PC.transform.name)
            {
                board[(int)pos.x, (int)pos.y] = o;
            }
        }

        if(level == 1)
        {
            GetComponent<AudioSource>().Play();
        }

        StartGame();
    }

    void Update()
    {
        UpdateUI();

        CheckPelletsConsumed();

        CheckShouldBlink();

        BonusItems();
    }

    void BonusItems()
    {
        if(consumedPellets >= 70 && consumedPellets < 170)
        {
            if(!didSpawnBonus1)
            {
                didSpawnBonus1 = true;
                SpawnBonusItem(level);
            }
        }

        else if(consumedPellets >= 170)
        {
            if(!didSpawnBonus2)
            {
                didSpawnBonus2 = true;
                SpawnBonusItem(level);
            }
        }
    }

    void SpawnBonusItem(int lvl)
    {
        GameObject bonusItem = null;

        if(level == 1)
        {
            bonusItem = Resources.Load("Prefabs/bonus_cherries", typeof(GameObject)) as GameObject;
        }

        else if(level == 2)
        {
            bonusItem = Resources.Load("Prefabs/bonus_strawberry", typeof(GameObject)) as GameObject;
        }

        else if(level == 3)
        {
            bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
        }

        else if(level == 4)
        {
            bonusItem = Resources.Load("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
        }

        else if(level == 5)
        {
            bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
        }

        else if(level == 6)
        {
            bonusItem = Resources.Load("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
        }

        else if(level == 7)
        {
            bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
        }

        else if(level == 8)
        {
            bonusItem = Resources.Load("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
        }

        else if(level == 9)
        {
            bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
        }

        else if(level == 10)
        {
            bonusItem = Resources.Load("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
        }

        else if(level == 11)
        {
            bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
        }

        else if(level == 12)
        {
            bonusItem = Resources.Load("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
        }

        else 
        {
            bonusItem = Resources.Load("Prefabs/bonus_key", typeof(GameObject)) as GameObject;
        }

        Instantiate(bonusItem);
    }

    void UpdateUI()
    {
        scoreText.text = score.ToString();

        if(lives == 3)
        {
            playerLives3.enabled = true;
            playerLives2.enabled = true;
        }

        else if(lives == 2)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = true;
        }

        else if(lives == 1)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = false;
        }

        for(int i = 0; i < levelImages.Length; i++)
        {
            Image li = levelImages[i];
            li.enabled = false;
        }

        for(int j = 0; j < levelImages.Length; j++)
        {
            if(level >= j + 1)
            {
                Image li = levelImages[j];
                li.enabled = true;
            }
        }
    }

    void CheckPelletsConsumed()
    {
        if(totalPellets == consumedPellets)
        {
            PlayerWin(1); 
        }
    }

    void PlayerWin(int playerNum)
    {
        if(!increaseLevel)
        {
            level++;
            increaseLevel = true;
            StartCoroutine(ProcessWin(2));
        }
    }

    IEnumerator ProcessWin(float delay)
    {
        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<PacMan>().canMove = false;
        PM.transform.GetComponent<Animator>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject o in ghosts)
        {
            o.transform.GetComponent<Ghost>().canMove = false;
            o.transform.GetComponent<Animator>().enabled = false;
        }

        yield return new WaitForSeconds(delay);

        StartCoroutine(BlinkBoard(2));
    }

    IEnumerator BlinkBoard (float delay)
    {
        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject o in ghosts)
        {
            o.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        blink = true;

        yield return new WaitForSeconds(delay);

        blink = false;
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        StopAllCoroutines();

        consumedPellets = 0;
        didSpawnBonus1 = false;
        didSpawnBonus2 = false;

        GameObject.Find("Blue").transform.GetComponent<SpriteRenderer>().sprite = mazeBlue;

        increaseLevel = false;

        SceneManager.LoadScene("Level1"); //StartCoroutine(ProcessStartNextLevel(1)); //
    }

    /*IEnumerator ProcessStartNextLevel(float delay)
    {
        player.transform.GetComponent<Text>().enabled = true;
        ready.transform.GetComponent<Text>().enabled = true;
    }*/

    private void CheckShouldBlink()
    {
        if(blink)
        {
            
            if(blinkTimer < blinkTime)
            {
                blinkTimer += Time.deltaTime;
            }

            else
            {
                blinkTimer = 0;

                if(GameObject.Find("Blue").transform.GetComponent<SpriteRenderer>().sprite == mazeBlue)
                {
                    GameObject.Find("Blue").transform.GetComponent<SpriteRenderer>().sprite = mazeWhite;
                }

                else
                {
                    GameObject.Find("Blue").transform.GetComponent<SpriteRenderer>().sprite = mazeBlue;
                }
            }
        }
    }

    public void StartGame()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject o in ghosts)
        {
            o.transform.GetComponent<SpriteRenderer>().enabled = false;
            o.transform.GetComponent<Ghost>().canMove = false;
        }

        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<SpriteRenderer>().enabled = false;
        PM.transform.GetComponent<PacMan>().canMove = false;

        StartCoroutine(DelayStart(2.25f));
    }

    public void StartConsumed(Ghost consumedGhost)
    {
        if(!didStartConsumed)
        {
            didStartConsumed = true;

            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject o in ghosts)
            {
                o.transform.GetComponent<Ghost>().canMove = false;
            }

            GameObject PM = GameObject.Find("Pacman");
            PM.transform.GetComponent<SpriteRenderer>().enabled = false;
            PM.transform.GetComponent<PacMan>().canMove = false;

            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            Vector2 pos = consumedGhost.transform.position;
            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

            ghostScore.GetComponent<RectTransform>().anchorMin = viewPortPoint;
            ghostScore.GetComponent<RectTransform>().anchorMax = viewPortPoint;

            ghostScore.text = ghostCombo.ToString();

            ghostScore.GetComponent<Text>().enabled = true;

            transform.GetComponent<AudioSource>().PlayOneShot(ghostDeath);

            StartCoroutine(AfterConsumed(0.75f, consumedGhost));
        }
    }

    public void didConsumeBonusItem(GameObject bonusItem, int scorevalue)
    {
        Vector2 pos = bonusItem.transform.position;
        Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

        ghostScore.GetComponent<RectTransform>().anchorMin = viewPortPoint;
        ghostScore.GetComponent<RectTransform>().anchorMax = viewPortPoint;

        ghostScore.text = scorevalue.ToString();

        ghostScore.GetComponent<Text>().enabled = true;

        Destroy(bonusItem.gameObject); 

        StartCoroutine(ProcessConsumedBonusItem(0.75f));
    }

    IEnumerator ProcessConsumedBonusItem(float delay)
    {
        yield return new WaitForSeconds(delay);

        ghostScore.GetComponent<Text>().enabled = false;
    }

    IEnumerator AfterConsumed(float delay, Ghost consumedGhost)
    {
        yield return new WaitForSeconds(delay);

        ghostScore.GetComponent<Text>().enabled = false;

        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<SpriteRenderer>().enabled = true;

        consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = true;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject o in ghosts)
        {
            o.transform.GetComponent<Ghost>().canMove = true;
        }

        PM.transform.GetComponent<PacMan>().canMove = true;

        transform.GetComponent<AudioSource>().Play();

        didStartConsumed = false;
    }

    IEnumerator DelayStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject o in ghosts)
        {
            o.transform.GetComponent<SpriteRenderer>().enabled = true;
        }

        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<SpriteRenderer>().enabled = true;

        player.transform.GetComponent<Text>().enabled = false;

        StartCoroutine(StartGameAfter(2));
    }

    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject o in ghosts)
        {
            o.transform.GetComponent<Ghost>().canMove = true;
        }

        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<PacMan>().canMove = true;

        ready.transform.GetComponent<Text>().enabled = false;

        transform.GetComponent<AudioSource>().clip = backgroundNormal;
        transform.GetComponent<AudioSource>().Play(); 
    }

    public void StartDeath()
    {
        if(!didStartDeath)
        {
            StopAllCoroutines();

            GameObject bonusitem = GameObject.Find("bonusItem");

            if(bonusitem)
            {
                Destroy(bonusitem.gameObject);
            }

            didStartDeath = true;

            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject o in ghosts)
            {
                o.transform.GetComponent<Ghost>().canMove = false;
            }

            GameObject PM = GameObject.Find("Pacman");
            PM.transform.GetComponent<PacMan>().canMove = false;

            PM.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(AfterDeath(2));
        }
    }

    IEnumerator AfterDeath(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject o in ghosts)
        {
            o.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(DeathAnimation(1.9f));
    }

    IEnumerator DeathAnimation(float delay)
    {
        GameObject PM = GameObject.Find("Pacman");

        PM.transform.localScale = new Vector3(1, 1, 1);
        PM.transform.localRotation = Quaternion.Euler(0, 0, 0);

        PM.transform.GetComponent<Animator>().runtimeAnimatorController = PM.transform.GetComponent<PacMan>().die;
        PM.transform.GetComponent<Animator>().enabled = true;

        transform.GetComponent<AudioSource>().clip = backgroundDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(1));
    }

    IEnumerator ProcessRestart(float delay)
    {
        lives--;

        if (lives == 0)
        {
            player.transform.GetComponent<Text>().enabled = true;

            ready.transform.GetComponent<Text>().text = "GAME OVER";
            ready.transform.GetComponent<Text>().color = new Color(170f / 255f, 19f / 255f, 0f, 255f);
            ready.transform.GetComponent<Text>().enabled = true;

            GameObject PM = GameObject.Find("Pacman");
            PM.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(GameOver(2));
        }

        else
        {
            player.transform.GetComponent<Text>().enabled = true;
            ready.transform.GetComponent<Text>().enabled = true;

            GameObject PM = GameObject.Find("Pacman");
            PM.transform.GetComponent<SpriteRenderer>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            yield return new WaitForSeconds(delay);

            StartCoroutine(RestartShowObjects(1));
        }
    }

    IEnumerator GameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Menu");
    }

    IEnumerator RestartShowObjects(float delay)
    {
        player.transform.GetComponent<Text>().enabled = false;

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject o in ghosts)
        {
            o.transform.GetComponent<SpriteRenderer>().enabled = true;
            o.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }

        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<Animator>().enabled = false;
        PM.transform.GetComponent<SpriteRenderer>().enabled = true;
        PM.transform.GetComponent<PacMan>().MoveToStartingPostion();

        yield return new WaitForSeconds(delay);

        Restart();
    }

    public void Restart()
    {
        GameObject PM = GameObject.Find("Pacman");
        PM.transform.GetComponent<PacMan>().Restart();

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject o in ghosts)
        {
            o.transform.GetComponent<Ghost>().Restart();
        }

        transform.GetComponent<AudioSource>().clip = backgroundNormal;
        transform.GetComponent<AudioSource>().Play(); 

        didStartDeath = false;
    }
}
