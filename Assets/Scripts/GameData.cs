using UnityEngine;

/// <summary>
/// Serializable class to store all the properties of a game to initialise it
/// </summary>
[System.Serializable]
public class GameData
{
    // Define all properties that are needed to instantiate a Game
    // Must be public
    public Game.TurnState turnState;
    public bool gameFinished;
    public bool testMode;
    public int currentPlayerID; // Index from 0
    
    // Players
    // Attack
    public int player1Attack;
    public int player2Attack;
    public int player3Attack;
    public int player4Attack;
    // Defence
    public int player1Defence;
    public int player2Defence;
    public int player3Defence;
    public int player4Defence;

    // Color
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;
    public Color player4Color;

    // Controller (Human, Neutral or None)
    public string player1Controller;
    public string player2Controller;
    public string player3Controller;
    public string player4Controller;

    // Sectors
    // Owner
    // Player ID index from 0, -1 for none (player1 index = 0)
    public int sector01Owner;
    public int sector02Owner;
    public int sector03Owner;
    public int sector04Owner;
    public int sector05Owner;
    public int sector06Owner;
    public int sector07Owner;
    public int sector08Owner;
    public int sector09Owner;
    public int sector10Owner;
    public int sector11Owner;
    public int sector12Owner;
    public int sector13Owner;
    public int sector14Owner;
    public int sector15Owner;
    public int sector16Owner;
    public int sector17Owner;
    public int sector18Owner;
    public int sector19Owner;
    public int sector20Owner;
    public int sector21Owner;
    public int sector22Owner;
    public int sector23Owner;
    public int sector24Owner;
    public int sector25Owner;
    public int sector26Owner;
    public int sector27Owner;
    public int sector28Owner;
    public int sector29Owner;
    public int sector30Owner;
    public int sector31Owner;
    public int sector32Owner;

    // Level (-1 for none)
    public int sector01Level;
    public int sector02Level;
    public int sector03Level;
    public int sector04Level;
    public int sector05Level;
    public int sector06Level;
    public int sector07Level;
    public int sector08Level;
    public int sector09Level;
    public int sector10Level;
    public int sector11Level;
    public int sector12Level;
    public int sector13Level;
    public int sector14Level;
    public int sector15Level;
    public int sector16Level;
    public int sector17Level;
    public int sector18Level;
    public int sector19Level;
    public int sector20Level;
    public int sector21Level;
    public int sector22Level;
    public int sector23Level;
    public int sector24Level;
    public int sector25Level;
    public int sector26Level;
    public int sector27Level;
    public int sector28Level;
    public int sector29Level;
    public int sector30Level;
    public int sector31Level;
    public int sector32Level;

    // Vice Chancelor
    // Sector number
    public int VCSector;

