using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSelectedButton : MonoBehaviour
{
    public GameObject button;

    // Update is called once per frame
    private void onEnabled()
    {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(button);
    }
}
