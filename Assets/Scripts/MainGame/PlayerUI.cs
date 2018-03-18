using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] Text m_header;
    [SerializeField] Text m_headerHighlight;
    [SerializeField] Text m_percentOwned;
    [SerializeField] Text m_attack;
    [SerializeField] Text m_defence;
    [SerializeField] Vector2 m_edgeOffset = new Vector2(10f, 10f);
    [SerializeField] float m_betweenGap = 10f;

    #endregion

    #region Private Fields

    const string PlayerNameFormat = "Player {0}";
    const string PercentOwnedFormat = "{0:P0}";
    readonly Color DefaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    int _playerId;
    bool _isActive; // default: false

    #endregion

    #region Public Properties

    /// <summary>
    /// The player attatched to this UI.
    /// </summary>
    public Player Player => Game.Instance.Players[_playerId];

    /// <summary>
    /// Whether the UI is active or not.
    /// </summary>
    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            if (_isActive)
                m_header.color = Player.Color;
            else
                m_header.color = DefaultHeaderColor;
        }
    }

    #endregion

    #region Initialize

    /// <summary>
    /// Loads all the components for a PlayerUI.
    /// </summary>
    /// <param name="playerId">ID of the player.</param>
    public void Init(int playerId, Player player)
    {
        _playerId = playerId;
        m_header.text = string.Format(PlayerNameFormat, playerId + 1);
        m_headerHighlight.text = m_header.text;
        m_headerHighlight.color = player.Color;

        // player id specified position of UI
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition =
            new Vector2(0, ((rectTransform.rect.height + m_betweenGap) * -playerId)) +
            m_edgeOffset;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Update the text labels in the UI.
    /// </summary>
    public void UpdateDisplay()
    {
        m_percentOwned.text = string.Format(PercentOwnedFormat, Player.OwnedSectors.Count() / (float)Game.Instance.Map.Sectors.Length);
        m_attack.text = Player.Stats.Attack.ToString();
        m_defence.text = Player.Stats.Defence.ToString();
    }

    #endregion
}
