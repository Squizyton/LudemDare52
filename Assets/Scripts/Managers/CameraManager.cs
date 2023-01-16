using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Cameras")] 
    
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;

    private bool IsRTS;
    private FMOD.Studio.EventInstance Music;
    FMOD.Studio.EventInstance muteSFXsnapshot;

    //Change priority of the cameras based on the mode

    private void Start()
    {

        muteSFXsnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/MuteFightSFX");
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Music_InGame");
        Music.start();
    }
    public void ChangeMode(GameManager.CurrentMode newMode)
    {
        if (newMode == GameManager.CurrentMode.FPS)
        {
            
            if (!IsRTS)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                Music.setParameterByName("GameMode", 1);
                muteSFXsnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                IsRTS = true;
            }
           
            topDownCamera.Priority = 0;
            fpsCamera.Priority = 10;
            
        }
        else
        {
            if (IsRTS)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                Music.setParameterByName("GameMode", 0);
                muteSFXsnapshot.start();
                IsRTS = false;
            }
         
            fpsCamera.Priority = 0;
            topDownCamera.Priority = 10;
        }
    }
}
