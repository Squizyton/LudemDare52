using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiver : MonoBehaviour
{
    public void FmodPostFootstepsEvent()
    {
        //CheckSurfaceMaterial();
        PlaySound("event:/SFX/Enemies/Enemy_Greg/Enemy_Greg_FS");
    }

    public void PlaySound(string sound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);   //FMOD
    }
}
