using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform player;
    
    //Managers
    [Header("Managers")] 
    [SerializeField]private UIManager uiManager;
    [SerializeField] private CameraManager camManager;
    [SerializeField]private MonsterSpawner creditManager;
    [SerializeField]private LoadSaveFile loadSaveFile;
    [SerializeField] public PlotGrid grid;
    //MOde
    [Header("Current Mode")]
    public CurrentMode currentMode;

    //Game Stats
    [Header("Game Stats")] 
    [SerializeField] private float timeAlive;
    [SerializeField] public int currentWave;
    [SerializeField] public int currentScore;
    [SerializeField] public int currentKills;
    [SerializeField] public int totalAmountOfWaves;
    
    

    [Header("Game Settings")] public Transform currentTarget;
    public int enemiesRemaining;

    [SerializeField] private Transform cow;
    //Timer Stats
    [Header("Timers")]
    [SerializeField]private float timeTillNextWave;
    
    [Header("Cow SpawnPoints")]
    public Transform[] spawnPoints;

    public Vector3 normalCowPosition;

    [Header("Other")] public GameObject sproutModel;

    public Collider monsterBounds;
    // Start is called before the first frame update
    private void Awake()
    {
        if(Instance) Destroy(this); else Instance = this;
        
        
        DontDestroyOnLoad(this);
        loadSaveFile = FindObjectOfType<LoadSaveFile>();
        
        normalCowPosition = cow.position;
    }

    private void Start()
    {
        ChangeMode(CurrentMode.TopDown);

        if (loadSaveFile != null && !loadSaveFile.saveFile.didTutorial)
        {
            StartTutorial();
        }
    }

    // Update is called once per framez
    void Update()
    {
        switch (currentMode)
        {
            case CurrentMode.FPS:
                ChangeMode(CurrentMode.FPS);
                //Increment total time alive
                timeAlive += Time.deltaTime;
                UIManager.Instance.UpdateTimeAlive(timeAlive);
                break;
            case CurrentMode.TopDown:
                ChangeMode(CurrentMode.TopDown);
                break;
        }
    }
    
    public void ChangeMode(CurrentMode newMode)
    {
        currentMode = newMode;

        //Change the mode of the current mode based on the new mode
        Cursor.lockState = newMode switch
        {
            CurrentMode.TopDown => CursorLockMode.None,
            CurrentMode.FPS => CursorLockMode.Locked,
            _ => Cursor.lockState
        };
        
        //Tell Camera Manager to change the camera mode
        camManager.ChangeMode(newMode);
        uiManager.ChangeModeUI(newMode);
    }
    public enum CurrentMode
    {
        FPS,
        TopDown
    }


    public void StartRoundOfWaves()
    {
        totalAmountOfWaves++;
        
        currentWave = 1;
        UIManager.Instance.UpdateWaveCount(currentWave);
        creditManager.StartWave();
        creditManager.availableCredits = 25 * totalAmountOfWaves;

        cow.position = normalCowPosition;
        
        if (totalAmountOfWaves % 10 == 0)
        {
            currentTarget = cow;

            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var collider = spawnPoint.GetComponent<Collider>();


            //get a random position in collider bounds
            var randomPos = new Vector3(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                0,
                Random.Range(collider.bounds.min.z, collider.bounds.max.z)
            );

            cow.position = randomPos;

            UIManager.Instance.SetCowText(true);
        }
        else currentTarget = player;

        ChangeMode(CurrentMode.FPS);
    }


    [ContextMenu("TPS Test")]
    public void TpsTest()
    {
        ChangeMode(CurrentMode.TopDown);
    }
    
    [ContextMenu("FPS Test")]
    public void FpsTest()
    {
        ChangeMode(CurrentMode.FPS);
    }

    public void RemoveEnemy()
    {
        enemiesRemaining--;
        UIManager.Instance.UpdateEnemiesRemaining(enemiesRemaining);
        RoundEnded();
    }

    public void RoundEnded()
    {
        if (creditManager.IsSpawning()|| enemiesRemaining > 0) return;
        //Have an action that automatically updates this
        currentWave++;
        UIManager.Instance.UpdateWaveCount(currentWave);
        
        if (currentWave < 6)
        {
            totalAmountOfWaves++;
            creditManager.availableCredits = 25 * totalAmountOfWaves;
            StartCoroutine(StartWaveCountdown());
        }else ChangeMode(CurrentMode.TopDown);
    }
    
    private IEnumerator StartWaveCountdown()
    {
        yield return new WaitForSeconds(5);
        //uiManager.UpdateWave(waveNumber);
        creditManager.StartWave();
    }
    
    
    
    
    #region Tutorial

    private void StartTutorial()
    {
        PlayerInputController.Instance.playerControls.Player.Confirm.performed +=  ProgressTutorial;
        uiManager.StartTutorial();
    }

    private void ProgressTutorial(InputAction.CallbackContext ctx)
    {
        uiManager.NextTutorial();
    }
    
    public void EndTutorial()
    {
        loadSaveFile.saveFile.didTutorial = true;
        loadSaveFile.SaveFile();

        PlayerInputController.Instance.playerControls.Player.Confirm.performed -= ProgressTutorial;
    }
    #endregion
}
