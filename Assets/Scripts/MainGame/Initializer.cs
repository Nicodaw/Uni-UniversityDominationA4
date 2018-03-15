using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Initializer : MonoBehaviour
{
    #region MonoBehaviour

    void Start()
    {
        if (Game.MinigameReward == null)
            Game.Instance.Init();
        else
            ; // restore game
    }

    #endregion
}