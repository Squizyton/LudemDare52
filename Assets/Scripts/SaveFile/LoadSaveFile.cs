using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SaveFile;
using UnityEngine;
using UnityEngine.Windows;
using Directory = System.IO.Directory;

public class LoadSaveFile : MonoBehaviour
{
    public FarmerInfo saveFile;
    
    
    

    private void Start()
    {
        DontDestroyOnLoad(this);


        LoadTheSaveFile();
    }

    public void LoadTheSaveFile()
    {
        var path = Application.persistentDataPath+"/FarmerInfo";
        Debug.Log("Looking At:" + path);


        if (!Directory.Exists(path))
        {
            Debug.Log("No save file found");
            //Assume there is no save file
            CreateNewSaveFile();
        }
        else
        {

            var formatter = new BinaryFormatter();
            var fileStream = new FileStream(path + "/saveFile.greg", FileMode.Open);

            saveFile = (FarmerInfo) formatter.Deserialize(fileStream);
            MainMenuUI.Instance.UpdateName(saveFile.farmerName);
            fileStream.Close();
        }
    }


    private void CreateNewSaveFile()
    {
        saveFile = new FarmerInfo();

        var path = Application.persistentDataPath + "/FarmerInfo";
        Directory.CreateDirectory(path);
        
        var formatter = new BinaryFormatter();
        Debug.Log(path);
        var fileStream = new FileStream(path+"/saveFile.greg", FileMode.Create);
        
        formatter.Serialize(fileStream, saveFile);
        fileStream.Close();

        MainMenuUI.Instance.EnterNameUI();


    }
    /// <summary>
    /// Called without Game Manager, save basic things (think auto save)
    /// </summary>
    public void SaveFile()
    {
        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + "/farmerInfo/saveFile.greg";
        var fileStream = new FileStream(path, FileMode.Create);
        formatter.Serialize(fileStream, saveFile);
        
        fileStream.Close();
    }
    
    /// <summary>
    /// Called With Game Manager, used to save stats and everything
    /// </summary>
    /// <param name="gameManager"></param>
    public void SaveFile(GameManager gameManager)
    {
        
    }
    

}
