using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;
    
    [SerializeField]private LoadSaveFile loadSaveFile;
    
    
    [Header("Name Stuff")]
    [SerializeField] private GameObject EnterNamePanel;
    [SerializeField]private TMP_InputField nameInputField;
    [SerializeField]private TMP_Text nameText;

    void Awake()
    {
        Instance = this;
    }

    #region Enter Name
    public void EnterNameUI()
    {
        EnterNamePanel.SetActive(true);
    }

    public void EnteredName()
    {
        if (nameInputField.text.Length <= 0) return;
        
        
        loadSaveFile.saveFile.farmerName = nameInputField.text;
        
        loadSaveFile.SaveFile();
        
        nameText.text = loadSaveFile.saveFile.farmerName;
    }
    
    
    
    
    #endregion
}
