using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Controls the overall rotation of the each planet.
//If a planet rotates in any way -> handle from this script.

public class PlanetManager : MonoBehaviour {


    public GameObject myCanvas;
    public GameObject zoomedOutCanvas;

    private Camera mainCam;

    private RaycastHit hit;
    private Ray ray;

    void Awake() {

        mainCam = Camera.main;

    }

    void HandleInput() {

        if (Input.GetMouseButtonDown(0)) {

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {

                if (hit.transform.tag == "Planet") {

                    if (!myCanvas.activeInHierarchy) {

                        //Lerp To planet
                        GameManager.Instance.CameraLerp(mainCam.transform.position,
                            new Vector3(hit.transform.position.x, hit.transform.position.y,
                            GameManager.Instance.normalDepth - hit.transform.localScale.x));

                        zoomedOutCanvas.SetActive(false);

                        StartCoroutine("ActivateUI");

                        GameManager.Instance.ChangeSideBar(hit.transform.name);

                    } else
                        hit.transform.gameObject.GetComponent<Planet>().OnClickRotatePlanet();
                }
            }
        }
    }

    IEnumerator ActivateUI() {

        yield return new WaitForSeconds(0.7f);

        myCanvas.SetActive(true);

    }



    void Update() {

        HandleInput();

    }


}
