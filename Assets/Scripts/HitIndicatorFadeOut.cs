using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicatorFadeOut : MonoBehaviour
{
    [SerializeField] CanvasGroup canvas;

    void Update()
    {
        if (canvas.alpha > 0)        
        {
            canvas.alpha -= 2*Time.deltaTime;
        }
    }
}
