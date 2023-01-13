using System.Collections;
using System.Collections.Generic;
using SaveFile;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance;
    
    [SerializeField]private LoadSaveFile loadSaveFile;
    
    
    [Header("Name Stuff")]
    [SerializeField] private GameObject EnterNamePanel;
    [SerializeField]private TMP_InputField nameInputField;
    [SerializeField]private TMP_Text nameText;


    [Header("Stat Numbers")] 
    [SerializeField] private TMP_Text waveNumber;
    [SerializeField] private TMP_Text timeNumber;
    [SerializeField] private TMP_Text cropsHNumber;
    [SerializeField] private TMP_Text cropsPNumber;
    [SerializeField] private TMP_Text bulletsNumber;
    [SerializeField] private TMP_Text enemiesKilledNumber;

    public GameObject nameFirstSelectedButton, menuFirstSelectedButton, statsFirstSelectedButton;

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
        EnterNamePanel.SetActive(false);
    }

    public void CanceledName()
    {
        nameInputField.text = "Unnamed Farmer";


        loadSaveFile.saveFile.farmerName = nameInputField.text;

        loadSaveFile.SaveFile();

        nameText.text = loadSaveFile.saveFile.farmerName;
        EnterNamePanel.SetActive(false);
    }


    public void UpdateEverything(FarmerInfo info)
    {
        nameText.text = info.farmerName;
        waveNumber.text = info.totalWavesSurvived.ToString();
        var minutes = Mathf.FloorToInt(info.totalTimeAlive / 60F);
        var seconds = Mathf.FloorToInt(info.totalTimeAlive - minutes * 60);
        timeNumber.text = $"{minutes:0}:{seconds:00}";
        cropsHNumber.text = info.totalCropsHarvested.ToString();
        cropsPNumber.text = info.totalCropsPlanted.ToString();
        bulletsNumber.text = info.bulletsShot.ToString();
        enemiesKilledNumber.text = info.totalEnemiesKilled.ToString();

    }


    #endregion

    #region LoadGameScene
    
    public void LoadGameScene()
    {
        SceneManager.LoadSceneAsync("GameScene");
    }
    

    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetSelectedButton(GameObject button)
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void SetStatsSelectedButton()
    {
        SetSelectedButton(statsFirstSelectedButton);
    }
    public void SetMenuSelectedButton()
    {
        SetSelectedButton(menuFirstSelectedButton);
    }
    public void SetNameSelectedButton()
    {
        SetSelectedButton(nameFirstSelectedButton);
    }

}
