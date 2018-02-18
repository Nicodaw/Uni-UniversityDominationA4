﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SavedGameTest {

    /// <summary>
    /// Check when saving and loading a game, the game object is the same
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator SaveLoadGame()
    {
        Game game = new Game();
        game.Initialize(true);
        SavedGame.Save("saveTest", game);
        Game savedGame = new Game();
        savedGame.Initialize(SavedGame.Load("saveTest"));
        Assert.AreSame(game, savedGame);

        yield return null;
    }
    
    /// <summary>
    /// Check if an incorrect filename is told to load, null is returned
    /// </summary>
    [UnityTest]
    public IEnumerator Load()
    {
        Assert.IsNull(SavedGame.Load(""));

        yield return null;
    }
}
