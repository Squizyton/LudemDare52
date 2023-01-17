using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	public float yOffset, xOffset;
	private void Start() //Makes sure the mouse is unlocked when you enter the main menu
	{
		Cursor.lockState = CursorLockMode.None;
	}

	void Update()
	{
		Vector2 mouseOffset = new Vector2(Screen.width / 2 - Input.mousePosition.x, Screen.height / 2 - Input.mousePosition.y);

		transform.localRotation = Quaternion.Euler(mouseOffset.y * 0.005f + yOffset, -mouseOffset.x * 0.005f + xOffset, 0);
	}

}
