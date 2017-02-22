using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {


	public float IdleSpinSpeed = 10;
	private int clickSpinAmount = 10;

	private Rigidbody rb;

	void Awake () {
		rb = GetComponent<Rigidbody>();

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

		RotatePlanetIdle();

	}

	void Update () {
		
	}
}
