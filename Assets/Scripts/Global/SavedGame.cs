using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

[Obsolete("Will be removed/reworked after memento pattern implementation.")]
public static class SavedGame
{
    /// <summary>
    /// Creates an xml file storing all the neccessary properties to instantiate a game
    /// </summary>
    /// <param name="fileName">The filename to store the data in</param>
    /// <param name="game">The game to save</param>
    /// <returns></returns>
    public static bool Save(string fileName, Game game)
    {
        string filePath = Application.persistentDataPath + "/";
        GameData gameData = new GameData();
        gameData.SetupGameData(game); // Creates a serializable set of properties
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            using (StreamWriter w = new StreamWriter(filePath + fileName + ".xml"))
            {
                serializer.Serialize(w, gameData);
            }
            return true;
        }
        catch (SerializationException)
        {
            return false;
        }
    }

    /// <summary>
    /// Loads a given file and instantiates a game
    /// </summary>
    /// <param name="fileName">The file to be loaded, exclusing the extension</param>
    /// <returns>True if game setup correctly, false if not</returns>
    public static GameData Load(string fileName)
    {
        string filePath = Application.persistentDataPath + "/";
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            using (StreamReader r = new StreamReader(filePath + fileName + ".xml"))
            {
                GameData savedGame = (GameData)serializer.Deserialize(r);
                // Access to saved game like -> savedGame.currentPlayerID
                // Calls to instantiate methods
                return savedGame;
            }
        }
        catch (SerializationException)
        {
            return null;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// returns if a save game exists to load
    /// </summary>
    /// <param name="fileName">save file name</param>
    /// <returns>true if game exists to load else false</returns>
    public static bool SaveExists(string fileName)
    {
        string filePath = Application.persistentDataPath + "/";
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            using (StreamReader r = new StreamReader(filePath + fileName + ".xml"))
            {
                GameData savedGame = (GameData)serializer.Deserialize(r);
                // Access to saved game like -> savedGame.currentPlayerID
                // Calls to instantiate methods
                return true;
            }
        }
        catch (SerializationException)
        {
            return false;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// Generates a list of any save files in the default folder
    /// </summary>
    /// <returns>List of strings</returns>
    public static List<string> GetSaves()
    {
        string filePath = Application.persistentDataPath + "/";
        List<string> saves = new List<string>();

        DirectoryInfo d = new DirectoryInfo(filePath);
        FileInfo[] files = d.GetFiles("*.xml");

        foreach(FileInfo file in files)
        {
            saves.Add(file.Name.Replace(".xml", null));
        }
        return saves;
    }


    public static void DeleteFile(string filename)
    {
        string filePath = Application.persistentDataPath + "/";
        File.Delete(filePath + filename);
    }
}
