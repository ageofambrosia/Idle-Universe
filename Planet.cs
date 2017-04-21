using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {


    public float IdleSpinSpeed = 10;

    public float planetSize;

    private int clickSpinAmount = 10;
    private Transform planetTransform;

    private Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        planetTransform = GetComponent<Transform>();
    }

    void Start() {

        string planetName = GetComponent<Transform>().name;
        planetSize = PlayerPrefs.GetFloat("sizeOf" + planetName, planetSize);

    }

    public void OnClickRotatePlanet() {

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

    void FixedUpdate() {

        if (GameManager.Instance.PointsPerSecond <= 0) {
            return;
        }
        RotatePlanetIdle();

    }

    private void Update() {

        planetTransform.localScale = new Vector3(planetSize, planetSize, planetSize);

    }

    public void Save(string nameOf) {

        PlayerPrefs.SetFloat("sizeOf" + nameOf, planetSize);

    }

}


