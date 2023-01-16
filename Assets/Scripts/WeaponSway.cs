using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    public float amount;
    public float smoothAmount;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        //I think I stole this from a YouTube tutorial. Works nicely though. Just make sure you don't attach this script to the same object as the SineSway.cs, or else fucky wucky will happen
        var movementX = -Input.GetAxisRaw("Mouse X") * amount;
        var movementY = -Input.GetAxisRaw("Mouse Y") * amount;

        var finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }
}
