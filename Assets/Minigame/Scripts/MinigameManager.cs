using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    #region Unity Bindings

    public static MinigameManager instance;
    public GameObject birdPrefab;
    public GameObject pillarPrefab;
    public GameObject cloudPrefab;
    public GameObject startPos;
    public GameObject uiOverlay, loseOverlay, minCoinSpawn, maxCoinSpawn;
    [Multiline]
    public string initialText;
    [Multiline]
    public string winText;
    [Multiline]
    public string loseText;
    public Vector3 minPos;
    public Vector3 maxPos;
    public float maxScore = 10;

    #endregion

    #region Private Fields

    List<GameObject> pillars = new List<GameObject>();
    List<GameObject> clouds = new List<GameObject>();
    bool gameOver = false;
    BirdController birdComponent;

    #endregion

    #region Private Properties

    bool WonGame
    {
        get { return birdComponent.GetScore() >= maxScore; }
    }

    #endregion

    #region Public Properties

    public static int CurrentPlayerId { get; set; }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        PillarController.Reset();
    }

    /// <summary>
    /// Starts the mini game
    /// </summary>
    void Start()
    {
        GameObject goBird = Instantiate(birdPrefab, Vector3.zero, Quaternion.identity, startPos.transform);
        goBird.transform.localPosition = Vector3.zero;
        goBird.transform.localRotation = Quaternion.identity;
        birdComponent = goBird.GetComponent<BirdController>();
        StartCoroutine(Countdown());
    }

    /// <summary>
    /// Updates the mini game
    /// </summary>
    void Update()
    {
        Vector3 pos = birdComponent.transform.position;
        if (!(pos.x > minPos.x && pos.x < maxPos.x && pos.y > minPos.y && pos.y < maxPos.y))
        {
            birdComponent.Pause();
        }
        if (WonGame || birdComponent.IsDead())
        {
            if (gameOver)
                return;
            gameOver = true;
            StartCoroutine(EndGame(WonGame));
        }
        if (birdComponent.IsPaused() == false)
        {
            SpawnPillar();
        }
        SpawnCloud();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Ends the game and reloads the main game scene
    /// </summary>
    /// <param name="won">True if the player won the minigame else false</param>
    IEnumerator EndGame(bool won)
    {
        loseOverlay.SetActive(true);
        loseOverlay.transform.GetChild(0).GetComponent<Text>().text = string.Format(won ? winText : loseText, birdComponent.GetScore());
        int reward = Mathf.FloorToInt((birdComponent.GetScore() + 1f) / 2f);
        Game.MinigameReward = new EffectImpl.MinigameRewardEffect(CurrentPlayerId, reward, reward);
        yield return new WaitForSeconds(3);
        //SceneManager.LoadScene("MainGame");
    }

    /// <summary>
    /// Updates the countdown berfore the minigame starts
    /// </summary>
    IEnumerator Countdown()
    {
        for (int i = 3; i >= 0; i--)
        {
            uiOverlay.transform.GetChild(0).GetComponent<Text>().text = string.Format(initialText, maxScore) + "\n" + i;
            yield return new WaitForSeconds(1);
        }
        uiOverlay.SetActive(false);
        birdComponent.UnPause();
    }

    /// <summary>
    /// Returns a vector with a random x and z value, y = 0
    /// x and z values are in range specified by min and max
    /// total length of vector is less than range
    /// </summary>
    /// <param name="min">min x and z value</param>
    /// <param name="max">max x and z value</param>
    /// <param name="position">where to measure distance from</param>
    /// <param name="range">Max length of vector</param>
    /// <returns></returns>
    Vector3 GetRandomValueBetweenWhilstAvoiding(Vector3 min, Vector3 max, Vector3 position, float range)
    {
        Vector3 newPos = position;
        while (Vector3.Distance(newPos, position) <= range)
        {
            newPos = new Vector3(Random.value * (max.x - min.x) + min.x, 0, Random.value * (max.z - min.z) + min.z);
        }
        return newPos;
    }

    /// <summary>
    /// Adds a pilar to the minigame scene
    /// </summary>
    public void SpawnPillar()
    {
        // Random.value * (max - min) + min
        if (pillars.Count == 0 || pillars[pillars.Count - 1].transform.position.x <= 1.2)
        {
            if (pillars.Count < maxScore)
            {
                pillars.Add(Instantiate(pillarPrefab, new Vector3(10f, -(Random.value * 2.2f + 2.3f), 5f), Quaternion.Euler(-90, 0, 0)));
            }
        }
    }

    /// <summary>
    /// Adds a cloud to the minigame scene
    /// </summary>
    public void SpawnCloud()
    {
        if (Random.value < 0.005f)
        {
            clouds.Add(Instantiate(cloudPrefab, new Vector3(10.5f, Random.value * 3 + 1.5f, Random.value * 0.2f - 0.1f + 6.21f), Quaternion.Euler(90, 180, 0)));
            clouds[clouds.Count - 1].GetComponent<PillarController>().SetSpeed(Random.value * 2 + 3);
        }
    }

    #endregion
}
