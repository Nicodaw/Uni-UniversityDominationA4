using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] Toggle m_musicToggle;
    [SerializeField] Toggle m_soundToggle;

    #endregion

    #region MonoBehaviour

    void Start()
    {
        // we have to do all this switching around because the 'isOn' property
        // fired the 'on changed' event, causing the click sound to play
        bool sfxOn = SoundManager.Instance.SoundEffectsPlaying;
        SoundManager.Instance.SoundEffectsPlaying = false;
        m_musicToggle.isOn = SoundManager.Instance.MusicPlaying;
        SoundManager.Instance.SoundEffectsPlaying = sfxOn;
        if (!SoundManager.Instance.SoundEffectsPlaying)
            m_soundToggle.isOn = false;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Starts a new game.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("MainGame");
        SoundManager.Instance.PlaySingle(Sound.UIButtonClickSound);
    }

    /// <summary>
    /// Starts a new game with 3 human and 1 AI player.
    /// </summary>
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
