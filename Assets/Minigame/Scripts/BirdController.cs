using System;
using System.Collections;
using UnityEngine;

//Added by Jack
[RequireComponent(typeof(Rigidbody))]
public class BirdController : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] Vector3 m_defaultRotation;
    [SerializeField] int m_maxScore;
    [SerializeField] float m_jumpForce;
    [SerializeField] float m_deathTurnTime;
    [SerializeField] Material[] m_states = new Material[2];

    #endregion

    #region Private fields

    MeshRenderer _renderer;
    Rigidbody _rigidbody;
    float _rotX;
    int _score;
    bool _started;
    bool _active;
    bool _done;

    #endregion

    #region Private Properties

    bool JumpFired => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);

    float XRotation
    {
        get { return _rotX; }
        set
        {
            _rotX = value;
            transform.eulerAngles = new Vector3(_rotX, m_defaultRotation.y, m_defaultRotation.z);
        }
    }

    #endregion

    #region Public Properties

    public int MaxScore => m_maxScore;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<MeshRenderer>();
        _rotX = m_defaultRotation.x;
    }

    void Update()
    {
        if ((_active || !_started) && JumpFired && !_done)
        {
            if (!_started)
                StartGame();
            DoFlap();
        }
    }

    /// <summary>
    /// Handle the bird colliding with something
    /// </summary>
    /// <param name="collision">The collision event</param>
    void OnCollisionEnter(Collision collision)
    {
        if (_done)
            return;
        switch (collision.transform.tag)
        {
            case "Coin":
                SoundManager.Instance.PlaySingle(Sound.CoinGainSound);
                Destroy(collision.gameObject);
                _score++;
                if (_score == m_maxScore)
                    EndGame(true);
                break;
            case "Ground":
                SoundManager.Instance.PlaySingle(Sound.GroundHitSound);
                Die();
                break;
            case "Pillar":
                SoundManager.Instance.PlaySingle(Sound.PipeHitSound);
                Die();
                break;
        }
    }

    #endregion

    #region Helper Methods

    void DoFlap()
    {
        _rigidbody.velocity = new Vector3(0, m_jumpForce);
        StartCoroutine(AnimFlap());
    }

    /// <summary>
    /// Sets up bird from scene components
    /// </summary>
    IEnumerator AnimFlap()
    {
        SoundManager.Instance.PlaySingle(Sound.WingFlapSound);
        _renderer.material = m_states[1];
        yield return new WaitForSeconds(0.1f);
        _renderer.material = m_states[0];
    }

    void Die()
    {
        _active = false;
        StartCoroutine(DeathTurn());
        EndGame(false);
    }

    void StartGame()
    {
        _started = true;
        _active = true;
        _rigidbody.useGravity = true;
        _rigidbody.velocity = Vector3.zero;
        MinigameManager.Instance.StartGame();
    }

    void EndGame(bool won)
    {
        if (won)
        {
            _rigidbody.useGravity = false;
            _rigidbody.velocity = Vector3.zero;
        }
        PillarController.Stop();
        _done = true;
        MinigameManager.Instance.EndGame(won, _score);
    }

    IEnumerator DeathTurn()
    {
        float startRot = XRotation;
        float endRot = startRot - 90;
        for (float turnPercent = 0; turnPercent < 1;
             turnPercent = Mathf.Clamp01(turnPercent + (Time.deltaTime / m_deathTurnTime)))
        {
            XRotation = Mathf.SmoothStep(startRot, endRot, turnPercent);
            yield return null;
        }
        XRotation = endRot;
    }

    #endregion
}
