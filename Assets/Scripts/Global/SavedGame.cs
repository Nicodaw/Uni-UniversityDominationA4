using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;

public static class SaveManager
{
    const string SaveDirectory = "/Saves/";
    const string SavePrefix = "Save-";
    const string SaveExtension = ".bin";
    const string SavePathFormat = "{0}-{{0:D3}}{1}";

    static string _savePath;
    static string _saveNameFormat;

    static Regex _saveSlotRegex = new Regex(string.Format(@"^{0}(...){1}$", SavePrefix, SaveExtension));

    static string GetSavePath(int slot)
    {
        if (_saveNameFormat == null)
        {
            _savePath = Application.persistentDataPath + SaveDirectory;
            _saveNameFormat = string.Format(SavePathFormat,
                                          SavePrefix,
                                          SaveExtension);
            Directory.CreateDirectory(_savePath);
        }
        return _savePath + string.Format(_saveNameFormat, slot);
    }

    public static void SaveGame(SerializableGame game, int slot)
    {
        using (FileStream fs = new FileStream(GetSavePath(slot), FileMode.OpenOrCreate))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, game);
            }
            catch (SerializationException ex)
            {
                Debug.Log("Failed to serialize. Reason: " + ex.Message);
                throw ex;
            }
        }
    }

    public static SerializableGame LoadGame(int slot)
    {
        if (!SaveExists(slot))
            throw new FileNotFoundException();

        using (FileStream fs = new FileStream(GetSavePath(slot), FileMode.Open))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (SerializableGame)formatter.Deserialize(fs);
            }
            catch (SerializationException ex)
            {
                Debug.Log("Failed to deserialize. Reason: " + ex.Message);
                throw ex;
            }
        }
    }

    public static bool SaveExists(int slot) => File.Exists(GetSavePath(slot));

    /// <summary>
    /// Gets an array of all the slots that have saves in.
    /// </summary>
    /// <returns>The array of filled save slots.</returns>
    public static int[] GetFilledSlots()
    {
        GetSavePath(0); // make sure variables have been initialized
        DirectoryInfo d = new DirectoryInfo(_savePath);
        return d.GetFiles(string.Format(_saveNameFormat, "???"))
                .Select(fi => int.Parse(_saveSlotRegex.Match(fi.Name).Groups[1].Value))
                .ToArray();
    }

    public static void DeleteSave(int slot)
    {
        if (!SaveExists(slot))
            throw new FileNotFoundException();

        File.Delete(GetSavePath(slot));
    }
}
