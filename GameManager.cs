using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public Text pointsText;

    [HideInInspector]
    public int Points;

    void Awake() {

        if (Instance != null) {

            Debug.LogError("More than one GameManager in scene");
            Destroy(this);

        }
        Instance = this;
    }

    void Update() {

        pointsText.text = Points.ToString();

    }
}
