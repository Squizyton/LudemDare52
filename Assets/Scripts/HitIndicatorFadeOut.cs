using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitIndicatorFadeOut : MonoBehaviour
{
    [SerializeField] CanvasGroup canvas;
    [SerializeField] Image uiElement;
    WaitForSeconds onTimer;
    WaitForSeconds offTimer;
    bool isFadePaused;
    void Update()
    {
        if (!isFadePaused)
        {
            canvas.alpha -= 2*Time.deltaTime;
        }
    }
    public void Start()
    {
        onTimer = new WaitForSeconds(0.10f);
        offTimer = new WaitForSeconds(0.05f);
    }
    public void StartFade()
    {
        canvas.alpha = 1;
    }
    public void StartFlash()
    {
        canvas.alpha = 1;
        StartCoroutine(FlashCoroutine());
    }
    private IEnumerator FlashCoroutine()
    {
        isFadePaused = true;
        int flashCount = 5;
        while (flashCount > 0)
        {
            uiElement.color = Color.yellow;
            yield return onTimer;
            uiElement.color = Color.white;
            yield return offTimer;
            flashCount--;
        }
        isFadePaused = false;
    }
}
