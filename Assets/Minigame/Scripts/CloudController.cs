using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]
public class CloudController : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] float m_maxYRange;
    [SerializeField] float m_minYRange;
    [SerializeField] float m_maxScaleRange;
    [SerializeField] float m_minScaleRange;
    [SerializeField] float m_maxSpeed;
    [SerializeField] float m_minSpeed;

    #endregion

    #region Private Fields

    Renderer _renderer;
    Rigidbody _rigidbody;
    bool _seen;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rigidbody.velocity = new Vector3(-Mathf.Lerp(m_minSpeed, m_maxSpeed, Random.value), 0);
        transform.position += new Vector3(0, Mathf.Lerp(m_minYRange, m_maxYRange, Random.value));
        transform.localScale *= Mathf.Lerp(m_minScaleRange, m_maxScaleRange, Random.value);
    }

    void Update()
    {
        if (!_seen)
            _seen = _renderer.isVisible;
        else if (!_renderer.isVisible)
            Destroy(gameObject);
    }

    #endregion
}
