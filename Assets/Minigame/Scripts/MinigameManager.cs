using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    #region Unity Bindings

    [Header("Prefabs")]
    [SerializeField] GameObject m_birdPrefab;
    [SerializeField] GameObject m_pillarPrefab;
    [SerializeField] GameObject m_cloudPrefab;

    [Header("Scene Objects")]
    [SerializeField] GameObject m_pillarParent;
    [SerializeField] GameObject m_cloudParent;

    [Header("UI")]
    [SerializeField] GameObject m_startOverlay;
    [SerializeField] Text m_startUiText;
    [SerializeField] GameObject m_loseOverlay;
    [SerializeField] Text m_loseUiText;
    [Multiline]
    [SerializeField] string m_initialText;
    [Multiline]
    [SerializeField] string m_winText;
    [Multiline]
    [SerializeField] string m_loseText;

    [Header("Config")]
    [SerializeField] float m_pillarSpawnWait;
    [SerializeField] float m_cloudSpawnChance;
    [SerializeField] Vector3 m_minPos;
    [SerializeField] Vector3 m_maxPos;

    #endregion

    #region Private Fields

    static MinigameManager _instance;

    bool _active;
    float _lastPillarSpawnTime;

    #endregion

    #region Public Properties

    /// <summary>
    /// The current instance.
    /// </summary>
    public static MinigameManager Instance => _instance;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            m_startUiText.text = string.Format(m_initialText,
                                               GameObject.Find("Bird").GetComponent<BirdController>().MaxScore);
        }
        else if (_instance != this)
            Destroy(this);
    }

    void Start()
    {
        SoundManager.Instance.PlayMusic(Sound.MiniGameMusic);
    }

    void Update()
    {
        if (_active)
            SpawnPillar();
        SpawnCloud();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Starts the game running.
    /// </summary>
    public void StartGame()
    {
        m_startOverlay.SetActive(false);
        _active = true;
        _lastPillarSpawnTime = 0;
        PillarController.Reset();
    }

    /// <summary>
    /// Ends the game and reloads the main game scene
    /// </summary>
    /// <param name="won">Whether the player won the minigame.</param>
    /// <param name="score">The score the player got.</param>
    public void EndGame(bool won, int score) => StartCoroutine(EndGameInternal(won, score));

    IEnumerator EndGameInternal(bool won, int score)
    {
        _active = false;
        m_loseOverlay.SetActive(true);
        m_loseUiText.text = string.Format(won ? m_winText : m_loseText, score);
        int cardCount = Mathf.FloorToInt((score + 2f) / 4f);
        Game.MinigameRewardApply = (game, dialog) =>
        {
            // apply reward
            Effect[] cards = new Effect[cardCount];
            for (int i = 0; i < cardCount; i++)
                cards[i] = CardFactory.GetRandomEffect(CardTier.Tier1);
            game.CurrentPlayer.Cards.AddCards(cards);

            // display dialog
            dialog.SetDialogType(DialogType.ShowText);
            dialog.SetDialogData("REWARD!", string.Format("Well done, you have\naquired {0} cards", cardCount));
            dialog.Show();
        };
        yield return new WaitForSeconds(3);
        Debug.Log("Switching back to main game");
        SceneManager.LoadScene("MainGame");
    }

    /// <summary>
    /// Adds a pilar to the minigame scene
    /// </summary>
    void SpawnPillar()
    {
        if (Time.time - _lastPillarSpawnTime >= m_pillarSpawnWait)
        {
            _lastPillarSpawnTime = Time.time;
            Instantiate(m_pillarPrefab, m_pillarParent.transform);
        }
    }

    /// <summary>
    /// Adds a cloud to the minigame scene
    /// </summary>
    void SpawnCloud()
    {
        if (Random.value < m_cloudSpawnChance)
            Instantiate(m_cloudPrefab, m_cloudParent.transform);
    }

    #endregion
}
