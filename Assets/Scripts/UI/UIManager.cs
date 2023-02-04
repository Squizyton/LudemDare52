using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        [SerializeField] private GameObject player;

        [Header("Mode UI")] [SerializeField] private CanvasGroup fpsCanvasGroup;
        [SerializeField] private CanvasGroup topDownCanvasGroup;
        [SerializeField] private CanvasGroup tutorialCanvasGroup;

        [Header("Weapon Related Things")] [SerializeField]
        private CanvasGroup reloadGroup;

        [SerializeField] private UnityEngine.UI.Slider reloadSlider;
        [SerializeField] private TextMeshProUGUI ammoCount;
        [SerializeField] private CanvasGroup fpsCornIcon;
        [SerializeField] private CanvasGroup fpsCarrotIcon;
        [SerializeField] private CanvasGroup fpsMelonIcon;
        [SerializeField] private CanvasGroup fpsPepperIcon;
        [SerializeField] private GameObject hitIndicator;
        [SerializeField] private GameObject damageIndicator;

        [Header("Stats")] [SerializeField] private TextMeshProUGUI waveCount;
        [SerializeField] private TextMeshProUGUI remainingEnemies;
        [SerializeField] private TextMeshProUGUI timeAlive;

        [Header("Seeds")] [SerializeField] private TextMeshProUGUI cornSeedCount;
        [SerializeField] private TextMeshProUGUI carrotSeedCount;
        [SerializeField] private TextMeshProUGUI melonSeedCount;
        [SerializeField] private TextMeshProUGUI pepperSeedCount;
        [SerializeField] private TextMeshProUGUI starFruitSeedCount;
        [Header("Bullets")] [SerializeField] private TextMeshProUGUI cornBulletCount;
        [SerializeField] private TextMeshProUGUI carrotBulletCount;
        [SerializeField] private TextMeshProUGUI melonBulletCount;
        [SerializeField] private TextMeshProUGUI pepperBulletCount;

        [Header("Player Related Things")] [SerializeField]
        private UnityEngine.UI.Slider staminaSlider;

        [SerializeField] private Slider healthBar;
        [Header("Cow")] public GameObject cowText;


        [Header("Tutorial")] [SerializeField] private List<GameObject> tutorialList;
        private int tutorialIndex = 0;
        [SerializeField] private UnityEngine.UI.Button startWaveButton;

        [Header("Other")] [SerializeField] private GameObject harvestText;
        public Transform highlightObject;

        [Header("Button selection")]        public GameObject RTSSelectionButton;
        private bool isRTS;


        private void Awake()
        {
            Instance = this;
            UpdateStaminaSlider(100);
            SetHealth(100);
        }

        private void Start()
        {
            SelectSeeds("corn");
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
                    if (isRTS) isRTS = false;
                    break;

                case GameManager.CurrentMode.TopDown:
                    GameManager.Instance.grid.TurnOnCellUI();
                    topDownCanvasGroup.alpha = 1;
                    topDownCanvasGroup.interactable = true;
                    fpsCanvasGroup.alpha = 0;
                    if (!isRTS)
                    {
                        isRTS = true;
                        setSelectionOnButton();
                    }
                    break;

                case GameManager.CurrentMode.GameOver:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
            }
        }

        private void setSelectionOnButton()
        {
            if (topDownCanvasGroup.interactable == true)
            {
                //clear selected object
                EventSystem.current.SetSelectedGameObject(null);

                //set a new selected object
                EventSystem.current.SetSelectedGameObject(RTSSelectionButton);
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

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("EnemyCounter", numEnemies);
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
            HitIndicatorFadeOut hitIndicatorScript;
            hitIndicator.TryGetComponent(out hitIndicatorScript);
            if (hitIndicatorScript != null)
                hitIndicatorScript.StartFade();
        }

        public void TriggerKillIndicator()
        {
            HitIndicatorFadeOut hitIndicatorScript;
            hitIndicator.TryGetComponent(out hitIndicatorScript);
            if (hitIndicatorScript != null)
                hitIndicatorScript.StartFlash();
        }

        public void SetHealth(float newHealth)
        {
            healthBar.value = newHealth;
        }

        public void UpdateHealth()
        {
            healthBar.value = PlayerInventory.Instance.GetHealth();
        }

        private void UpdateHealthBar()
        {
            while (healthBar.value != PlayerInventory.Instance.GetHealth())
            {
                healthBar.value -= Time.deltaTime * 1.5f;
            }
        }

        public void TriggerDamageIndicator(Vector3 sourceDirection)
        {
            sourceDirection.y = 0;
            sourceDirection = sourceDirection.normalized;
            float angle = Vector3.SignedAngle(sourceDirection, player.transform.forward,
                Vector3.Cross(sourceDirection, player.transform.forward));
            damageIndicator.TryGetComponent<RectTransform>(out RectTransform dmgIndTransform);
            if (dmgIndTransform)
                dmgIndTransform.rotation = Quaternion.Euler(0, 0, angle);
            else
                Debug.Log("Error: TriggerDamageIndicator");
            //damageIndicator.TryGetComponent<CanvasGroup>(out CanvasGroup grpInd);
            //grpInd.alpha = 1;
            HitIndicatorFadeOut damageIndicatorScript;
            damageIndicator.TryGetComponent(out damageIndicatorScript);
            if (damageIndicatorScript != null)
                damageIndicatorScript.StartFade();
        }

        #endregion

        #region Reload

        public void ReloadGroupStatus(bool value, float reloadTime = 1f)
        {
            reloadGroup.alpha = value ? 1 : 0;
           
            reloadSlider.maxValue = reloadTime;
            Debug.Log(reloadSlider.value);
            reloadSlider.value = 0;
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
            var playerInventory = PlayerInventory.Instance;
            playerInventory.SetSeed(seedName);
        }

        public void UpdateSeedCount(PlantInfo plantInfo)
        {
            var playerInventory = PlayerInventory.Instance;
            var seedCount = playerInventory.GetSeedCount(plantInfo);

            switch (plantInfo.PlantName)
            {
                case "corn":
                    cornSeedCount.text = "x" + seedCount.ToString();
                    cornBulletCount.text = cornSeedCount.text;
                    break;
                case "pepper":
                    pepperSeedCount.text = "x" + seedCount.ToString();
                    pepperBulletCount.text = pepperSeedCount.text;
                    break;
                case "melon":
                    melonSeedCount.text = "x" + seedCount.ToString();
                    melonBulletCount.text = melonSeedCount.text;
                    break;
                case "carrot":
                    carrotSeedCount.text = "x" + seedCount.ToString();
                    carrotBulletCount.text = carrotSeedCount.text;
                    break;
                case "starfruit":
                    starFruitSeedCount.text = "x" + seedCount.ToString();
                    break;
            }
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
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Menu_Click");
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


        public void MoveSquare(Transform mTransform)
        {
            highlightObject.position = mTransform.position;
        }
    }
}