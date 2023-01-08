using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Cameras")] 
    
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;

    private bool IsRTS;
    
    //Change priority of the cameras based on the mode
    public void ChangeMode(GameManager.CurrentMode newMode)
    {
        if (newMode == GameManager.CurrentMode.FPS)
        {
            if (!IsRTS)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/GameStats/GoToRTSMode");
                IsRTS = true;
            }
           
            topDownCamera.Priority = 0;
            fpsCamera.Priority = 10;
        }
        else
        {
            if (IsRTS)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/GameStats/GoToFightMode");
                IsRTS = false;
            }
         
            fpsCamera.Priority = 0;
            topDownCamera.Priority = 10;
        }
    }
}
