using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Audio : MonoBehaviour
{
    public void FmodClick()
    {
        PlaySound("event:/SFX/UI/UI_Menu_Click");
    }
    public void FmodOnSelect()
    {
        PlaySound("event:/SFX/UI/UI_Menu_Select");
    }
    public void FmodBack()
    {
        PlaySound("event:/SFX/UI/UI_Menu_Back");
    }

    private void PlaySound(string EventToPlay)
    {
        FMODUnity.RuntimeManager.PlayOneShot(EventToPlay);
    }
}
