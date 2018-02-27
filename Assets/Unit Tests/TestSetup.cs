using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;

public class TestSetup 
{
    private Game game;
    private Map map;
    private Player[] players;
    private PlayerUI[] gui;
    private GameObject unitPrefab;

    public TestSetup()
    {
        Setup();
    }
    
    private void Setup()
    {
        // initialize the game, map, and players with any references needed
        // the "GameManager" asset contains a copy of the GameManager object
        // in the 4x4 Test, but its script lacks references to players & the map
        game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();

        // the "Map" asset is a copy of the 4x4 Test map, complete with
        // adjacent sectors and landmarks at (0,1), (1,3), (2,0), and (3,2),
        // but its script lacks references to the game & sectors
        map = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Map")).GetComponent<Map>();
                
        
        // the "GUI" asset contains the PlayerUI object for each Player        

        gui = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GUI")).GetComponentsInChildren<PlayerUI>();

        unitPrefab = Resources.Load<GameObject>("UnitCubeP1");

        // the "Scenery" asset contains the camera and light source of the 4x4 Test
        // can uncomment to view scene as tests run, but significantly reduces speed
        //MonoBehaviour.Instantiate(Resources.Load<GameObject>("Scenery"));


    }

    private IEnumerator Hold()
    {
        yield return null;
    }

    public Game Game
    {
        get { return game; }
    }

    public Map GetMap()
    {
        return this.map;
    }

    public Player[] GetPlayers()
    {
        return this.players;
    }

    public PlayerUI[] GetPlayerUIs()
    {
        return this.gui;
    }

    public GameObject UnitPrefab { get { return unitPrefab; } }
    
}