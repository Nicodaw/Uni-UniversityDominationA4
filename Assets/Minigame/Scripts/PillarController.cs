using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarController : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] float m_speed;
    [SerializeField] float m_maxYRange;
    [SerializeField] float m_minYRange;

    #endregion

    #region Private Fields

    static bool _stopped;

    Renderer _childRenderer;
    bool _seen;
    bool _hasStopped;

    #endregion

    #region MonoBehvaiour

    void Awake()
    {
        _childRenderer = transform.Find("TopPillar").GetComponent<Renderer>();
    }

    void Start()
    {
        transform.position += new Vector3(0, Mathf.Lerp(m_minYRange, m_maxYRange, Random.value));
    }

    void Update()
    {
        if (_stopped && !_hasStopped)
        {
            _hasStopped = true;
            foreach (Collider collider in GetComponentsInChildren<Collider>())
                collider.enabled = false;
        }
        if (!_hasStopped)
            transform.position -= new Vector3(m_speed * Time.deltaTime, 0, 0);

        if (!_seen)
            _seen = _childRenderer.isVisible;
        else if (!_childRenderer.isVisible)
            Destroy(gameObject);
    }

    #endregion

    #region Helper Methods

    public static void Stop()
    {
        _stopped = true;
    }
    public static void ResetPillars()
    {
        _stopped = false;
    }

    #endregion
}
