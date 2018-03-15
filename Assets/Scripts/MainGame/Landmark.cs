using UnityEngine;

public class Landmark : MonoBehaviour
{
    #region Unity Bindings

    [UnityEngine.Serialization.FormerlySerializedAs("resourceType")]
    [SerializeField] ResourceType m_resourceType;
    [UnityEngine.Serialization.FormerlySerializedAs("amount")]
    [SerializeField] int m_amount = 2;

    #endregion

    #region Public Properties

    public ResourceType Resource => m_resourceType;
    public int Amount => m_amount;

    #endregion
}
