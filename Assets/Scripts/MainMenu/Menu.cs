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
    public void Play()
    {
        SceneManager.LoadScene("MainGame");
    }

    /// <summary>
    /// Loads the saved game.
    /// </summary>
    public void PlayLoad()
    {
        // assume slot 0 for now
        if (!SaveManager.SaveExists(0))
            return;
        Game.MementoToRestore = SaveManager.LoadGame(0);
        SceneManager.LoadScene("MainGame");
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
