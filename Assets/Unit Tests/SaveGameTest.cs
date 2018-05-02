#if false
// tests not implemented due to complexity & time
using NUnit.Framework;

public class SaveGameTest : BaseGameTest
{
    /// <summary>
    /// Check when saving and loading a game, the game object is the same
    /// </summary>
    /// <returns></returns>
    [Test]
    public void SaveLoadGame()
    {
        Game game = new Game();
        game.Initialize(true);
        SavedGame.Save("saveTest", game);
        Game savedGame = new Game();
        savedGame.Initialize(SavedGame.Load("saveTest"));
        Assert.That(game, Is.EqualTo(savedGame));
    }

    /// <summary>
    /// Check if an incorrect filename is told to load, null is returned
    /// </summary>
    [Test]
    public void Load()
    {
        Assert.That(SavedGame.Load(""), Is.Null);
    }
}
#endif
