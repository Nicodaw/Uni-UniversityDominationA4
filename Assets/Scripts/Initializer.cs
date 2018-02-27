using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Initializer : MonoBehaviour {

    public Game game;
 
    /// <summary>
    /// start the game 
    /// </summary>
    void Start()
    {
        switch (PlayerPrefs.GetInt("_gamemode", 0))
        {
            case 0:
                game.Initialize(false); // start new game with no neutral player
                break;
            case 1:
                game.Initialize(true); // start new game with neutal player
                break;
            case 2:
                game.Initialize(SavedGame.Load("test1")); // load saved game
                break;
            case 3:
                game.Initialize(SavedGame.Load("_tmp")); // load back from the mini-game
                game.GiveReward();
                break;
        }
    }
}