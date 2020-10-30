using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            SceneManager.LoadScene("Level1");
        }
    }
}
