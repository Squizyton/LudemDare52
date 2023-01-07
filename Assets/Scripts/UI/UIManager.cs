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
        private PlayerInventory playerInventory;

        private void Awake()
        {
            Instance = this;

            //TODO: Change this to something more performant
            GameObject playerObj = FindObjectOfType<PlayerInventory>().gameObject;
            playerObj.TryGetComponent(typeof(PlayerInventory), out Component inventory);
            if (inventory)
            {
                playerInventory = (PlayerInventory)inventory;
            }
        }
        
        #region Mode Changing

        public void ChangeModeUI(GameManager.CurrentMode newMode)
        {
            switch (newMode)
            {
                case GameManager.CurrentMode.FPS:
                    fpsCanvasGroup.alpha = 1;
                    topDownCanvasGroup.alpha = 0;
                    topDownCanvasGroup.interactable = false;
                    break;
                case GameManager.CurrentMode.TopDown:
                    topDownCanvasGroup.alpha = 1;
                    topDownCanvasGroup.interactable = true;
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

        #region SeedMenu
        public void SelectSeeds(string seedName)
        {
            playerInventory.SetSeed(seedName);
            Debug.Log("playerInventory set to " + seedName);
            Debug.Log(playerInventory.SelectedSeed);
        }
        #endregion
    }
}
