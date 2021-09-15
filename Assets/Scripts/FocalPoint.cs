using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * FocalPoint.cs
 *
 * This script controls the rotation of the camera by rotating the FocalPoint object,
 * of which the camera is a child object, so the rotation of the FocalPoint object
 * will affect the rotation of the camera around said FocalPoint object.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class FocalPoint : MonoBehaviour
{

    public float turnSpeed = 20.0f;  // The speed at which the focal point should rotate

    void Update() {  // Update is called once per frame

        // Rotate the focal point based on horizontal user input.
        // Multiply by Time.deltaTime to account for variation in the game's
        // framerate.
        transform.Rotate(Vector3.down * turnSpeed * Input.GetAxis("Horizontal") * Time.deltaTime);

    }
}
