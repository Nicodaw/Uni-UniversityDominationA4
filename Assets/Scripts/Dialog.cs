﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class created by Jack (01/02/2018)
public class Dialog : MonoBehaviour
{

    public GameObject texture;

    private DialogType type;

    public enum DialogType
    {
        EndGame, PlayerElimated, SaveQuit
    }

    /// <summary>
    /// 
    /// Sets up the dialog in the format of the passed dialog type
    /// 
    /// </summary>
    /// <param name="type">The type that this dialog should be set up in the form of</param>
    public void SetDialogType(DialogType type)
    {
        // Updates the dialog with the different buttons needed for each mode
        this.type = type;
        switch (type)
        {
            case DialogType.EndGame:
                texture.transform.GetChild(0).GetComponent<Text>().text = "GAME OVER!"; // Hides the unnecessary buttons and shows the others
                texture.transform.GetChild(2).gameObject.SetActive(false);
                texture.transform.GetChild(3).gameObject.SetActive(true);
                texture.transform.GetChild(4).gameObject.SetActive(true);
                texture.transform.GetChild(5).gameObject.SetActive(false);
                break;
            case DialogType.PlayerElimated:
                texture.transform.GetChild(0).GetComponent<Text>().text = "ELIMINATED!";
                texture.transform.GetChild(2).gameObject.SetActive(true);
                texture.transform.GetChild(3).gameObject.SetActive(false);
                texture.transform.GetChild(4).gameObject.SetActive(false);
                texture.transform.GetChild(5).gameObject.SetActive(false);
                break;
            case DialogType.SaveQuit:
                texture.transform.GetChild(0).GetComponent<Text>().text = "OPTIONS";
                texture.transform.GetChild(1).GetComponent<Text>().text = "";
                texture.transform.GetChild(2).gameObject.SetActive(false);
                texture.transform.GetChild(3).gameObject.SetActive(false);
                texture.transform.GetChild(4).gameObject.SetActive(true);
                texture.transform.GetChild(5).gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 
    /// Sets the players name in this dialog
    /// 
    /// </summary>
    /// <param name="name">The pla yer's name who this dialog refers to</param>
    public void SetPlayerName(string name)
    {

        switch (type)
        {
            case DialogType.EndGame:
                texture.transform.GetChild(1).GetComponent<Text>().text = name + " WON!";
                break;
            case DialogType.PlayerElimated:
                texture.transform.GetChild(1).GetComponent<Text>().text = name + " was eliminated";
                break;
        }
    }

    /// <summary>
    /// 
    /// Displays this dialog
    /// 
    /// </summary>
    public void Show()
    {
        texture.SetActive(true);
    }

    /// <summary>
    /// 
    /// Closes this dialog
    /// 
    /// </summary>
    public void Close()
    {
        Debug.Log("Closing");
        texture.SetActive(false);
    }

    /// <summary>
    /// 
    /// Changes to the previous sce3ne
    /// 
    /// </summary>
    public void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// 
    /// Loads the main menu scene
    /// 
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

}