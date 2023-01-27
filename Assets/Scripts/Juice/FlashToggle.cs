using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FlashToggle : MonoBehaviour
{
    [SerializeField] Image uiElement;
    WaitForSeconds onTimer;
    WaitForSeconds offTimer;
    public void Start()
    {
        onTimer = new WaitForSeconds(0.10f);
        offTimer = new WaitForSeconds(0.05f);
    }
    public void StartFlash()
    {
        StartCoroutine(FlashCoroutine());
    }
    private IEnumerator FlashCoroutine()
    {
        int flashCount = 5;
        while(flashCount > 0)
        {
            uiElement.color = Color.yellow;
            yield return onTimer;
            uiElement.color = Color.white;
            yield return offTimer;
            flashCount--;
        }

    }
}
