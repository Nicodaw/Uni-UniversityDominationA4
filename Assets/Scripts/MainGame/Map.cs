using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data for the game map.
/// </summary>
public class Map : MonoBehaviour
{
    #region Unity Bindings

    [UnityEngine.Serialization.FormerlySerializedAs("sectors")]
    [SerializeField]
    Sector[] m_sectors;

    public Game game;

    #endregion

    #region Public Properties

    public Sector[] Sectors => m_sectors;

    #endregion
}
