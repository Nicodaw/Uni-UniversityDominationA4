using System.Collections;
using System.Collections.Generic;
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

    public static MinigameManager Instance => _instance;

    public static int CurrentPlayerId { get; set; }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        SoundManager.Instance.PlayMusic(Sound.MiniGameMusic);
        if (_instance == null)
        {
            _instance = this;
            m_startUiText.text = string.Format(m_initialText,
                                               GameObject.Find("Bird").GetComponent<BirdController>().MaxScore);
        }
        else if (_instance != this)
            Destroy(this);
    }

    void Update()
    {
        if (_active)
            SpawnPillar();
        SpawnCloud();
    }

    #endregion

    #region Helper Methods

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
        int reward = Mathf.FloorToInt((score + 1f) / 2f);
        Game.MinigameReward = new EffectImpl.MinigameRewardEffect(CurrentPlayerId, reward, reward);
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
