using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}
public class GameData : MonoBehaviour
{
    public static GameData gameData;
    private string path;
    FileStream fileStream;
    BinaryFormatter binaryFormatter;

   public SaveData saveData;
    private void Awake()
    {
        if(gameData==null)
        {
            gameData = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        Load();
    }

    public void Save()
    {
        binaryFormatter = new BinaryFormatter();
        path= Application.persistentDataPath + "/player.dat";
        if (!File.Exists(path))
        {
           fileStream = File.Create(path);
            saveData = new SaveData();
            saveData.isActive = new bool[100];
            saveData.stars = new int[100];
            saveData.highScores = new int[100];
            saveData.isActive[0] = true;
        }
        else
        {
            fileStream = File.Open(path,FileMode.Open);
        }

          SaveData data = new SaveData();
          data = saveData;
          binaryFormatter.Serialize(fileStream, data);

        fileStream.Close();
        
    }//Save Method


    public void Load()
    {
        path= Application.persistentDataPath + "/player.dat";
        if (File.Exists(path))
        {
            binaryFormatter = new BinaryFormatter();
            fileStream = File.Open(path, FileMode.Open);
            saveData = binaryFormatter.Deserialize(fileStream) as SaveData;
            fileStream.Close(); 
        }
        else
        {
            Save();
            Load();
        }

    }//Load Method


    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

}//class
























































