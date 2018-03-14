using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    #region Unity Bindings

    public ResourceType resourceType;
    public int amount = 2;

    #endregion

    #region Public Properties

    public ResourceType Resource { get { return resourceType; } set { resourceType = value; } }
    public int Amount { get { return amount; } set { amount = value; } }

    #endregion
}
