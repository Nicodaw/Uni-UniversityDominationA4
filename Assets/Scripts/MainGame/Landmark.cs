using System;
using EffectImpl;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] ResourceType m_resourceType;
    [SerializeField] int m_amount = 2;

    #endregion

    #region Public Properties

    public ResourceType Resource => m_resourceType;
    public int Amount => m_amount;

    #endregion
}
