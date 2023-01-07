using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Weapon Related Things")] [SerializeField]
    private CanvasGroup reloadGroup;
    private Slider reloadSlider;


    #region Reload

    public void ReloadGroupStatus(bool value, float reloadTime)
    {
        reloadGroup.alpha = value ? 1 : 0;
        reloadSlider.value = 0;
        reloadSlider.maxValue = reloadTime;
    }

    public void FeedReloadTime(float time)
    {
        reloadSlider.value = time;
    }

    #endregion
}
