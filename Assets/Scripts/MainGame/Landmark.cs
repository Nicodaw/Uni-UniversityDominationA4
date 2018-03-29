using System;
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

    internal void RegisterPlayerEffect(LandmarkEffect effect)
    {
        if (_effect != null)
            throw new InvalidOperationException();
        _effect = effect;
    }

    public void Sector_OnCaptured(object sender, UpdateEventArgs<Player> e)
    {
        e.OldValue?.Stats.RemoveEffect(_effect);
        _effect = null;
        if (e.NewValue != null)
        {
            switch (Resource)
            {
                case ResourceType.Attack:
                    _effect = new LandmarkEffect(((Sector)sender).Id, Amount, 0);
                    break;
                case ResourceType.Defence:
                    _effect = new LandmarkEffect(((Sector)sender).Id, 0, Amount);
                    break;
            }
            e.NewValue.Stats.ApplyEffect(_effect);
        }
    }

    #endregion
}
