using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CurrentMode currentMode;
    
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
        
        
    }
    public enum CurrentMode
    {
        FPS,
        TopDown
    }
}
