using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODClearSounds : MonoBehaviour
{

    FMOD.Studio.Bus GameBus;

    private void Start()
    {
        GameBus = FMODUnity.RuntimeManager.GetBus("Bus:/");

    }


    public void ClearSounds()
    {

        GameBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}
