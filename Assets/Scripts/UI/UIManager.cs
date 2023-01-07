using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
    
        [Header("Weapon Related Things")] [SerializeField]
        private CanvasGroup reloadGroup;
        [SerializeField]private Slider reloadSlider;

        private void Awake()
        {
            Instance = this;
      
        }


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
