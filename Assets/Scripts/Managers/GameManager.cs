using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Managers
    [Header("Managers")] 
    [SerializeField]private UIManager uiManager;
    [SerializeField] private CameraManager camManager;
    [SerializeField]private MonsterSpawner creditManager;
    //MOde
    [Header("Current Mode")]
    public CurrentMode currentMode;

    //Game Stats
    [Header("Game Stats")] 
    [SerializeField] private float timeAlive;
    [SerializeField] public int currentWave;
    [SerializeField] public int currentScore;
    [SerializeField] public int currentKills;

    
    

    [Header("Game Settings")] public Transform currentTarget;
    public int enemiesRemaining;
    //Timer Stats
    [Header("Timers")]
    [SerializeField]private float timeTillNextWave;
    
    // Start is called before the first frame update
    private void Start()
    {
        if(Instance) Destroy(this); else Instance = this;
        
        
        ChangeMode(CurrentMode.FPS);
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
    
    public void RoundEnded()
    {
        if (enemiesRemaining > 0) return;
        //Have an action that automatically updates this
        creditManager.availableCredits = 25 * currentWave;
        StartCoroutine(StartWaveCountdown());
    }
    
    private IEnumerator StartWaveCountdown()
    {
        yield return new WaitForSeconds(5);
        currentWave++;
        
        //uiManager.UpdateWave(waveNumber);
        creditManager.StartWave();
    }
}
