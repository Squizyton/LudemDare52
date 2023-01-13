using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	private PlayerControls controls;


	[Header("Movement Values")]
	[SerializeField] private float runSpeed = 5f;
	[SerializeField] private float sprintSpeed = 5f;
	[SerializeField] private float jumpForce = 5f;
	
	[Header("Ground Check")]
	[SerializeField] private Rigidbody rb;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float RaycastDistance = 0.5f;
	
	[Header("Stamina")]
	[SerializeField] private float staminaAmount;
	[SerializeField] private float staminaDrain = 10f;
	[SerializeField] private float staminaMax = 100f;
	private bool exhausted;
	
    private bool isWalking;
	private bool isMovement;

    private FMOD.Studio.EventInstance FMODPlayerWalk;

    [HideInInspector]
	public Vector2 movePos;
	[HideInInspector]
	public bool canJump;
	private bool hadJumped;
	private bool sprinting;
	private float currentSpeed;
	private Coroutine staminaCoroutine;
	private WaitForSeconds staminaCooldown;
	private WaitForSeconds staminaRegenTic;


    private void Start()
	{
		staminaCooldown = new WaitForSeconds(1f);
		staminaRegenTic = new WaitForSeconds(0.02f);
		staminaAmount = 100;
		UIManager.Instance.UpdateStaminaSlider(staminaAmount);

        controls = new PlayerControls();
		controls.Player.Movement.performed += GetMovement;
		controls.Player.Movement.canceled += GetMovement;
        controls.Player.Movement.performed += ctx => isMovement = true;
        controls.Player.Movement.canceled += ctx => isMovement = false;
        controls.Player.Sprint.performed += GetSprint;
		controls.Player.Sprint.canceled += GetSprint;
		controls.Enable();
		staminaAmount = staminaMax;
        FMODPlayerWalk = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Movement/Player_Footsteps");
    }

	private void GetMovement(InputAction.CallbackContext context)
	{
        movePos = context.ReadValue<Vector2>();
	}
	private void GetSprint(InputAction.CallbackContext context)
	{
		if (exhausted) return;
		
		sprinting = context.ReadValueAsButton();

		if(sprinting && staminaCoroutine != null)
		{
            StopCoroutine(staminaCoroutine);
			staminaCoroutine = null;
            exhausted = false;
        }


		if (sprinting || staminaCoroutine != null) return;
	
		staminaCoroutine = StartCoroutine(RechargeStamina());
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
	        if (rb.velocity.y > 3) return;
			
			if (!Physics.Raycast(transform.position, Vector3.down, RaycastDistance, groundLayer)) return;

			if (!canJump && hadJumped)
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

		if (sprinting)
		{
            staminaAmount -= staminaDrain * Time.deltaTime;
            UIManager.Instance.UpdateStaminaSlider(staminaAmount);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("IsSprinting", "Sprinting");
        }
        else
            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("IsSprinting", "Walking");

        if (staminaAmount <= 0 && staminaCoroutine == null)
        {
            staminaAmount = 0;
            exhausted = true;
            sprinting = false;
            staminaCoroutine = StartCoroutine(RechargeStamina());
        }

        //Get the velocity of the player
        var playerVelocity = new Vector3(movePos.x * currentSpeed, rb.velocity.y, movePos.y * currentSpeed);
        //Set the velocity of the player based on the velocity * transform.foward
        rb.velocity = transform.TransformDirection(playerVelocity);

		if (isMovement)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Speed", currentSpeed);

            if (!isWalking && canJump)			
				PlayerMoveSFX();
        }
        else
			if(isWalking ||!canJump)
				PlayerStopMoveSFX();
    }

    private void Jump()
    {
        if (!canJump) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        PlayerJumpSFX();
        hadJumped = true;
        canJump = false;
    }


    private IEnumerator RechargeStamina()
	{
		yield return staminaCooldown;
		
		while ((int)staminaAmount != (int)staminaMax)
		{
			staminaAmount += staminaDrain * Time.deltaTime;
			UIManager.Instance.UpdateStaminaSlider(staminaAmount);
			yield return staminaRegenTic;
		}
		
		if(exhausted)
			exhausted = false;

		staminaCoroutine = null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Vector3.down * RaycastDistance);
	}

	#region Sound
	private void PlayerMoveSFX()
    {
        FMODPlayerWalk.start();
		isWalking = true;
        return;
    }

    private void PlayerStopMoveSFX()
    {
        //stop
        FMODPlayerWalk.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isWalking = false;
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
    #endregion
}