#if UNITY_EDITOR
using NUnit.Framework;

public class PlayerTest : BaseGameTest
{
    [Test]
    public void CaptureSector_ChangesOwner()
    {
        Player previousOwner = map.Sectors[0].Owner;

        game.players[0].Capture(map.Sectors[0]);
        Assert.That(map.Sectors[0].Owner, Is.EqualTo(game.players[0])); // owner stored in sector
        Assert.That(game.players[0].OwnedSectors, Contains.Item(map.Sectors[0])); // sector is stored as owned by the player

        if (previousOwner != null) // if sector had previous owner
            Assert.That(previousOwner.OwnedSectors, Does.Not.Contains(map.Sectors[0])); // sector has been removed from previous owner list
    }

    [Test]
    public void CaptureLandmark_BothPlayersBeerAmountCorrect()
    {
        // capturing landmark
        Sector landmarkedSector = map.Sectors[1];
        landmarkedSector.Initialize();

        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.That(landmarkedSector.Landmark, Is.Not.Null);
        landmark.Resource = ResourceType.Attack;

        // get beer amounts for each player before capture
        int attackerBeerBeforeCapture = playerA.AttackBonus;
        int defenderBeerBeforeCapture = playerB.AttackBonus;
        Player previousOwner = landmarkedSector.Owner;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.That(landmarkedSector.Owner, Is.EqualTo(playerA));
        Assert.That(playerA.OwnedSectors, Contains.Item(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.That(attackerBeerBeforeCapture + landmark.Amount, Is.EqualTo(playerA.AttackBonus));
        Assert.That(defenderBeerBeforeCapture - landmark.Amount, Is.EqualTo(previousOwner.AttackBonus));
    }

    [Test]
    public void CaptureLandmark_BothPlayersKnowledgeAmountCorrect()
    {
        // capturing landmark
        Sector landmarkedSector = map.Sectors[1];
        landmarkedSector.Initialize();

        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.That(landmarkedSector.Landmark, Is.Not.Null);
        landmark.Resource = ResourceType.Defence;

        // get knowledge amounts for each player before capture
        int attackerKnowledgeBeforeCapture = playerA.DefenceBonus;
        int defenderKnowledgeBeforeCapture = playerB.DefenceBonus;
        Player previousOwner = landmarkedSector.Owner;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.That(landmarkedSector.Owner, Is.EqualTo(playerA));
        Assert.That(playerA.OwnedSectors, Contains.Item(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.That(attackerKnowledgeBeforeCapture + landmark.Amount, Is.EqualTo(playerA.DefenceBonus));
        Assert.That(defenderKnowledgeBeforeCapture - landmark.Amount, Is.EqualTo(previousOwner.DefenceBonus));
    }

    [Test]
    public void CaptureLandmark_NeutralLandmarkPlayerBeerAmountCorrect()
    {
        // capturing landmark
        Sector landmarkedSector = map.Sectors[1];
        landmarkedSector.Initialize();

        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.That(landmarkedSector.Landmark, Is.Not.Null);
        landmark.Resource = ResourceType.Attack;

        // get player beer amount before capture
        int oldBeer = playerA.AttackBonus;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.That(landmarkedSector.Owner, Is.EqualTo(playerA));
        Assert.That(playerA.OwnedSectors, Contains.Item(landmarkedSector));

        // ensure resources are gained correctly
        Assert.That(playerA.AttackBonus - oldBeer, Is.EqualTo(landmark.Amount));
    }

    [Test]
    public void CaptureLandmark_NeutralLandmarkPlayerKnowledgeAmountCorrect()
    {
        // capturing landmark
        Sector landmarkedSector = map.Sectors[1];
        landmarkedSector.Initialize();

        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.That(landmarkedSector.Landmark, Is.Not.Null);
        landmark.Resource = ResourceType.Defence;

        // get player knowledge amount before capture
        int oldKnowledge = playerA.DefenceBonus;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.That(landmarkedSector.Owner, Is.EqualTo(playerA));
        Assert.That(playerA.OwnedSectors, Contains.Item(landmarkedSector));

        // ensure resources are gained correctly
        Assert.That(playerA.DefenceBonus - oldKnowledge, Is.EqualTo(landmark.Amount));
    }

    [Test]
    public void SpawnUnits_SpawnedWhenLandmarkOwnedAndUnoccupied()
    {
        Sector landmarkedSector = map.Sectors[1];
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = null;
        Assert.That(landmarkedSector.Landmark, Is.Not.Null);

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a unit has been spawned for playerA in landmarkedSector
        Assert.That(playerA.Units, Contains.Item(landmarkedSector.Unit));
    }

    [Test]
    public void SpawnUnits_NotSpawnedWhenLandmarkOwnedAndOccupied()
    {
        Sector landmarkedSector = map.Sectors[1];
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and contains a Level 5 unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = InitUnit(0);
        landmarkedSector.Unit.Level = 5;
        landmarkedSector.Unit.Owner = playerA;
        Assert.That(landmarkedSector.Landmark, Is.Not.Null);

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a Level 1 unit has not spawned over the Level 5 unit already in landmarkedSector
        Assert.That(landmarkedSector.Unit.Level, Is.EqualTo(5));
    }

    [Test]
    public void SpawnUnits_NotSpawnedWhenLandmarkNotOwned()
    {
        Sector landmarkedSector = map.Sectors[1];
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        landmarkedSector.Unit = null;

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = null;
        Assert.That(landmarkedSector.Landmark, Is.Not.Null);

        playerB.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure no unit is spawned at landmarkedSector
        Assert.That(landmarkedSector.Unit, Is.Null);
    }

    [Test]
    public void IsEliminated_PlayerWithNoUnitsAndNoLandmarksEliminated()
    {
        Player playerA = game.players[0];

        Assert.That(playerA.IsEliminated(), Is.False); // not eliminated because they have units

        for (int i = 0; i < playerA.Units.Count; i++)
            playerA.Units[i].DestroySelf(); // removes units
        Assert.That(playerA.IsEliminated(), Is.False); // not eliminated because they still have a landmark

        // player[0] needs to lose their landmark
        for (int i = 0; i < playerA.OwnedSectors.Count; i++)
            playerA.OwnedSectors[i].Landmark = null; // player[0] no longer has landmarks
        Assert.That(playerA.IsEliminated());
    }
}
#endif
