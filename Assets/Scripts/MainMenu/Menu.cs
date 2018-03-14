using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    #region Helper Methods

    /// <summary>
    /// Starts a new game.
    /// </summary>
    /// <param name="neutralPlayer">True if neutal player should be in the game, else false.</param>
    public void Play(bool neutralPlayer)
    {
        PlayerPrefs.SetInt("_gamemode", neutralPlayer ? 1 : 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Loads the saved game.
    /// </summary>
    public void PlayLoad()
    {
        if (!SavedGame.SaveExists("test1")) return;
        PlayerPrefs.SetInt("_gamemode", 2);
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
	public void Quit()
    {
        Application.Quit();
    }

    #endregion
}
