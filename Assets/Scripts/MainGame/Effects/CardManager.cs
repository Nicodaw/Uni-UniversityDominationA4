using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] GameObject m_cardPrefab;

    #endregion

    #region Private Fields

    readonly List<CardController> _cards = new List<CardController>();
    bool _clickable;

    #endregion

    #region Public Properties

    public int Count => _cards.Count;

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

    #endregion

    #region Handlers

    void Card_OnConsumed(object sender, EventArgs e)
    {
        CardController card = (CardController)sender;
        card.OnConsumed -= Card_OnConsumed;
        if (!_cards.Remove(card))
            throw new InvalidOperationException();
        SetCardPositions(false);
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

    public void AddEffect(Effect effect)
    {
        GameObject gobj = Instantiate(m_cardPrefab, transform);
        CardController card = gobj.GetComponent<CardController>();
        card.Init(effect);
        card.ResetToOuterPosition();
        card.Clickable = Clickable;
        card.OnConsumed += Card_OnConsumed;
        _cards.Add(card);
        SetCardPositions(false);
    }

    public void CardEnter()
    {
        SetCardPositions(true);
    }

    public void CardExit()
    {
        foreach (CardController card in _cards)
            card.Exit();
    }

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

    float[] GetPositions()
    {
        if (_cards.Count == 0)
            return new float[0];
        Vector3[] corners = _cards[0].GetCornerVectors();
        float sizePerc = _cards[0].CardSizeBase.x / Mathf.Abs(corners[0].x - corners[1].x);
        float space = Mathf.Min(sizePerc, 1f / _cards.Count);
        float offset = (1 - (space * (_cards.Count - 1f))) / 2f;
        float[] positions = new float[_cards.Count];
        positions[0] = offset;
        for (int i = 1; i < positions.Length; i++)
            positions[i] = positions[i - 1] + space;
        return positions;
    }

    #endregion
}
