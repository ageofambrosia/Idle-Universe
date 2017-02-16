using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkyBox : MonoBehaviour {

    //For Rotating Camera with skybox.
    //Constant rotation.

    public float rotSpeed = 2f;


    void Update() {

        transform.Rotate(-Vector3.up * rotSpeed * Time.deltaTime);

    }
}
