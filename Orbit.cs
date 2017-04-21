using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

    public GameObject planetGO;
    public float orbitAmount;

    private float offSet;
    private Vector3 scale;
    private float distance;

    void Update() {

        distance = planetGO.transform.localScale.x - 0.5f;

        transform.position = (transform.position - planetGO.transform.position).normalized * distance + planetGO.transform.position;

        transform.RotateAround(planetGO.transform.position, planetGO.transform.up, orbitAmount * Time.deltaTime);

        transform.localScale = planetGO.transform.localScale;


    }
}
