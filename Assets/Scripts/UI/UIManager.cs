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
