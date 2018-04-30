using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiController : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] RectTransform _canvasRectTransform;

    #endregion

    #region Private Fields

    Camera _mainCamera;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        Bounds cameraBounds = _mainCamera.OrthographicBounds();
        _canvasRectTransform.sizeDelta = new Vector2(
            cameraBounds.size.x / transform.localScale.x,
            cameraBounds.size.z / transform.localScale.y);
    }

    #endregion
}
