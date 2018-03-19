using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarController : MonoBehaviour
{
    #region Unity Bindings

    public float speed;

    #endregion

    #region Private Fields

    static bool stopped = false;

    #endregion

    #region MonoBehvaiour

    void Update()
    {
        if (!stopped)
            transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
    }

    #endregion

    #region Helper Methods

    public static void Stop()
    {
        stopped = true;
    }

    public static void Reset()
    {
        stopped = false;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    #endregion
}
