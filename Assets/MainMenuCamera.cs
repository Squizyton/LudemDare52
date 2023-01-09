using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	[Range(0.0f, 10.0f)]
	public float rotationSpeed = 0.0f;
	public float yOffset, xOffset;
	private void Start() //Makes sure the mouse is unlocked when you enter the main menu
	{
		Cursor.lockState = CursorLockMode.None;
	}

	//Slowly rotates the camera. Not needed anymore now that it's just an image

	void Update()
	{
		Vector2 mouseOffset = new Vector2(Screen.width / 2 - Input.mousePosition.x, Screen.height / 2 - Input.mousePosition.y);

		transform.localRotation = Quaternion.Euler(mouseOffset.y * 0.005f + yOffset, -mouseOffset.x * 0.005f + xOffset, 0);
	}

}