    /// <summary>
    /// Fetches all data when called and assigns to properties
    /// </summary>
    /// <param name="game">The game to save</param>
    public void SetupGameData(Game game)
    {
        // Game properties
        this.turnState = game.GetTurnState();
        this.gameFinished = game.IsFinished();
        this.testMode = game.GetTestMode();
        this.currentPlayerID = game.GetPlayerID(game.GetCurrentPlayer());
        
        // Player properties
        Player[] players = game.GetPlayers();

        // Attack
        this.player1Attack = players[0].GetAttack();
        this.player2Attack = players[1].GetAttack();
        this.player3Attack = players[2].GetAttack();
        this.player4Attack = players[3].GetAttack();

        // Defence
        this.player1Defence = players[0].GetDefence();
        this.player2Defence = players[1].GetDefence();
        this.player3Defence = players[2].GetDefence();
        this.player4Defence = players[3].GetDefence();

        // Color
        this.player1Color = players[0].GetColor();
        this.player2Color = players[1].GetColor();
        this.player3Color = players[2].GetColor();
        this.player4Color = players[3].GetColor();

        // Controller (Human, Neutral or None)
        this.player1Controller = players[0].GetController();
        this.player2Controller = players[1].GetController();
        this.player3Controller = players[2].GetController();
        this.player4Controller = players[3].GetController();

        // Sectors
        Sector[] sectors = game.GetSectors();

        // Owner
        this.sector01Owner = game.GetPlayerID(sectors[0].Owner);
        this.sector02Owner = game.GetPlayerID(sectors[1].Owner);
        this.sector03Owner = game.GetPlayerID(sectors[2].Owner);
        this.sector04Owner = game.GetPlayerID(sectors[3].Owner);
        this.sector05Owner = game.GetPlayerID(sectors[4].Owner);
        this.sector06Owner = game.GetPlayerID(sectors[5].Owner);
        this.sector07Owner = game.GetPlayerID(sectors[6].Owner);
        this.sector08Owner = game.GetPlayerID(sectors[7].Owner);
        this.sector09Owner = game.GetPlayerID(sectors[8].Owner);
        this.sector10Owner = game.GetPlayerID(sectors[9].Owner);
        this.sector11Owner = game.GetPlayerID(sectors[10].Owner);
        this.sector12Owner = game.GetPlayerID(sectors[11].Owner);
        this.sector13Owner = game.GetPlayerID(sectors[12].Owner);
        this.sector14Owner = game.GetPlayerID(sectors[13].Owner);
        this.sector15Owner = game.GetPlayerID(sectors[14].Owner);
        this.sector16Owner = game.GetPlayerID(sectors[15].Owner);
        this.sector17Owner = game.GetPlayerID(sectors[16].Owner);
        this.sector18Owner = game.GetPlayerID(sectors[17].Owner);
        this.sector19Owner = game.GetPlayerID(sectors[18].Owner);
        this.sector20Owner = game.GetPlayerID(sectors[19].Owner);
        this.sector21Owner = game.GetPlayerID(sectors[20].Owner);
        this.sector22Owner = game.GetPlayerID(sectors[21].Owner);
        this.sector23Owner = game.GetPlayerID(sectors[22].Owner);
        this.sector24Owner = game.GetPlayerID(sectors[23].Owner);
        this.sector25Owner = game.GetPlayerID(sectors[24].Owner);
        this.sector26Owner = game.GetPlayerID(sectors[25].Owner);
        this.sector27Owner = game.GetPlayerID(sectors[26].Owner);
        this.sector28Owner = game.GetPlayerID(sectors[27].Owner);
        this.sector29Owner = game.GetPlayerID(sectors[28].Owner);
        this.sector30Owner = game.GetPlayerID(sectors[29].Owner);
        this.sector31Owner = game.GetPlayerID(sectors[30].Owner);
        this.sector32Owner = game.GetPlayerID(sectors[31].Owner);

        // Level
        this.sector01Level = sectors[0].GetLevel();
        this.sector02Level = sectors[1].GetLevel();
        this.sector03Level = sectors[2].GetLevel();
        this.sector04Level = sectors[3].GetLevel();
        this.sector05Level = sectors[4].GetLevel();
        this.sector06Level = sectors[5].GetLevel();
        this.sector07Level = sectors[6].GetLevel();
        this.sector08Level = sectors[7].GetLevel();
        this.sector09Level = sectors[8].GetLevel();
        this.sector10Level = sectors[9].GetLevel();
        this.sector11Level = sectors[10].GetLevel();
        this.sector12Level = sectors[11].GetLevel();
        this.sector13Level = sectors[12].GetLevel();
        this.sector14Level = sectors[13].GetLevel();
        this.sector15Level = sectors[14].GetLevel();
        this.sector16Level = sectors[15].GetLevel();
        this.sector17Level = sectors[16].GetLevel();
        this.sector18Level = sectors[17].GetLevel();
        this.sector19Level = sectors[18].GetLevel();
        this.sector20Level = sectors[19].GetLevel();
        this.sector21Level = sectors[20].GetLevel();
        this.sector22Level = sectors[21].GetLevel();
        this.sector23Level = sectors[22].GetLevel();
        this.sector24Level = sectors[23].GetLevel();
        this.sector25Level = sectors[24].GetLevel();
        this.sector26Level = sectors[25].GetLevel();
        this.sector27Level = sectors[26].GetLevel();
        this.sector28Level = sectors[27].GetLevel();
        this.sector29Level = sectors[28].GetLevel();
        this.sector30Level = sectors[29].GetLevel();
        this.sector31Level = sectors[30].GetLevel();
        this.sector32Level = sectors[31].GetLevel();

        // Vice Chancelor
        this.VCSector = game.GetVCSectorID();
    }
}