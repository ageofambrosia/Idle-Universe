using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

	public GameObject planetGO;
	public float orbitAmount;

	void Update() {
		transform.RotateAround(planetGO.transform.position, planetGO.transform.up, orbitAmount * Time.deltaTime);
	}
}
