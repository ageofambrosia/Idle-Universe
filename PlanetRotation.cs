using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Controls the overall rotation of the each planet.
//If a planet rotates in any way -> handle from this script.

public class PlanetRotation : MonoBehaviour {

    public float IdleSpinSpeed = 10;
    private int clickSpinAmount = 30;
    private Rigidbody rb;

    private RaycastHit hit;
    private Ray ray;

    void Awake() {

        rb = GetComponent<Rigidbody>();

    }

    void HandleInput() {

        if (Input.GetMouseButtonDown(0)) {


            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {

                if (hit.transform.tag == "Planet") {

                    OnClickRotatePlanet(clickSpinAmount);
                }
            }
        }
    }

    void OnClickRotatePlanet(float amount) {

        //If clicks are on UI, dont raycast under.
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0)) {
            return;
        }


        rb.AddRelativeTorque(Vector3.up * clickSpinAmount, ForceMode.Impulse);
        GameManager.Instance.AddPointsEachClick();
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
