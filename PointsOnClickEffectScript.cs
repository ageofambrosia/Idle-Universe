using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsOnClickEffectScript : MonoBehaviour {

    private float fadeTime;
    private Text text;
    public Color endColor;

    void Awake() {

        text = GetComponent<Text>();

        text.fontSize = Random.Range(90, 115);
        transform.position += new Vector3(Random.Range(-45, 45), Random.Range(-45, 45));
        fadeTime = Random.Range(3f, 4f);

        text.text = "+" + GameManager.Instance.PointsPerClick;

        Destroy(this.gameObject, fadeTime);
    }

    void Update() {

        text.color = Color.LerpUnclamped(text.color, endColor, Time.deltaTime * fadeTime);
    }
}
