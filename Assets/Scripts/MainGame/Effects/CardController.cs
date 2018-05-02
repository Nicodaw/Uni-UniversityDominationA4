using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    #region Unity Bindings

    [Header("UI")]
    [SerializeField] Text m_nameText;
    [SerializeField] Text m_descText;
    [SerializeField] Image m_cornerIcon;
    [SerializeField] MeshRenderer m_border;

    [Header("Icons")]
    [SerializeField] Sprite m_selfPlayer;
    [SerializeField] Sprite m_selfUnit;
    [SerializeField] Sprite m_enemyPlayer;
    [SerializeField] Sprite m_enemyUnit;
    [SerializeField] Sprite m_sector;
    [SerializeField] Sprite m_sacrifice;

    [Header("Materials")]
    [SerializeField] Material m_tier1;
    [SerializeField] Material m_tier2;
    [SerializeField] Material m_tier3;

    #endregion

    #region Private Fields

    // animation constants

    const float OffscreenPosition = 1.3f;
    const float PositionMoveTime = 1.5f;
    const float PositionNewMoveTime = 0.5f;
    const float TurnMoveTime = 1f;
    const float TopPosition = -10f;
    const float BottomPosition = -30f;
    const float StartSize = 0.8f;
    const float EndSize = 1.5f;
    const float MouseMinDistance = 10f;
    const float MaxMouseOffsetDistance = 10f;
    const float HeldYPosition = -10f;

    /// <summary>
    /// Whether any card is held.
    /// </summary>
    /// <remarks>
    /// Used to prevent interaction while a card is held.
    /// </remarks>
    static bool CardHeld = false;

    // state vars

    Camera _mainCamera;
    Effect _effect;
    float _aimedPosition;
    bool _readyForInteraction;
    bool _held;

    // property vars

    Vector3 _cardSizeBase;
    float _positionPercent;
    float _turnPercent;
    float _hightPercent;
    float _sizePercent;
    bool _clickable;

    #endregion

    #region Private Properties

    /// <summary>
    /// The percent across the card is along the bottom of the screen.
    /// </summary>
    float PositionPercent
    {
        get { return _positionPercent; }
        set
        {
            _positionPercent = value;
            transform.position = GetPositionByPercent(_positionPercent);
        }
    }

    /// <summary>
    /// The percentage that the card is turned.
    /// </summary>
    float TurnPercent
    {
        get { return _turnPercent; }
        set
        {
            _turnPercent = Mathf.Clamp01(value);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(180, 0, _turnPercent));
        }
    }

    /// <summary>
    /// The percentage through the hight bounds the card is.
    /// </summary>
    float HightPercent
    {
        get { return _hightPercent; }
        set
        {
            _hightPercent = Mathf.Clamp01(value);
            // basically, cards that are aimed to go further to the left
            // should experience a larger height change, so we scale the
            // height percent down, then adjust it to be centered around
            // 0.5. The actual percentage used is also raised to a power
            // in order to make it reach the bottom quicker than the top
            float scale = 1f - _aimedPosition;
            float actualPercent = Mathf.Pow(_hightPercent * scale, 1.2f);
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(BottomPosition, TopPosition, 0.5f + actualPercent - (scale * 0.5f)),
                transform.position.z);
        }
    }

    /// <summary>
    /// The actual size percent used to scale the card based on how far betwee
    /// the bounds it is.
    /// </summary>
    float SizePercentLerp => Mathf.Lerp(StartSize, EndSize, _sizePercent);

    /// <summary>
    /// The percentage that the size of the card is between the size bounds.
    /// </summary>
    float SizePercent
    {
        get { return _sizePercent; }
        set
        {
            _sizePercent = Mathf.Clamp01(value);
            transform.localScale = Vector3.one * SizePercentLerp;
        }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The effect that is driving the card.
    /// </summary>
    /// <value>The effect.</value>
    public Effect Effect => _effect;

    /// <summary>
    /// The actual size of the card.
    /// </summary>
    public Vector3 CardSize => _cardSizeBase * SizePercentLerp;

    /// <summary>
    /// The base size of the card.
    /// </summary>
    public Vector3 CardSizeBase => _cardSizeBase;

    /// <summary>
    /// Whether the card is clickable.
    /// </summary>
    public bool Clickable
    {
        get { return _clickable; }
        set { _clickable = value; }
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the card is consumed.
    /// </summary>
    public event System.EventHandler OnConsumed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the card with the given effect.
    /// </summary>
    /// <param name="effect">The effect to initialize with.</param>
    public void Init(Effect effect)
    {
        _effect = effect;
        m_nameText.text = _effect.CardName;
        m_descText.text = _effect.CardDescription;
        switch (_effect.CardCornerIcon)
        {
            case CardCornerIcon.SelfPlayer:
                m_cornerIcon.sprite = m_selfPlayer;
                break;
            case CardCornerIcon.SelfUnit:
                m_cornerIcon.sprite = m_selfUnit;
                break;
            case CardCornerIcon.EnemyPlayer:
                m_cornerIcon.sprite = m_enemyPlayer;
                break;
            case CardCornerIcon.EnemyUnit:
                m_cornerIcon.sprite = m_enemyUnit;
                break;
            case CardCornerIcon.Sector:
                m_cornerIcon.sprite = m_sector;
                break;
            case CardCornerIcon.Sacrifice:
                m_cornerIcon.sprite = m_sacrifice;
                break;
        }
        switch (_effect.CardTier)
        {
            case CardTier.Tier1:
                m_border.materials[0] = m_tier1;
                break;
            case CardTier.Tier2:
                m_border.materials[0] = m_tier2;
                break;
            case CardTier.Tier3:
                m_border.materials[0] = m_tier3;
                break;
        }
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _mainCamera = Camera.main;
        _cardSizeBase = m_border.bounds.size;
    }

    void Update()
    {
        SetZoomState();
        ProcessHold();
    }

    void OnMouseUpAsButton()
    {
        ProcessClick();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the corner vectors that are used to position the card.
    /// </summary>
    public Vector3[] GetCornerVectors()
    {
        const float xOffset = 1f;
        const float zOffset = 1f;
        Bounds cameraBounds = _mainCamera.OrthographicBounds();
        Vector3 sizeOffset = CardSize * 0.5f;
        return new[] {
            new Vector3(
                cameraBounds.max.x - sizeOffset.x - xOffset,
                transform.position.y,
                cameraBounds.max.z - sizeOffset.z - zOffset),
            new Vector3(
                cameraBounds.min.x + sizeOffset.x + xOffset,
                transform.position.y,
                cameraBounds.max.z - sizeOffset.z - zOffset)
        };
    }

    /// <summary>
    /// Gets the position that the card should actual go to.
    /// </summary>
    /// <returns>The actual position of the card.</returns>
    /// <param name="perc">
    /// The percentage along the bottom of the screen that the card is going.
    /// </param>
    Vector3 GetPositionByPercent(float perc)
    {
        Vector3[] corners = GetCornerVectors();
        return Vector3.LerpUnclamped(corners[0], corners[1], perc);
    }

    /// <summary>
    /// Processes the zoom state of the card.
    /// </summary>
    void SetZoomState()
    {
        if (_held) // do not process zoom if held
            return;
        if (_readyForInteraction && !CardHeld && Clickable)
        {
            const float zStartOffset = 1f;
            Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 baseSize = CardSizeBase * StartSize;
            Vector3 aimedPosition = GetPositionByPercent(_aimedPosition);
            // if mouse is far enough down the screen, do size adjustments
            if (mousePos.z > aimedPosition.z - (baseSize.z / 2f) - zStartOffset)
            {
                // scale size by how close the mouse is to the card
                float mouseDistance = aimedPosition.x - mousePos.x;
                float mouseDirection = mouseDistance < 0 ? -1f : 1f;
                mouseDistance = Mathf.Abs(mouseDistance);
                SizePercent = 1 - (mouseDistance / MouseMinDistance);
                // adjust position away from mouse to make it properly visible when overlapping
                PositionPercent = PositionPercent; // properly set Z axis

                // adjust X axis to move away from mouse
                //transform.position = new Vector3(
                //transform.position.x + (mouseDirection * Mathf.Clamp01(mouseDistance / MaxMouseOffsetDistance)),
                //transform.position.y,
                //transform.position.z);
                return; // we set the position so exit function
            }
        }
        // if we get here, default size and position
        SizePercent = 0;
        PositionPercent = PositionPercent;
        HightPercent = 0;
    }

    /// <summary>
    /// Processes the card position if it is held.
    /// </summary>
    void ProcessHold()
    {
        if (_held)
        {
            Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = HeldYPosition;
            transform.position = mousePos;
        }
    }

    /// <summary>
    /// Processes a mouse click on the card.
    /// </summary>
    void ProcessClick()
    {
        if (!Clickable)
            return;
        // deselect sectors when processing click
        HumanPlayer currentPlayer = Game.Instance.CurrentPlayer as HumanPlayer;
        if (currentPlayer != null)
            currentPlayer.DeselectSector();

        EffectAvailableSelection selection = _effect.AvailableSelection(Game.Instance);
        if (_held)
        {
            // prevent the raycast from hitting the card
            Collider triggerCollider = gameObject.GetComponent<Collider>();
            triggerCollider.enabled = false;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // check if a player UI was clicked
                // and if so attempt to apply the effect to the player
                PlayerUI playerUI = hit.collider.transform.parent?.gameObject.GetComponent<PlayerUI>();
                if (playerUI != null && (selection.Players?.Contains(playerUI.Player) ?? false))
                    ConsumeEffect(playerUI.Player.Stats);
                // check if a sector was clicked
                // if so, check if we can apply the effect to the unit,
                // otherwise just apply the effect to the sector
                Sector sector = hit.collider.gameObject.GetComponent<Sector>();
                if (sector != null)
                {
                    if (sector.Unit != null && (selection.Units?.Contains(sector.Unit) ?? false))
                        ConsumeEffect(sector.Unit.Stats);
                    else if (selection.Sectors?.Contains(sector) ?? false)
                        ConsumeEffect(sector.Stats);
                }
            }
            triggerCollider.enabled = true;
            ClearHighlights();
            CardHeld = _held = false;
            PositionPercent = PositionPercent;
        }
        else
        {
            // activate highlight for all relevant objects
            SetSelectedHighlight(true, selection);
            SizePercent = 0;
            CardHeld = _held = true;
        }
    }

    /// <summary>
    /// Sets the highlight value of the given selection.
    /// </summary>
    /// <param name="highlighted">The highlight value to apply.</param>
    /// <param name="selection">The selection.</param>
    void SetSelectedHighlight(bool highlighted, EffectAvailableSelection selection)
    {
        if (selection.Units != null)
            foreach (Sector sector in selection.Units.Select(u => u.Sector))
                sector.Highlighted = highlighted;
        if (selection.Sectors != null)
            foreach (Sector sector in selection.Sectors)
                sector.Highlighted = highlighted;
        if (selection.Players != null)
            foreach (PlayerUI playerUI in selection.Players.Select(p => p.Gui))
                playerUI.Highlighted = highlighted;
    }

    /// <summary>
    /// Clears all highlights.
    /// </summary>
    void ClearHighlights()
    {
        foreach (Sector sector in Game.Instance.Map.Sectors)
            sector.Highlighted = false;
        foreach (PlayerUI playerUI in Game.Instance.Players.Select(p => p.Gui))
            playerUI.Highlighted = false;
    }

    /// <summary>
    /// Consumes the effect.
    /// </summary>
    /// <param name="effectManager">Effect manager to apply the effect to.</param>
    void ConsumeEffect(EffectManager effectManager)
    {
        effectManager.ApplyEffect(_effect);
        Destroy(gameObject);
        OnConsumed?.Invoke(this, new System.EventArgs());
        Debug.Log(string.Format("effect {0} applied to {1}", _effect, effectManager));
    }

    /// <summary>
    /// Adjusts the position of the card.
    /// </summary>
    /// <param name="position">The new position to set to.</param>
    public void AdjustPosition(float position) => StartCoroutine(MoveIntoNewPosition(position));

    /// <summary>
    /// Processes the adjust position animation.
    /// </summary>>
    /// <param name="pos">The new position to move to.</param>
    IEnumerator MoveIntoNewPosition(float pos)
    {
        _readyForInteraction = false;
        _aimedPosition = pos;
        float oldPos = PositionPercent;
        HightPercent = 0;
        for (float animPercent = 0; animPercent < 1;
             animPercent = Mathf.Clamp01(animPercent + (Time.deltaTime / PositionNewMoveTime)))
        {
            PositionPercent = Mathf.Lerp(oldPos, _aimedPosition, animPercent);
            yield return null;
        }
        PositionPercent = _aimedPosition;
        _readyForInteraction = true;
    }

    /// <summary>
    /// Resets the card's positions to offscreen.
    /// </summary>
    public void ResetToOuterPosition() => PositionPercent = OffscreenPosition;

    #region Enter Screen Animation

    /// <summary>
    /// Plays the card enter animation.
    /// </summary>
    /// <param name="position">The final position to move to.</param>
    public void Enter(float position) => StartCoroutine(MoveIntoPosition(position));

    /// <summary>
    /// Processes the enter screen animation.
    /// </summary>
    /// <param name="end">The position to move to.</param>
    IEnumerator MoveIntoPosition(float end)
    {
        bool startedTurning = false;
        TurnPercent = 0;
        _aimedPosition = end;
        HightPercent = 1;
        for (float animPercent = 0; animPercent < 1;
             animPercent = Mathf.Clamp01(animPercent + (Time.deltaTime / PositionMoveTime)))
        {
            PositionPercent = Mathf.SmoothStep(OffscreenPosition, end, animPercent);
            if (!startedTurning && ((animPercent + (_aimedPosition * 0.5f)) >= 0.6f))
            {
                startedTurning = true;
                StartCoroutine(TurnIntoPosition());
            }
            yield return null;
        }
        PositionPercent = end;
        _readyForInteraction = true;
    }

    /// <summary>
    /// Processes the enter turn animation.
    /// </summary>
    IEnumerator TurnIntoPosition()
    {
        for (float animPercent = 0; animPercent < 1;
             animPercent = Mathf.Clamp01(animPercent + (Time.deltaTime / TurnMoveTime)))
        {
            TurnPercent = Mathf.SmoothStep(0, 1, animPercent);
            HightPercent = 1f - animPercent;
            yield return null;
        }
        TurnPercent = 1;
        HightPercent = 0;
    }

    #endregion

    #region Exit Screen Animation

    /// <summary>
    /// Plays the card exit animation.
    /// </summary>
    public void Exit() => StartCoroutine(MoveOutOfPosition());

    /// <summary>
    /// Processes the exit screen animation.
    /// </summary>
    IEnumerator MoveOutOfPosition()
    {
        _readyForInteraction = false;
        bool startedTurning = false;
        TurnPercent = 1;
        HightPercent = 0;
        for (float animPercent = 0; animPercent < 1;
             animPercent = Mathf.Clamp01(animPercent + (Time.deltaTime / PositionMoveTime)))
        {
            PositionPercent = Mathf.SmoothStep(_aimedPosition, OffscreenPosition, animPercent);
            if (!startedTurning && (animPercent + ((1 - _aimedPosition) * 0.3f) >= 0.3f))
            {
                startedTurning = true;
                StartCoroutine(TurnOutOfPosition());
            }
            yield return null;
        }
        PositionPercent = OffscreenPosition;
    }

    /// <summary>
    /// Processes the exit turn animation.
    /// </summary>
    IEnumerator TurnOutOfPosition()
    {
        for (float animPercent = 0; animPercent < 1;
             animPercent = Mathf.Clamp01(animPercent + (Time.deltaTime / TurnMoveTime)))
        {
            TurnPercent = Mathf.SmoothStep(1, 0, animPercent);
            HightPercent = animPercent;
            yield return null;
        }
        TurnPercent = 0;
        HightPercent = 1;
    }

    #endregion

    #endregion
}
