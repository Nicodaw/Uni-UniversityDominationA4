using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] GameObject m_cardPrefab;

    #endregion

    #region Private Fields

    readonly List<CardController> _cards = new List<CardController>();
    bool _clickable = true;
    bool _shown;

    #endregion

    #region Public Properties

    /// <summary>
    /// The number of cards in the manager.
    /// </summary>
    public int Count => _cards.Count;

    /// <summary>
    /// Whether the cards are clickable or not.
    /// </summary>
    public bool Clickable
    {
        get { return _clickable; }
        set
        {
            _clickable = value;
            foreach (CardController card in _cards)
                card.Clickable = _clickable;
        }
    }

    #endregion

    #region Serialization

    public SerializableCardManager CreateMemento()
    {
        return new SerializableCardManager
        {
            cards = _cards.Select(c => c.Effect).ToArray(),
            shown = _shown
        };
    }

    public void RestoreMemento(SerializableCardManager memento)
    {
        AddCards(memento.cards);
        if (memento.shown)
            CardsEnter();
    }

    #endregion

    #region Handlers

    void Card_OnConsumed(object sender, EventArgs e)
    {
        CardController card = (CardController)sender;
        RemoveCard(card);
        Sound toPlay;
        switch (card.Effect.CardCornerIcon)
        {
            case CardCornerIcon.SelfPlayer:
            case CardCornerIcon.SelfUnit:
                toPlay = Sound.FriendlyEffectSound;
                break;
            case CardCornerIcon.EnemyPlayer:
            case CardCornerIcon.EnemyUnit:
                toPlay = Sound.EnemyEffectSound;
                break;
            case CardCornerIcon.Sector:
                toPlay = Sound.SectorEffect;
                break;
            case CardCornerIcon.Sacrifice:
                toPlay = Sound.SacrificeSound;
                break;
            default:
                throw new InvalidOperationException();
        }
        SoundManager.Instance.PlaySingle(toPlay);
    }

    #endregion

    #region MonoBehaviour

    void OnEnable()
    {
        foreach (CardController card in _cards)
            card.OnConsumed += Card_OnConsumed;
    }

    void OnDisable()
    {
        foreach (CardController card in _cards)
            card.OnConsumed -= Card_OnConsumed;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Adds the given cards to the manager.
    /// </summary>
    /// <param name="effects">The effects to add.</param>
    public void AddCards(params Effect[] effects)
    {
        foreach (Effect effect in effects)
        {
            GameObject gobj = Instantiate(m_cardPrefab, transform);
            CardController card = gobj.GetComponent<CardController>();
            card.Init(effect);
            card.ResetToOuterPosition();
            card.Clickable = Clickable;
            card.OnConsumed += Card_OnConsumed;
            _cards.Add(card);
        }
        if (_shown)
            SetCardPositions(false);
        if (Count > 0)
            SoundManager.Instance.PlaySingle(Sound.CardInSound);
    }

    /// <summary>
    /// Removes a random card from the manager and returns the effect it had.
    /// </summary>
    /// <returns>The random card's effect.</returns>
    public Effect RemoveRandomCard()
    {
        CardController card = _cards.RandomOrDefault();
        if (card != null)
        {
            RemoveCard(card);
            return card.Effect;
        }
        return null;
    }

    /// <summary>
    /// Removes the card from the manager.
    /// </summary>
    /// <param name="card">The card to remove.</param>
    void RemoveCard(CardController card)
    {
        card.OnConsumed -= Card_OnConsumed;
        if (!_cards.Remove(card))
            throw new InvalidOperationException();
        if (_shown)
            SetCardPositions(false);
    }

    /// <summary>
    /// Plays the card enter animations.
    /// </summary>
    public void CardsEnter()
    {
        SetCardPositions(true);
        _shown = true;
        if (Count > 0)
            SoundManager.Instance.PlaySingle(Sound.CardInSound);
    }

    /// <summary>
    /// Plays the card exit animations.
    /// </summary>
    public void CardsExit()
    {
        if (Count > 0)
            SoundManager.Instance.PlaySingle(Sound.CardOutSound);
        _shown = false;
        foreach (CardController card in _cards)
            card.Exit();
    }

    /// <summary>
    /// Sets the card positions.
    /// </summary>
    /// <param name="enter">
    /// Whether to use the enter animation (<c>true</c>), or the adjust
    /// animation (<c>false</c>).
    /// </param>
    void SetCardPositions(bool enter)
    {
        float[] positions = GetPositions();
        for (int i = 0; i < _cards.Count; i++)
        {
            if (enter)
                _cards[i].Enter(positions[i]);
            else
                _cards[i].AdjustPosition(positions[i]);
        }
    }

    /// <summary>
    /// Gets the positions that the cards should go to.
    /// </summary>
    /// <returns>The positions of the cards.</returns>
    float[] GetPositions()
    {
        // return empty array for no cards
        if (_cards.Count == 0)
            return new float[0];
        // we get the percentage of the percentage that the width of each card
        // will take up
        Vector3[] corners = _cards[0].GetCornerVectors();
        float sizePerc = _cards[0].CardSizeBase.x / Mathf.Abs(corners[0].x - corners[1].x);
        // we then get the space that we will use between each card
        // (we account for if don't have enough space, and require cards overlapping)
        float space = Mathf.Min(sizePerc, 1f / _cards.Count);
        // since we want to centre the cards on the screen, we get the offset
        // on the left that would push all the cards to be on each side of the
        // screen evenly
        float offset = (1 - (space * (_cards.Count - 1f))) / 2f;
        // we then just calculate each position in turn using the offset to
        // start, and then the space between for each next position
        float[] positions = new float[_cards.Count];
        positions[0] = offset;
        for (int i = 1; i < positions.Length; i++)
            positions[i] = positions[i - 1] + space;
        return positions;
    }

    #endregion
}
