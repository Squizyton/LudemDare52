using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Cameras")] 
    
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;

    private bool IsRTS;
    private FMOD.Studio.EventInstance Music;

    //Change priority of the cameras based on the mode

    private void Start()
    {

        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/InGameMusic");
        Music.start();
    }
    public void ChangeMode(GameManager.CurrentMode newMode)
    {
        if (newMode == GameManager.CurrentMode.FPS)
        {
            if (!IsRTS)
            {
                //FMODUnity.RuntimeManager.PlayOneShot("event:/GameStats/GoToRTSMode");
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_GameModeTransistion");
                //FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameMode", "RTS");
                Music.setParameterByName("GameMode", 1);
                IsRTS = true;
            }
           
            topDownCamera.Priority = 0;
            fpsCamera.Priority = 10;
        }
        else
        {
            if (IsRTS)
            {
                //FMODUnity.RuntimeManager.PlayOneShot("event:/GameStats/GoToFightMode");
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_GameModeTransistion");
                //FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("GameMode", "Fight");
                Music.setParameterByName("GameMode", 0);
                IsRTS = false;
            }
         
            fpsCamera.Priority = 0;
            topDownCamera.Priority = 10;
        }
    }
}
