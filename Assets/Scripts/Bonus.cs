using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    float lifeLength;
    float currLifeTime;

    void Start()
    {
        lifeLength = Random.Range(9, 10);

        this.name = "bonusitem";

        GameObject.Find("GameManager").GetComponent<GameBoard>().board[14, 13] = this.gameObject;
    }

    void Update()
    {
        if(currLifeTime < lifeLength)
        {
            currLifeTime += Time.deltaTime;
        }

        else
        {
            Destroy(this.gameObject); 
        }
    }
}
