using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiver : MonoBehaviour
{
    [SerializeField] private bool ifShouldNotPlaySound;
    public void FmodPostFootstepsEvent()
    {
        //CheckSurfaceMaterial();
        PlaySound("event:/SFX/Enemy/Greg/Enemy_Greg_FS");
    }

    public void PlaySound(string sound)
    {
        if(!ifShouldNotPlaySound)        FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);   //FMOD
    }


    public void DevourSound()
    {
        PlaySound("event:/SFX/Enemy/Felonious/Enemy_Felonious_Devour");
    }
    
    public void BodyfallSound()
    {
        PlaySound("event:/SFX/Enemy/Enemy_BodyfallSound");
    }
    
}
