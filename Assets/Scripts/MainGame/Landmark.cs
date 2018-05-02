using UnityEngine;

public class Landmark : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] ResourceType m_resourceType;
    [SerializeField] int m_amount = 2;

    #endregion

    #region Public Properties

    /// <summary>
    /// The resource this landmark provides.
    /// </summary>
    public ResourceType Resource => m_resourceType;
    /// <summary>
    /// The amount of the resource the landmark provides.
    /// </summary>
    public int Amount => m_amount;

    #endregion
}
