using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        SoundManager.Instance.PlaySingle(Sound.UIButtonClickSound);
        SoundManager.Instance.ToggleMusic();
    }

    public void PlaySecondary()
    {
        // tmp method to support 3 player start game
        Game.PlayerSetupData = Game.DefaultPlayerData.Take(3).Concat(new[] { Game.DefaultPlayerData[4] });
        Play();
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
        SoundManager.Instance.PlaySingle(Sound.UIButtonClickSound);
        SoundManager.Instance.ToggleMusic();
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
	public void Quit()
    {
        Application.Quit();
        SoundManager.Instance.PlaySingle(Sound.UIButtonClickSound);
    }

    #endregion
}
