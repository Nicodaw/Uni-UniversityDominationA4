using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

public static class SaveManager
{
    const string SaveGameDirectory = "Saves/";
    const string SameGameExtension = ".bin";

    static string _saveGamePath;

    static void Init()
    {
        if (_saveGamePath == null)
        {
            _saveGamePath = Application.persistentDataPath + "/" + SaveGameDirectory;
            Directory.CreateDirectory(_saveGamePath);
        }
    }

    // save game

    // load game

    // save exists

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

        foreach (FileInfo file in files)
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
