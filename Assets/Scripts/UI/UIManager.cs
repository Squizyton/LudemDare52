using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
    
        [Header("Mode UI")]
        [SerializeField]private CanvasGroup fpsCanvasGroup;
        [SerializeField]private CanvasGroup topDownCanvasGroup;
        
        
        [Header("Weapon Related Things")] [SerializeField]
        private CanvasGroup reloadGroup;
        [SerializeField]private Slider reloadSlider;

        private void Awake()
        {
            Instance = this;
      
        }
        
        #region Mode Changing

        public void ChangeModeUI(GameManager.CurrentMode newMode)
        {
            switch (newMode)
            {
                case GameManager.CurrentMode.FPS:
                    fpsCanvasGroup.alpha = 1;
                    topDownCanvasGroup.alpha = 0;
                    break;
                case GameManager.CurrentMode.TopDown:
                    topDownCanvasGroup.alpha = 1;
                    fpsCanvasGroup.alpha = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
            }
        }

        #endregion
        

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
}
