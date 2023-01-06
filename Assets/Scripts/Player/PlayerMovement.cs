using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;


    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 movePos;
    private bool canJump;
    [SerializeField] private float RaycastDistance = 0.5f;

    private void Start()
    {
        controls = new PlayerControls();
        controls.Player.Movement.performed += GetMovement;
        controls.Player.Movement.canceled += GetMovement;
        controls.Enable();
    }

    private void GetMovement(InputAction.CallbackContext context)
    {
        movePos = context.ReadValue<Vector2>();
    }

    private void Jump()
    {
        if (!canJump) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
                canJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //Move Player based movePos

        //Get the velocity of the player
        var playerVelocity = new Vector3(movePos.x * speed, rb.velocity.y, movePos.y * speed);
        //Set the velocity of the player based on the velocity * transform.foward
        rb.velocity = transform.TransformDirection(playerVelocity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * RaycastDistance);
    }
}