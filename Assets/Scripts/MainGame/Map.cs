using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data for the game map.
/// </summary>
public class Map : MonoBehaviour
{
    [SerializeField]
    Sector[] m_sectors;
    public Game game;

    public Sector[] Sectors => m_sectors;
}
