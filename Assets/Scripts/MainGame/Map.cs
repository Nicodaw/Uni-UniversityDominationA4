using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Stores the data for the game map.
/// </summary>
public class Map : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] Sector[] m_sectors;

    #endregion

    #region Private Fields

    const int MaxPVCAllocateWait = 2; // the number of complete turn cycle to wait

    int _pvcAllocateWait = -1;
    int? _lastPvcSector;

    #endregion

    #region Public Properties

    /// <summary>
    /// The sectors on the map.
    /// </summary>
    public Sector[] Sectors => m_sectors;

    /// <summary>
    /// The sectors that contain landmarks.
    /// </summary>
    public IEnumerable<Sector> LandmarkedSectors => Sectors.Where(s => s.Landmark != null);

    public int PVCAllocateWait => _pvcAllocateWait;

    #endregion

    #region Serialization

    public SerializableMap CreateMemento()
    {
        return new SerializableMap
        {
            pvcAllocateWait = _pvcAllocateWait,
            lastPvcSector = _lastPvcSector,
            // create array of serialized sectors
            sectors = Sectors.Select(s => s.CreateMemento()).ToArray()
        };
    }

    public void RestoreMemento(SerializableMap memento)
    {
        _pvcAllocateWait = memento.pvcAllocateWait;
        _lastPvcSector = memento.lastPvcSector;
        for (int i = 0; i < Sectors.Length; i++)
            Sectors[i].RestoreMemento(memento.sectors[i]);
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        // init sectors here so they are ready for usage by Start methods
        for (int i = 0; i < m_sectors.Length; i++)
            m_sectors[i].Init(i);
    }

    void OnEnable()
    {
        Game.Instance.OnPlayerTurnStart += Game_OnPlayerTurnStart;
    }

    void OnDisable()
    {
        Game.Instance.OnPlayerTurnStart -= Game_OnPlayerTurnStart;
    }

    #endregion

    #region Handlers

    void Game_OnPlayerTurnStart(object sender, EventArgs e) => ProcessPVCAllocateDec();

    #endregion

    #region Helper Methods

    void ProcessPVCAllocateDec()
    {
        if (_pvcAllocateWait == 0)
        {
            AllocatePVC();
            _pvcAllocateWait--; // make sure we don't re-allocate unnecessarily
        }
        else if (_pvcAllocateWait > 0)
        {
            _pvcAllocateWait--;
            Debug.LogFormat("pvc allocation wait now {0}", _pvcAllocateWait);
        }
    }

    /// <summary>
    /// Resets the PVC allocation wait timer.
    /// The value is reset to the <see cref="MaxPVCAllocateWait"/> number of
    /// complete turn cycles (w/ adjustment). This means that if it is 1, then all players
    /// have to complete their turn once, and the player who got found the PVC
    /// has to complete their turn twice, before it is reallocated. This is to give
    /// an advantage to the other players.
    /// </summary>
    public void ResetPVCAllocateWait(bool currentPlayerIsRewardee)
    {
        _lastPvcSector = Sectors.First(s => s.HasPVC).Id;
        Sectors[_lastPvcSector.Value].Stats.RemoveEffect<EffectImpl.PVCEffect>();
        Debug.Log("Previous sector de-allocated");
        _pvcAllocateWait = Game.Instance.Players.Count * MaxPVCAllocateWait;
        // if the minigame was triggered on the player's last action, then
        // we would have missed the turn start event, so we need to do it manually
        if (!currentPlayerIsRewardee)
            ProcessPVCAllocateDec();
        else
            Debug.LogFormat("pvc allocate wait set to {0}", _pvcAllocateWait);
    }

    /// <summary>
    /// Randomly allocates the PVC.
    /// </summary>
    public void AllocatePVC()
    {
        Sector lastPvcSector = _lastPvcSector.HasValue ? Sectors[_lastPvcSector.Value] : null;
        Sector randomSector = Sectors.Random(s => s.AllowPVC && s != lastPvcSector);

        randomSector.Stats.ApplyEffect(new EffectImpl.PVCEffect());
        Debug.Log("Allocated PVC at " + randomSector);
    }

    #endregion
}
