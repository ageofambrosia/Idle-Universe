using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Controls the overall rotation of the each planet.
//If a planet rotates in any way -> handle from this script.

public class PlanetRotation : MonoBehaviour {

    public float IdleSpinSpeed = 10;
    private int clickSpinAmount = 100;
    private Rigidbody rb;

    void Start() {

        rb = GetComponent<Rigidbody>();

    }

    void HandleInput() {

        if (Input.GetMouseButtonDown(0)) {

            OnClickRotatePlanet(clickSpinAmount);


        }

    }

    void OnClickRotatePlanet(float amount) {

        rb.AddRelativeTorque(Vector3.up * clickSpinAmount, ForceMode.Impulse);
        GameManager.Instance.Points += 1;
    }

    void RotatePlanetIdle() {

        rb.AddRelativeTorque(Vector3.up * IdleSpinSpeed);

    }

    void Update() {

        HandleInput();

    }

    void FixedUpdate() {

        RotatePlanetIdle();


    }

}
