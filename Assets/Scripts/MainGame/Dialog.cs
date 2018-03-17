using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] GameObject m_dialogTitle;
    [SerializeField] GameObject m_dialogInfo;
    [SerializeField] GameObject m_dialogOkay;
    [SerializeField] GameObject m_dialogRestart;
    [SerializeField] GameObject m_dialogQuit;
    [SerializeField] GameObject m_dialogSaveQuit;
    [SerializeField] GameObject m_dialogCloseDialogBtn;

    #endregion

    #region Private fields

    DialogType type;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets whether the dialog is shown.
    /// </summary>
    public bool IsShown => gameObject.activeInHierarchy;

    #endregion

    #region Helper Methods

    /// <summary>
    /// Sets up the dialog in the format of the passed dialog type
    /// </summary>
    public void SetDialogType(DialogType type)
    {
        // Updates the dialog with the different buttons needed for each mode
        this.type = type;
        switch (type)
        {
            case DialogType.EndGame:
                ApplyDialogState("GAME OVER!", false, false, true, true, false, false);
                break;
            case DialogType.PlayerElimated:
                ApplyDialogState("ELIMINATED!", true, true, false, false, false, false);
                break;
            case DialogType.SaveQuit:
                ApplyDialogState("PAUSED", false, false, false, true, true, true, "");
                break;
            case DialogType.ShowText:
                ApplyDialogState("", true, true, false, false, false, false);
                break;

        }
    }

    /// <summary>
    /// Applies the given state to the dialog.
    /// </summary>
    /// <param name="titleText">The title text.</param>
    /// <param name="infoEnabled">Whether the info object is enabled.</param>
    /// <param name="okayEnabled">Whether the okay object is enabled.</param>
    /// <param name="restartEnabled">Whether the restart object is enabled.</param>
    /// <param name="quitEnabled">Whether the quit object is enabled.</param>
    /// <param name="saveQuitEnabled">Whether the save &amp; quit object is enabled.</param>
    /// <param name="closeEnabled">Whether the close object is enabled.</param>
    void ApplyDialogState(string titleText,
                          bool infoEnabled, bool okayEnabled,
                          bool restartEnabled, bool quitEnabled,
                          bool saveQuitEnabled, bool closeEnabled,
                          string infoText = null)
    {
        m_dialogTitle.GetComponent<Text>().text = titleText;
        m_dialogInfo.SetActive(infoEnabled);
        if (infoText != null)
            m_dialogInfo.GetComponent<Text>().text = infoText;
        m_dialogOkay.SetActive(okayEnabled);
        m_dialogRestart.SetActive(restartEnabled);
        m_dialogQuit.SetActive(quitEnabled);
        m_dialogSaveQuit.SetActive(saveQuitEnabled);
        m_dialogCloseDialogBtn.SetActive(closeEnabled);
    }

    /// <summary>
    /// Sets the players name in this dialog.
    /// </summary>
    /// <param name="player">The player's name who this dialog refers to.</param>
    public void SetDialogData(string player)
    {
        switch (type)
        {
            case DialogType.EndGame:
                m_dialogInfo.GetComponent<Text>().text = player + " WON!";
                break;
            case DialogType.PlayerElimated:
                m_dialogInfo.GetComponent<Text>().text = player + "\nwas eliminated";
                break;
        }
    }

    /// <summary>
    /// Creates a dialog with specific header and bidy text.
    /// </summary>
    /// <param name="header">Header text.</param>
    /// <param name="body">Body text.</param>
    public void SetDialogData(string header, string body)
    {
        switch (type)
        {
            case DialogType.ShowText:
                m_dialogTitle.GetComponent<Text>().text = header;
                m_dialogInfo.GetComponent<Text>().text = body;
                break;
        }
    }

    /// <summary>
    /// Displays this dialog
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes this dialog
    /// </summary>
    public void Close()
    {
        Debug.Log("Closing");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Toggle this dialog.
    /// </summary>
    public void Toggle()
    {
        if (IsShown)
            Close();
        else
            Show();
    }

    /// <summary>
    /// Changes to the previous scene
    /// </summary>
    [System.Obsolete("Use direct scene name")]
    public void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    [System.Obsolete("Use direct scene name")]
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    #endregion
}
