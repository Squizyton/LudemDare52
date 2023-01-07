using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
 
    [Header("Current Mode")]
    public CurrentMode currentMode;

    [Header("Game Stats")] 
    [SerializeField] private float timeAlive;
    [SerializeField] public int currentWave;
    [SerializeField] public int currentScore;
    [SerializeField] public int currentKills;

    
    [Header("Timers")]
    [SerializeField]private float timeTillNextWave;
    
    // Start is called before the first frame update
    private void Start()
    {
        if(Instance) Destroy(this); else Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    

    public void ChangeMode(CurrentMode newMode)
    {
        currentMode = newMode;

        switch (newMode)
        {
            case CurrentMode.TopDown:
                Cursor.lockState = CursorLockMode.None;
                break;
            case CurrentMode.FPS:
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }

        
        
    }
    public enum CurrentMode
    {
        FPS,
        TopDown
    }
}
