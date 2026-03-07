using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string fullPath;
    private bool encrpytData;
    private string codeWord = "HarryGe";

    public FileDataHandler(string dataDirPath, string dataFileName,bool encrpyData)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
        this.encrpytData = encrpyData;
    }

    public void SaveData(GameData gameData)
    {
        try
        {
            // Creat a directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Convert the GameData to JSON string 
            string dataToSave = JsonUtility.ToJson(gameData, true);

            if(encrpytData)
                dataToSave = EncryptDecrypt(dataToSave);

            //Open/Creat a new file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                // Write Json text to the file
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToSave);
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" + e);
        }

    }

    public GameData LoadData()
    {
        GameData loadData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (encrpytData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }

            catch(Exception e)
            {
                Debug.LogError("Error on trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        return loadData;

    }

    public void Delete()
    {
        if(File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }

}
