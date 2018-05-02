using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerManager))]
public class Game : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] GameObject m_defaultMap;
    [SerializeField] Dialog m_dialog;
    [SerializeField] Button m_endTurnButton;
    [SerializeField] Text m_actionsRemaining;

    #endregion

    #region Private Fields

    static Game _instance;
    const float _ColorConvert = 255;
    static Tuple<PlayerKind, Color>[] _defaultPlayerData =
    {
        Tuple.Create(PlayerKind.Human,new Color(205 / _ColorConvert, 0, 0)), // #CD0000
        Tuple.Create(PlayerKind.Human, new Color(177 / _ColorConvert,0, 240 / _ColorConvert)), // #B100F0
        Tuple.Create(PlayerKind.Human, new Color(205 / _ColorConvert, 205 / _ColorConvert, 0)), // #CDCD00
        Tuple.Create(PlayerKind.Human, new Color(0, 205 / _ColorConvert, 0)), // #00CD00
        Tuple.Create(PlayerKind.AI, new Color(1, 1, 1)) // #FFFFFF
    };

    bool _processEvents = true;
    Map _map;
    PlayerManager _players;
    int _currentPlayerId;
    /// <summary>
    /// Item1: Eliminated.
    /// Item2: Eliminator.
    /// </summary>
    Queue<Tuple<Player, Player>> _eliminatedPlayers = new Queue<Tuple<Player, Player>>();
    bool _eliminatedPlayerRoutineActive;

    #endregion

    #region Private Properties

    bool ProcessEvents => _processEvents; // in case we need to have other constraints

    #endregion

    #region Public Properties

    /// <summary>
    /// The current game instance.
    /// </summary>
    public static Game Instance => _instance;

    /// <summary>
    /// The default player data.
    /// Indexes 0-3 are the default players, index 4 is the neutral player.
    /// </summary>
    /// <value>The default player data.</value>
    public static Tuple<PlayerKind, Color>[] DefaultPlayerData => _defaultPlayerData;

    /// <summary>
    /// The data to set up the game with.
    /// </summary>
    public static IEnumerable<Tuple<PlayerKind, Color>> PlayerSetupData { get; set; } = null;

    /// <summary>
    /// The map to load into the game. If null, the default is used.
    /// </summary>
    /// <value>The map to load.</value>
    public static GameObject MapToLoad { get; set; } = null;

    /// <summary>
    /// The memento to restore when the game load up.
    /// </summary>
    public static SerializableGame MementoToRestore { get; set; } = null;

    /// <summary>
    /// The reward to give to the player.
    /// </summary>
    /// <remarks>
    /// Only not null upon minigame compeletion, meaning that if it isn't null,
    /// the system will assume that a minigame justs occured, and will act
    /// accordingly.
    /// </remarks>
    public static Action<Game, Dialog> MinigameRewardApply { get; set; } = null;

    /// <summary>
    /// The current game map.
    /// </summary>
    public Map Map => _map;

    /// <summary>
    /// The players in the game.
    /// </summary>
    public PlayerManager Players => _players;

    /// <summary>
    /// Gets the current player.
    /// </summary>
    public Player CurrentPlayer => Players[_currentPlayerId];

    /// <summary>
    /// Whether the End Turn button is enabled.
    /// </summary>
    public bool EndTurnButtonEnabled
    {
        get { return m_endTurnButton.interactable; }
        set { m_endTurnButton.interactable = value; }
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the player's turn starts.
    /// Sender is the player who raised the event.
    /// </summary>
    public event EventHandler OnPlayerTurnStart;

    /// <summary>
    /// Raised when the player's turn ends.
    /// Sender is the player who raised the event.
    /// </summary>
    public event EventHandler OnPlayerTurnEnd;

    /// <summary>
    /// Raised when a player performs an action (actions are as specified by
    /// <see cref="Player.OnActionPerformed"/>).
    /// Sender is the player who raised the event.
    /// </summary>
    public event EventHandler OnPlayerActionPerformed;

    /// <summary>
    /// Raised when a player is eliminated from the game.
    /// Sender is the player who was eliminated.
    /// </summary>
    public event EventHandler<EliminatedEventArgs> OnPlayerEliminated;

    /// <summary>
    /// Raised when a sector is captured.
    /// Sender is the sector who raised the event.
    /// </summary>
    public event EventHandler<UpdateEventArgs<Player>> OnSectorCaptured;

    /// <summary>
    /// Raised when a unit moves.
    /// Sender is the unit who moved.
    /// </summary>
    public event EventHandler<UpdateEventArgs<Sector>> OnUnitMove;

    /// <summary>
    /// Raised when a unit dies.
    /// Sender is the unit who died.
    /// </summary>
    public event EventHandler<EliminatedEventArgs> OnUnitDeath;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new game.
    /// </summary>
    public void Init()
    {
        // set up players
        Players.InitPlayers(PlayerSetupData ?? DefaultPlayerData.Take(4));
        PlayerSetupData = null;

        // initialize the map and allocate players to landmarks
        InitMap();

        // init starting player
        _currentPlayerId = 0;
        CurrentPlayer.Gui.IsActive = true;
        CurrentPlayer.ProcessTurnStart();

        // update GUIs
        UpdateGUI();

        // start up script
        enabled = true;
    }

    /// <summary>
    /// Initialize all sectors, allocate players to landmarks, and spawn units.
    /// </summary>
    public void InitMap()
    {
        LoadMapObject();

        // ensure there are at least as many landmarks as players
        if (Map.LandmarkedSectors.Count() < Players.Count)
            throw new InvalidOperationException("Not enough landmarks for players to start on");

        // randomly allocate sectors to players
        foreach (Player player in Players)
        {
            Sector selected = Map.LandmarkedSectors.Random(s => s.Owner == null);
            selected.Owner = player;
        }

        // allocate Pro-Vice Chancellor
        Map.AllocatePVC();
    }

    internal void LoadMapObject()
    {
        if (_map != null)
            throw new InvalidOperationException(); // only allow loading map once
        GameObject mapGo = MapToLoad ?? m_defaultMap;
        _map = Instantiate(mapGo).GetComponent<Map>();
    }

    #endregion

    #region Serialization

    public SerializableGame CreateMemento()
    {
        return new SerializableGame
        {
            processEvents = _processEvents,
            map = Map.CreateMemento(),
            playerManager = Players.CreateMemento(),
            currentPlayerId = _currentPlayerId
        };
    }

    public void RestoreMemento(SerializableGame memento)
    {
        _processEvents = memento.processEvents;
        // load map so we have access to actual sectors
        LoadMapObject(); // just load map object using default system for now
        // restore all the players
        Players.RestoreMemento(memento.playerManager);
        _currentPlayerId = memento.currentPlayerId;
        // restore all the sectors
        Map.RestoreMemento(memento.map);

        enabled = true;
        EndTurnButtonEnabled = true;

        if (MinigameRewardApply != null)
            // if a minigame reward exists, then apply it to the player
            StartCoroutine(ApplyReward());

        CurrentPlayer.Gui.IsActive = true;
        UpdateGUI();
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _players = gameObject.GetComponent<PlayerManager>();
        }
        else if (_instance != this)
            Destroy(gameObject);
    }

    void OnEnable()
    {
        foreach (Player player in Players)
        {
            player.OnTurnStart += Player_OnTurnStart;
            player.OnTurnEnd += Player_OnTurnEnd;
            player.OnActionPerformed += Player_OnActionPerformed;
        }
        foreach (Sector sector in Map.Sectors)
        {
            sector.OnClick += Sector_OnClick;
            sector.OnCaptured += Sector_OnCaptured;
            sector.OnUnitMove += Sector_OnUnitMove;
            sector.OnUnitDeath += Sector_OnUnitDeath;
        }
    }

    void OnDisable()
    {
        foreach (Player player in Players)
        {
            player.OnTurnStart -= Player_OnTurnStart;
            player.OnTurnEnd -= Player_OnTurnEnd;
            player.OnActionPerformed -= Player_OnActionPerformed;
        }
        foreach (Sector sector in Map.Sectors)
        {
            sector.OnClick -= Sector_OnClick;
            sector.OnCaptured -= Sector_OnCaptured;
            sector.OnUnitMove -= Sector_OnUnitMove;
            sector.OnUnitDeath -= Sector_OnUnitDeath;
        }
    }

    #endregion

    #region Handlers

    void Player_OnTurnStart(object sender, EventArgs e)
    {
        if (!ProcessEvents)
            return;

        OnPlayerTurnStart?.Invoke(sender, e);
        UpdateGUI();
    }

    void Player_OnTurnEnd(object sender, EventArgs e)
    {
        if (!ProcessEvents)
            return;

        OnPlayerTurnEnd?.Invoke(sender, e);
        Players.ToNextPlayer(ref _currentPlayerId);
        CurrentPlayer.ProcessTurnStart();
    }

    void Player_OnActionPerformed(object sender, EventArgs e)
    {
        if (!ProcessEvents)
            return;

        OnPlayerActionPerformed?.Invoke(sender, e);
        UpdateGUI();
    }

    void Sector_OnClick(object sender, EventArgs e)
    {
        if (!ProcessEvents)
            return;

        CurrentPlayer.ProcessSectorClick((Sector)sender);
    }

    void Sector_OnCaptured(object sender, UpdateEventArgs<Player> e)
    {
        if (!ProcessEvents)
            return;

        OnSectorCaptured?.Invoke(sender, e);
        if (e.NewValue != null && (e.OldValue?.IsEliminated ?? false))
            // if the old player is eliminated, it means that this sector was
            // the last one keeping them in the game
            PlayerEliminated(e.OldValue, e.NewValue);
        UpdateGUI();
    }

    void Sector_OnUnitMove(object sender, UpdateEventArgs<Sector> e)
    {
        if (!ProcessEvents)
            return;

        OnUnitMove?.Invoke(sender, e);
    }

    void Sector_OnUnitDeath(object sender, EliminatedEventArgs e)
    {
        if (!ProcessEvents)
            return;

        OnUnitDeath?.Invoke(sender, e);
        Player owner = ((Unit)sender).Owner;
        if (owner.IsEliminated)
            PlayerEliminated(owner, e.Eliminator);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Toggles the Save and quit dialog.
    /// </summary>
    public void ToggleSaveQuitMenu()
    {
        m_dialog.SetDialogType(DialogType.SaveQuit);
        m_dialog.Toggle();
    }

    /// <summary>
    /// Attempts to queue up the player elimination to show the dialog.
    /// </summary>
    /// <param name="eliminated">Eliminated player.</param>
    /// <param name="eliminator">Player did the elimination.</param>
    internal void PlayerEliminated(Player eliminated, Player eliminator)
    {
        if (!_eliminatedPlayers.Select(p => p.Item1).Contains(eliminated))
            _eliminatedPlayers.Enqueue(Tuple.Create(eliminated, eliminator));
        if (!_eliminatedPlayerRoutineActive)
            StartCoroutine(PlayerEliminatedInternal());
    }

    /// <summary>
    /// Used to display the elimination dialog.
    /// </summary>
    /// <remarks>
    /// This is a coroutine in order to prevent the same player from being
    /// displayed twice in a row.
    /// </remarks>
    IEnumerator PlayerEliminatedInternal()
    {
        _eliminatedPlayerRoutineActive = true;
        while (_eliminatedPlayers.Count > 0)
        {
            Tuple<Player, Player> elim = _eliminatedPlayers.Peek();
            Player eliminated = elim.Item1;
            Player eliminator = elim.Item2;

            OnPlayerEliminated?.Invoke(eliminated, new EliminatedEventArgs(eliminator));
            foreach (Sector conqueredSector in eliminated.OwnedSectors)
                conqueredSector.Owner = null;
            Player winner = Players.Winner;
            if (winner != null)
                EndGame(winner);
            else
            {
                m_dialog.SetDialogType(DialogType.PlayerEliminated);
                m_dialog.SetDialogData(eliminated.ToString());
                m_dialog.Show();
                if (CurrentPlayer == eliminated)
                    CurrentPlayer.EndTurn();
            }
            while (m_dialog.IsShown)
                yield return null;
            // dequeue at end to prevent re-adding
            _eliminatedPlayers.Dequeue();
        }
        _eliminatedPlayerRoutineActive = false;
    }

    /// <summary>
    /// Ends the current player's turn.
    /// </summary>
    public void EndCurrentTurn() => CurrentPlayer.EndTurn();

    /// <summary>
    /// Called when the game is over.
    /// Displays a Dialog saying which player has won and allows the player to
    /// quit the game or restart the game.
    /// </summary>
    internal void EndGame(Player winner)
    {
        _processEvents = false;

        m_dialog.SetDialogType(DialogType.EndGame);
        m_dialog.SetDialogData(winner.ToString());
        m_dialog.Show();
        Debug.Log("GAME FINISHED");
    }

    /// <summary>
    /// Updates the player GUIs and the Actions remaining label.
    /// </summary>
	public void UpdateGUI()
    {
        Players.UpdateGUIs();
        m_actionsRemaining.text = (CurrentPlayer.Stats.Actions - CurrentPlayer.ActionsPerformed).ToString();
    }

    /// <summary>
    /// Triggers the minigame with the given player who will receive the reward.
    /// </summary>
    public void TriggerMinigame() => StartCoroutine(TriggerMinigameInternal());

    IEnumerator TriggerMinigameInternal()
    {
        yield return null; // allow events to propagate
        if (!_processEvents)
            yield break; // if process events is disabled, then we don't want to trigger the minigame
        MementoToRestore = CreateMemento(); // save game into static var
        AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Minigame"); // trigger scene change
        while (!asyncOp.isDone)
            // wait for scene change completion
            yield return null;
    }

    /// <summary>
    /// Allocates a reward to the player when they complete the mini game.
    /// </summary>
    internal IEnumerator ApplyReward()
    {
        yield return new WaitForSeconds(1.5f);
        MinigameRewardApply(this, m_dialog); // apply reward
        UpdateGUI(); // update GUI
        Debug.Log(string.Format("{0} won the minigame, effect was applied", CurrentPlayer));
        MinigameRewardApply = null; // clear reward

        // reset PVC allocation
        Map.ResetPVCAllocateWait();
    }

    #endregion
}
