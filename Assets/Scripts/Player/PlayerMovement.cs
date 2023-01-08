using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	private PlayerControls controls;


	[SerializeField] private float runSpeed = 5f;
	[SerializeField] private float sprintSpeed = 5f;
	[SerializeField] private float jumpForce = 5f;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float RaycastDistance = 0.5f;

    private bool isWalking;

    private FMOD.Studio.EventInstance FMODPlayerWalk;

    [HideInInspector]
	public Vector2 movePos;
	[HideInInspector]
	public bool canJump;
	private bool sprinting;
	private float currentSpeed;

	private void Start()
	{
		controls = new PlayerControls();
		controls.Player.Movement.performed += GetMovement;
		controls.Player.Movement.canceled += GetMovement;
		controls.Player.Sprint.performed += GetSprint;
		controls.Player.Sprint.canceled += GetSprint;
		controls.Enable();

        FMODPlayerWalk = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Movement/Player_FootSteps");
    }

	private void GetMovement(InputAction.CallbackContext context)
	{
		FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("IsSprinting", "Walking");
		PlayerMoveSFX();


        movePos = context.ReadValue<Vector2>();
	}
	private void GetSprint(InputAction.CallbackContext context)
	{
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("IsSprinting", "Sprinting");
		PlayerMoveSFX();
        Debug.Log(context.ReadValueAsButton());
		sprinting = context.ReadValueAsButton();
	}


	private void Jump()
	{
		if (!canJump) return;

		rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		PlayerJumpSFX();
		canJump = false;
	}

	private void Update()
	{

		//If we are in top down mode, don't allow anyting else to happen
		if (GameManager.Instance.currentMode == GameManager.CurrentMode.TopDown) return;

        if (controls.Player.Jump.triggered)
		{
			Jump();
		}
		else
		{
			if (!Physics.Raycast(transform.position, Vector3.down, RaycastDistance, groundLayer)) return;

			if (!canJump)
			{
				PlayerLandSFX();
			}
			canJump = true;
		}
	}

	private void FixedUpdate()
	{

		if (GameManager.Instance.currentMode == GameManager.CurrentMode.TopDown) return;

		currentSpeed = sprinting ? sprintSpeed : runSpeed;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Speed", currentSpeed);

        //Get the velocity of the player
        var playerVelocity = new Vector3(movePos.x * currentSpeed, rb.velocity.y, movePos.y * currentSpeed);
		//Set the velocity of the player based on the velocity * transform.foward
		rb.velocity = transform.TransformDirection(playerVelocity);

		//system thinks player is moving while there is no input
		if (rb.velocity.x != 0 || rb.velocity.z != 0)
		{
			//PlayerMoveSFX();
        }
        else
            PlayerStopMoveSFX(); 

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Vector3.down * RaycastDistance);
	}
    private void PlayerMoveSFX()
    {
        //play
        if (!isWalking)
        {
            FMODPlayerWalk.start();
            isWalking = true;
        }
        return;
    }

    private void PlayerStopMoveSFX()
    {
        //stop
        if (isWalking)
        {
            FMODPlayerWalk.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isWalking = false;
        }
        return;
    }

    private void PlayerJumpSFX()
	{
		PlayerStopMoveSFX();

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Movement/Player_Jump");
        return;
	}

	private void PlayerLandSFX()
	{
		FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Movement/Player_Land");
		return;
	}
}