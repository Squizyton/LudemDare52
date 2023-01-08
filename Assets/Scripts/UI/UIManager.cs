using System;
using System.Collections.Generic;
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
        [SerializeField] private CanvasGroup tutorialCanvasGroup;
        
        [Header("Weapon Related Things")] [SerializeField]
        private CanvasGroup reloadGroup;
        [SerializeField]private Slider reloadSlider;
        private PlayerInventory playerInventory;


        [Header("Player Related Things")] [SerializeField]
        private Slider staminaSlider;
        
        [Header("Cow")]
        public GameObject cowText;

        
        [Header("Tutorial")]
        [SerializeField]private List<GameObject> tutorialList;
        private int tutorialIndex = 0;
        [SerializeField] private Button startWaveButton;
        private void Awake()
        {
            Instance = this;

            //TODO: Change this to something more performant
            var playerObj = FindObjectOfType<PlayerInventory>().gameObject;
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
                    GameManager.Instance.grid.TurnOffCellUI();
                    fpsCanvasGroup.alpha = 1;
                    topDownCanvasGroup.alpha = 0;
                    topDownCanvasGroup.interactable = false;
                    break;
                case GameManager.CurrentMode.TopDown:
                    GameManager.Instance.grid.TurnOnCellUI();
                    topDownCanvasGroup.alpha = 1;
                    topDownCanvasGroup.interactable = true;
                    fpsCanvasGroup.alpha = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
            }
        }

        #endregion

        #region Cow

        public void SetCowText(bool value)
        {
            cowText.SetActive(value);
        }


        #endregion

        #region Player Related
        public void UpdateStaminaSlider(float stamina)
        {
            staminaSlider.value = stamina;
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
        
        
        #region Tutorial

        public void StartTutorial()
        {
            tutorialList[tutorialIndex].SetActive(true);
            startWaveButton.enabled = false;
        }
        
        public void NextTutorial()
        {
            if (tutorialIndex < tutorialList.Count - 1)
            {
                //Set the previous tutorial to false
                tutorialList[tutorialIndex].SetActive(false);
                tutorialIndex++;
                //Set the next tutorial to true
                tutorialList[tutorialIndex].SetActive(true);
            }
            else
            {
                EndTutorial();
            }

        }
        
        public void EndTutorial()
        {
            tutorialList[tutorialIndex].SetActive(false);
            tutorialIndex = 0;
            startWaveButton.enabled = true;
            GameManager.Instance.EndTutorial();
        }

        #endregion
    }
}
