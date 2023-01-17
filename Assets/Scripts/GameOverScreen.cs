using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    
    [Header("Stat Numbers")] 
    [SerializeField] private TMP_Text waveNumber;
    [SerializeField] private TMP_Text timeNumber;
    [SerializeField] private TMP_Text cropsHNumber;
    [SerializeField] private TMP_Text cropsPNumber;
    [SerializeField] private TMP_Text bulletsNumber;
    [SerializeField] private TMP_Text enemiesKilledNumber;


    private void Start()
    {
        waveNumber.text = GameManager.Instance.totalAmountOfWaves.ToString();
        timeNumber.text = GameManager.Instance.timeAlive.ToString();
        cropsHNumber.text = GameManager.Instance.cropsHarvested.ToString();
        cropsPNumber.text = GameManager.Instance.cropsPlanted.ToString();
        bulletsNumber.text = GameManager.Instance.bulletsFired.ToString();
        enemiesKilledNumber.text = GameManager.Instance.currentKills.ToString();

        UpdateSave();
    }

    void UpdateSave()
    {
        
        GameManager.Instance.loadSaveFile.SaveFile(GameManager.Instance);
        Destroy(GameManager.Instance.gameObject);
    }

    public void GoBackToMainMenu()
    {
        
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
