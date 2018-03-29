using EffectImpl;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    #region Unity Bindings

    [UnityEngine.Serialization.FormerlySerializedAs("resourceType")]
    [SerializeField] ResourceType m_resourceType;
    [UnityEngine.Serialization.FormerlySerializedAs("amount")]
    [SerializeField] int m_amount = 2;

    #endregion

    #region Private Fields

    LandmarkEffect _effect;

    #endregion

    #region Public Properties

    public ResourceType Resource => m_resourceType;
    public int Amount => m_amount;

    #endregion

    #region Handlers

#pragma warning disable RECS0154 // Parameter is never used
    public void Sector_OnCaptured(object sender, UpdateEventArgs<Player> e)
#pragma warning restore RECS0154 // Parameter is never used
    {
        e.OldValue?.Stats.RemoveEffect(_effect);
        _effect = null;
        if (e.NewValue != null)
        {
            switch (Resource)
            {
                case ResourceType.Attack:
                    _effect = new LandmarkEffect(Amount, 0);
                    break;
                case ResourceType.Defence:
                    _effect = new LandmarkEffect(0, Amount);
                    break;
            }
            e.NewValue.Stats.ApplyEffect(_effect);
        }
    }

    #endregion
}
