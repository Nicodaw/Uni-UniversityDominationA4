using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

	/*
	// Use this for initialization
	void Start () {
		
	} 
	*/

	[SerializeField] private Player player;
	[SerializeField] private UnityEngine.UI.Text header;
	[SerializeField] private UnityEngine.UI.Text headerHighlight;
	[SerializeField] private UnityEngine.UI.Text percentOwned;
	[SerializeField] private UnityEngine.UI.Text beer;
	[SerializeField] private UnityEngine.UI.Text knowledge;
	[SerializeField] private int numberOfSectors;
	private Color defaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

	public void Initialize(Player player, int player_id) {

		this.player = player;

		header = transform.Find("Header").GetComponent<UnityEngine.UI.Text>();
		headerHighlight = transform.Find("HeaderHighlight").GetComponent<UnityEngine.UI.Text>();
		percentOwned = transform.Find("PercentOwned_Value").GetComponent<UnityEngine.UI.Text>();
		beer = transform.Find("Beer_Value").GetComponent<UnityEngine.UI.Text>();
		knowledge = transform.Find("Knowledge_Value").GetComponent<UnityEngine.UI.Text>();
		numberOfSectors = player.GetGame().gameMap.GetComponent<Map>().sectors.Length;

		header.text = "Player " + player_id.ToString();
		headerHighlight.text = header.text;
		headerHighlight.color = player.GetColor();

        if (player.IsNeutral())
            header.text = "Neutral";
	
	}

	public void UpdateDisplay() {

		percentOwned.text = Mathf.Round(100 * player.ownedSectors.Count / numberOfSectors).ToString() + "%";
		beer.text = player.GetBeer().ToString();
		knowledge.text = player.GetKnowledge().ToString();

	}

	public void Activate() {

		header.color = player.GetColor();

	}

	public void Deactivate() {

		header.color = defaultHeaderColor;

	}
}
