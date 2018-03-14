using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class created by Jack (01/02/2018)
public class Dialog : MonoBehaviour
{
    #region Unity Bindings

    public GameObject dialogTitle;
    public GameObject dialogInfo;
    public GameObject dialogOkay;
    public GameObject dialogRestart;
    public GameObject dialogQuit;
    public GameObject dialogSaveQuit;
    public GameObject dialogCloseDialogBtn;

    #endregion

    #region Private fields

    DialogType type;

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
        dialogTitle.GetComponent<Text>().text = titleText;
        dialogInfo.SetActive(infoEnabled);
        if (infoText != null)
            dialogInfo.GetComponent<Text>().text = infoText;
        dialogOkay.SetActive(okayEnabled);
        dialogRestart.SetActive(restartEnabled);
        dialogQuit.SetActive(quitEnabled);
        dialogSaveQuit.SetActive(saveQuitEnabled);
        dialogCloseDialogBtn.SetActive(closeEnabled);
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
                dialogInfo.GetComponent<Text>().text = player + " WON!";
                break;
            case DialogType.PlayerElimated:
                dialogInfo.GetComponent<Text>().text = player + "\nwas eliminated";
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
                dialogTitle.GetComponent<Text>().text = header;
                dialogInfo.GetComponent<Text>().text = body;
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
    /// Changes to the previous scene
    /// </summary>
    public void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    #endregion
}