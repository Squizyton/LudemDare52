using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        [SerializeField] private GameObject player;

        [Header("Mode UI")]
        [SerializeField] private CanvasGroup fpsCanvasGroup;
        [SerializeField] private CanvasGroup topDownCanvasGroup;
        [SerializeField] private CanvasGroup tutorialCanvasGroup;

        [Header("Weapon Related Things")][SerializeField]
        private CanvasGroup reloadGroup;
        [SerializeField] private UnityEngine.UI.Slider reloadSlider;
        [SerializeField] private TextMeshProUGUI ammoCount;
        [SerializeField] private CanvasGroup fpsCornIcon;
        [SerializeField] private CanvasGroup fpsCarrotIcon;
        [SerializeField] private CanvasGroup fpsMelonIcon;
        [SerializeField] private CanvasGroup fpsPepperIcon;
        [SerializeField] private CanvasGroup hitIndicator;
        [SerializeField] private GameObject damageIndicator;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI waveCount;
        [SerializeField] private TextMeshProUGUI remainingEnemies;
        [SerializeField] private TextMeshProUGUI timeAlive;


        [Header("Player Related Things")][SerializeField]
        private UnityEngine.UI.Slider staminaSlider;

        [Header("Cow")]
        public GameObject cowText;


        [Header("Tutorial")]
        [SerializeField] private List<GameObject> tutorialList;
        private int tutorialIndex = 0;
        [SerializeField] private UnityEngine.UI.Button startWaveButton;

        [Header("Other")] [SerializeField] private GameObject harvestText;
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

        #region Stats
        public void UpdateWaveCount(int wave)
        {
            waveCount.text = "Wave: " + wave.ToString();
        }

        public void UpdateEnemiesRemaining(int numEnemies)
        {
            remainingEnemies.text = "Enemies: " + numEnemies.ToString();
        }

        public void UpdateTimeAlive(float newTimeAlive)
        {
            //Convert to minutes and seconds
            var minutes = Mathf.FloorToInt(newTimeAlive / 60F);
            var seconds = Mathf.FloorToInt(newTimeAlive - minutes * 60);
            timeAlive.text = "Time Alive: " + string.Format("{0:0}:{1:00}", minutes, seconds);
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

        public void TriggerHitIndicator()
        {
            hitIndicator.alpha = 1;
        }

        public void TriggerDamageIndicator(Vector3 sourceDirection)
        {
            sourceDirection.y = 0;
            sourceDirection = sourceDirection.normalized;
            float angle = Vector3.SignedAngle(sourceDirection, player.transform.forward, Vector3.Cross(sourceDirection, player.transform.forward));
            damageIndicator.TryGetComponent<RectTransform>(out RectTransform dmgIndTransform);
            if (dmgIndTransform)
                dmgIndTransform.rotation = Quaternion.Euler(0, 0, angle);
            else
                Debug.Log("Error: TriggerDamageIndicator");
            damageIndicator.TryGetComponent<CanvasGroup>(out CanvasGroup grpInd);
            grpInd.alpha = 1;
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

        #region Ammo
        public void UpdateAmmoCount(int inMagazine, int inSack, bool isInfinite)
        {
            if (isInfinite)
            {
                ammoCount.text = inMagazine.ToString() + " / ∞";
                return;
            }

            ammoCount.text = inMagazine.ToString() + " / " + inSack.ToString();
        }

        public void UpdateAmmoType(PlantInfo plantInfo)
        {
            fpsCornIcon.alpha = 0;
            fpsCarrotIcon.alpha = 0;
            fpsMelonIcon.alpha = 0;
            fpsPepperIcon.alpha = 0;

            switch (plantInfo.PlantName)
            {
                case "corn":
                    fpsCornIcon.alpha = 1;
                    break;
                case "carrot":
                    fpsCarrotIcon.alpha = 1;
                    break;
                case "pepper":
                    fpsPepperIcon.alpha = 1;
                    break;
                case "melon":
                    fpsMelonIcon.alpha = 1;
                    break;
            }
        }
        #endregion

        #region SeedMenu
        public void SelectSeeds(string seedName)
        {
            PlayerInventory playerInventory = PlayerInventory.Instance;
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
        
        public void HarvestText(bool value)
        {
           harvestText.SetActive(value);
        }
    }
}
