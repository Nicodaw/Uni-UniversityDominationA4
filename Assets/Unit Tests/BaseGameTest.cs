#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BaseGameTest
{
    #region Test Environment

    /// <summary>
    /// The main game object.
    /// </summary>
    protected Game game => Game.Instance;

    /// <summary>
    /// The current player manager.
    /// </summary>
    protected PlayerManager Players => game.Players;

    protected Map map => game.Map;

    #endregion

    #region Test Management

    [SetUp]
    public void SetUp()
    {
        // load test scene
        SceneManager.LoadScene("TestScene", LoadSceneMode.Additive);
    }

    [TearDown]
    public void TearDown()
    {
        // destroy main scene object
        UnityEngine.Object.Destroy(GameObject.Find("Scene"));
        GameObject mapGobj = GameObject.Find("DefaultMap(Clone)");
        if (mapGobj != null)
            UnityEngine.Object.Destroy(mapGobj);
        SceneManager.UnloadSceneAsync("TestScene");
    }

    protected IEnumerable<Tuple<PlayerKind, Color>> DefaultPlayerInitData => Game.DefaultPlayerData.Take(4);

    protected IEnumerable<Tuple<PlayerKind, Color>> AiPlayerInitData => Game.DefaultPlayerData.Take(3).Concat(new[] { Game.DefaultPlayerData[4] });

    protected void DefaultPlayerInit() => Players.InitPlayers(DefaultPlayerInitData);

    protected void AiPlayerInit() => Players.InitPlayers(AiPlayerInitData);

    /// <summary>
    /// Initializes the game object with default player data.
    /// </summary>
    protected void DefGameInit()
    {
        Game.PlayerSetupData = DefaultPlayerInitData;
        game.Init();
    }

    /// <summary>
    /// Initializes the game's map object and the default player data.
    /// </summary>
    protected void DefMapInit()
    {
        DefaultPlayerInit();
        game.InitMap();
    }

    protected void SpawnAllPlayerUnits()
    {
        foreach (Player player in Players)
            player.SpawnUnits();
    }

    /// <summary>
    /// Instantiates a new unit at the given sector.
    /// </summary>
    /// <returns>The new unit.</returns>
    /// <param name="sector">The sector to spawn the unit at.</param>
    /// <param name="player">The player to take the unit prefab from.</param>
    protected Unit InitUnit(Sector sector, int player = 0)
    {
        Players[player].SpawnUnitAt(sector);
        return sector.Unit;
    }

    #endregion

    #region Test Data

    protected class CustomEffect : Effect
    {
        public UnityEngine.Object appliedObject => (UnityEngine.Object)AppliedPlayer ?? (UnityEngine.Object)AppliedSector ?? (UnityEngine.Object)AppliedUnit;

        public EffectManager AppliedManager => Manager;

        public int? attackBonus;
        public override int? AttackBonus => attackBonus;

        public int? defenceBonus;
        public override int? DefenceBonus => defenceBonus;

        public int? actionBonus;
        public override int? ActionBonus => actionBonus;

        public bool? traversable;
        public override bool? Traversable => traversable;

        public int? moveRangeBonus;
        public override int? MoveRangeBonus => moveRangeBonus;

        public int? levelCapBonus;
        public override int? LevelCapBonus => levelCapBonus;

        protected override void ApplyToPlayer()
        { }

        protected override void ApplyToSector()
        { }

        protected override void ApplyToUnit()
        { }

        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            throw new NotImplementedException();
        }
    }

    protected class CustomSecondaryEffect : Effect
    {
        protected override void ApplyToPlayer()
        { }

        protected override void ApplyToSector()
        { }

        protected override void ApplyToUnit()
        { }

        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
#endif
