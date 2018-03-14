#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public abstract class BaseGameTest
{
    #region Test Environment

    protected Game game;
    protected Map map;
    protected PlayerUI[] gui;

    protected Player[] Players
    {
        get { return game.players; }
    }

    #endregion

    #region Test Management

    const string resourcePath = "Testing/";
    List<GameObject> unitObjects = new List<GameObject>(); // used to keep track of units

    [SetUp]
    public void SetUp()
    {
        // grab main game assets
        game = Object.Instantiate(Resources.Load<GameObject>(resourcePath + "GameManager")).GetComponent<Game>();
        map = Object.Instantiate(Resources.Load<GameObject>(resourcePath + "Map")).GetComponent<Map>();

        // grab GUI asset and extract players UIs
        GameObject mainGui = Object.Instantiate(Resources.Load<GameObject>(resourcePath + "GUI"));
        gui = mainGui.GetComponentsInChildren<PlayerUI>();

        // the "Scenery" asset contains the camera and light source of the 4x4 Test
        // can uncomment to view scene as tests run, but significantly reduces speed
        //MonoBehaviour.Instantiate(Resources.Load<GameObject>("Scenery"));

        // establish references
        game.gameMap = map.gameObject;
        game.actionsRemaining = mainGui.transform.Find("RemainingActionsValue").gameObject.GetComponent<UnityEngine.UI.Text>();
        game.dialog = mainGui.transform.Find("Dialog").GetComponent<Dialog>();

        map.game = game;
        //map.Sectors = map.gameObject.GetComponentsInChildren<Sector>();

        // establish references to a PlayerUI and Game for each player & initialize GUI
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].Gui = gui[i];
            Players[i].Game = game;
            Players[i].Gui.Initialize(Players[i], i + 1);
        }
        game.neutralPlayer.Gui = gui.Last();
        game.neutralPlayer.Game = game;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(game.gameObject);
        Object.Destroy(map.gameObject);
        Object.Destroy(Players[0].transform.parent.gameObject);
        Object.Destroy(gui[0].transform.parent.gameObject);
        foreach (GameObject go in unitObjects)
            if (go != null)
                Object.Destroy(go);
    }

    /// <summary>
    /// Instantiates a new unit and tracks it for tear-down.
    /// </summary>
    /// <returns>The new unit.</returns>
    /// <param name="player">The player to take the unit prefab from.</param>
    protected Unit InitUnit(int player = 0)
    {
        GameObject go = Object.Instantiate(Players[player].UnitPrefab);
        unitObjects.Add(go);
        return go.GetComponent<Unit>();
    }

    /// <summary>
    /// Instantiates the given number of new units and tracks them for tear-down.
    /// This will use the unit prefab in the default player.
    /// </summary>
    /// <returns>The new units.</returns>
    /// <param name="amount">Amount of units to instanciate.</param>
    protected Unit[] InitUnits(int amount)
    {
        Unit[] units = new Unit[amount];
        for (int i = 0; i < amount; i++)
            units[i] = InitUnit();
        return units;
    }

    #endregion
}
#endif
