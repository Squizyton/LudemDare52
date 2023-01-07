using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Cameras")] 
    
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;
    
    //Change priority of the cameras based on the mode
    public void ChangeMode(GameManager.CurrentMode newMode)
    {
        if (newMode == GameManager.CurrentMode.FPS)
        {
            topDownCamera.Priority = 0;
            fpsCamera.Priority = 10;
        }
        else
        {
            fpsCamera.Priority = 0;
            topDownCamera.Priority = 10;
        }
    }
}
