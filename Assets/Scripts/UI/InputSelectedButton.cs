using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSelectedButton : MonoBehaviour
{
    public GameObject button, onDisabledButton;

    // Update is called once per frame
    private void Start()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(button);
    }
    private void onDisabled()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(onDisabledButton);
    }
}
