using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Director.choice == 5)
        {
            Director.choice = 0;
            Director.gold = 10;
            Director.playerCreatures.Clear();
            Director.allCreatures.Clear();
            Director.state = "shop";
            Director.level = 0;
            Director.logs.Clear();
            Director.logtimers.Clear();
            Director.moneylogs.Clear();
            Director.moneylogtimers.Clear();
            SceneManager.LoadScene("game");
        }
        if (Director.choice == 6)
        {
            Application.Quit();
        }
    }
}
