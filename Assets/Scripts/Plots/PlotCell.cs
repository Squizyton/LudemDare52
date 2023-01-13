using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace Plots
{
	public class PlotCell : MonoBehaviour
	{
		private int value; //Numerical display (replace with enum when I stop being dumb)

		private bool isMouseHovering; //Bools for UI state
		private bool isSelected;
		private bool isPlayerNear;

		private float timeElapsed; //Growth stuff/models
		[SerializeField] private Image mesh;
		[SerializeField] private PlantInfo plantInfo;
		[SerializeField] private GameObject player;

		[SerializeField] private Slider timeSlider;
		[SerializeField] private Image seedIcon;
		private PlayerInventory playerInventory;
		private GameObject plantModel;

		private Vector3 position;
		private Sprite defaultSprite;

		public bool beingChargedAt;
		public void Plant()
		{
			Debug.Log("Planting");
			if (!playerInventory.SelectedSeed)
			{
				FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_RTS_Error");

				Debug.Log("play error");
				return;
			}
			Debug.Log("Checking for seed");

			if (!playerInventory.RemoveSeed(playerInventory.SelectedSeed)) return;


			FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_RTS_Plant");
			Debug.Log("play sound + Planted " + playerInventory.SelectedSeed.name);

			plantInfo = playerInventory.SelectedSeed;
			value = 1;
			timeElapsed = 0;
			timeSlider.value = 0;
			seedIcon.sprite = plantInfo.seedIcon;
			timeSlider.maxValue = plantInfo.GrowTime;
			GameManager.Instance.cropsPlanted++;
			mesh.color = Color.white;
			mesh.sprite = plantInfo.seedIcon;
			plantModel = Instantiate(GameManager.Instance.sproutModel, this.transform.position, GameManager.Instance.sproutModel.transform.rotation);
		}

		public PlantInfo HarvestSeeds()
		{
			if (value != 2) return null;
			value = 0;

			UIManager.Instance.HarvestText(false);
			mesh.color = Color.white;
			mesh.sprite = defaultSprite;
			Destroy(plantModel);
			return plantInfo;
		}

		public bool IsGrowingSomething()
		{
			return value == 1;
		}

		public void ImageStatus(bool value)
		{
			mesh.enabled = value;
		}

		private void Start()
		{
			//TODO: Change this to something more performant
			player = FindObjectOfType<PlayerInventory>().gameObject;

			player.TryGetComponent(typeof(PlayerInventory), out Component inventory);
			if (inventory)
			{
				playerInventory = (PlayerInventory)inventory;
			}
			defaultSprite = mesh.sprite;
			position = transform.position;
		}

		private void Update()
		{
			// Only grows in state 1 during first-person mode
			if (value != 1 || GameManager.Instance.currentMode == GameManager.CurrentMode.TopDown) return;
			// Growth
			timeElapsed += Time.deltaTime;
			timeSlider.value = timeElapsed;
			if (timeElapsed > plantInfo.GrowTime)
			{

				Destroy(plantModel);
				plantModel = Instantiate(plantInfo.plantModel, position, plantInfo.plantModel.transform.rotation);
				timeElapsed = 0;
				value = 2;
				if (isPlayerNear)
				{
					mesh.color = Color.green;
				}
			}
		}

		public void DestroyCrop()
		{
			Destroy(plantModel);
			value = 0;
			mesh.color = Color.white;
			mesh.sprite = defaultSprite;
			plantInfo = null;
			TurnOnGrowingInfo(false);

		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.name != "Bean" || value != 2) return;


			UIManager.Instance.HarvestText(true);
			plantModel.GetComponent<Outline>().enabled = true;
			mesh.color = Color.green;
			isPlayerNear = true;
		}

		private void OnTriggerExit(Collider col)
		{
			if (col.name != "Bean" || value != 2) return;

			UIManager.Instance.HarvestText(false);
			plantModel.GetComponent<Outline>().enabled = false;
			mesh.color = Color.white;
			isPlayerNear = false;
		}

		private void OnMouseEnter()
		{
			isMouseHovering = true;
			if (value != 0 || GameManager.Instance.currentMode == GameManager.CurrentMode.FPS) return;
			mesh.color = Color.red;
		}

		private void OnMouseExit()
		{
			isMouseHovering = false;
			isSelected = false;
			mesh.color = Color.white;
		}

		private void OnMouseDown()
		{
			if (!isMouseHovering || GameManager.Instance.currentMode == GameManager.CurrentMode.FPS) return;
			isSelected = true;
			mesh.color = Color.yellow;
		}

		private void OnMouseUp()
		{
			if (!isSelected || GameManager.Instance.currentMode == GameManager.CurrentMode.FPS) return;

			if (value == 0)
			{
				Plant();
			}
		}

		public void TurnOnGrowingInfo(bool activeValue)
		{
			timeSlider.gameObject.SetActive(activeValue);
			seedIcon.gameObject.SetActive(activeValue);
		}

	}
}
