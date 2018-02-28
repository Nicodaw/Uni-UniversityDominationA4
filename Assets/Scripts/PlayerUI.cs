using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region Private Fields

    readonly Color defaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    Player player;
    Text header;
    Text headerHighlight;
    Text percentOwned;
    Text attack;
    Text defence;
    int numberOfSectors;

    #endregion

    #region Initialize

    /// <summary>
    /// Loads all the components for a PlayerUI.
    /// </summary>
    /// <param name="player">The player object for whom this UI is for.</param>
    /// <param name="player_id">ID of the player.</param>
    public void Initialize(Player player, int player_id)
    {
        this.player = player;

        header = transform.Find("Header").GetComponent<Text>();
        headerHighlight = transform.Find("HeaderHighlight").GetComponent<Text>();
        percentOwned = transform.Find("PercentOwned_Value").GetComponent<Text>();
        attack = transform.Find("ATK_Value").GetComponent<Text>();
        defence = transform.Find("DEF_Value").GetComponent<Text>();
        numberOfSectors = player.Game.gameMap.GetComponent<Map>().sectors.Length;

        header.text = "Player " + player_id.ToString();
        headerHighlight.text = header.text;
        headerHighlight.color = player.Color;

        if (player.Neutral)
        {
            header.text = "Neutral";
            headerHighlight.text = header.text;
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Update the text labels in the UI.
    /// </summary>
    public void UpdateDisplay()
    {
        percentOwned.text = Mathf.Round(100 * player.OwnedSectors.Count / numberOfSectors).ToString() + "%";
        attack.text = player.AttackBonus.ToString();
        defence.text = player.DefenceBonus.ToString();
    }

    /// <summary>
    /// Highlight the player's name in the UI.
    /// </summary>
	public void Activate()
    {
        header.color = player.Color;
    }

    /// <summary>
    /// Un-highlight the player's name in the UI.
    /// </summary>
	public void Deactivate()
    {
        header.color = defaultHeaderColor;
    }

    #endregion

}
