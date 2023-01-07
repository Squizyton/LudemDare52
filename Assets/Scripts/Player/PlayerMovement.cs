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
    
    private Vector2 movePos;
    private bool canJump;
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
    }

    private void GetMovement(InputAction.CallbackContext context)
    {
        movePos = context.ReadValue<Vector2>();
    }
    private void GetSprint(InputAction.CallbackContext context)
    {
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
        if (controls.Player.Jump.triggered)
        {
            Jump();
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.down, RaycastDistance, groundLayer))
            {
                if (!canJump)
                {
                    PlayerLandSFX();
                }
                canJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        currentSpeed = sprinting ? sprintSpeed : runSpeed;

        //Get the velocity of the player
        var playerVelocity = new Vector3(movePos.x * currentSpeed, rb.velocity.y, movePos.y * currentSpeed);
        //Set the velocity of the player based on the velocity * transform.foward
        rb.velocity = transform.TransformDirection(playerVelocity);
        if(rb.velocity.x != 0 || rb.velocity.z != 0)
        {
            PlayerMoveSFX();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * RaycastDistance);
    }

    //Note: Do these need to be public?
    public void PlayerMoveSFX()
    {
        return;
    }

    public void PlayerSprintSFX()
    {
        return;
    }

    public void PlayerJumpSFX()
    {
        return;
    }

    public void PlayerLandSFX()
    {
        return;
    }
}