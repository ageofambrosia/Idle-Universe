using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    private GameObject currentPlanet;

    public Text pointsText;
    public Text pointsPerClickText;
    public Text pointsPerSecondText;

    public float Points;
    public float PointsPerClick = 1;
    public float PointsPerSecond;

    private DateTime exitTime, EnterTime;

    public TimeSpan difference;

    void Awake() {
        if (Instance != null) {

            Debug.LogError("More than one GameManager in scene");
            Destroy(this);

        }
        Instance = this;

        currentPlanet = GameObject.FindGameObjectWithTag("Planet");
        if (currentPlanet == null) {
            Debug.LogError("No Planet Found!!");
            return;
        }

        Points = PlayerPrefs.GetFloat("Points", 0);
        PointsPerClick = PlayerPrefs.GetFloat("PerClick", 1);
        PointsPerSecond = PlayerPrefs.GetFloat("PerSecond", 0);

    }

    void Start() {

        EnterTime = DateTime.Now;

        //Grab the old time from the player prefs as a long
        long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));

        //Convert the old time from binary to a DataTime variable
        DateTime oldDate = DateTime.FromBinary(temp);

        if (oldDate == null) {
            oldDate = DateTime.MinValue;
        }
        //Use the Subtract method and store the result as a timespan variable
        difference = EnterTime.Subtract(oldDate);

        StartCoroutine(AddPointsPerSecond());

        float pointsFromIdle = difference.Seconds * PointsPerSecond;
        Points += pointsFromIdle;

    }


    public void AddPointsEachClick() {

        Points += PointsPerClick;

    }

    IEnumerator AddPointsPerSecond() {

        while (true) {
            Points += PointsPerSecond;

            yield return new WaitForSeconds(1);
        }
    }

    void Update() {

        pointsText.text = "Points: " + Points;
        pointsPerClickText.text = "Per Click: " + PointsPerClick;
        pointsPerSecondText.text = "Per Second :" + PointsPerSecond;

        //} else if (currentPlanet.transform.eulerAngles.z < 300 ||
        //    currentPlanet.transform.eulerAngles.z < -300) {
        //}
    }

    void OnApplicationPause(bool pauseStatus) {

        if (pauseStatus) {

            PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
            PlayerPrefs.SetFloat("Points", Points);
            PlayerPrefs.SetFloat("PerClick", PointsPerClick);
            PlayerPrefs.SetFloat("PerSecond", PointsPerSecond);

        }

    }

    void OnDestroy() {

        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetFloat("Points", Points);
        PlayerPrefs.SetFloat("PerClick", PointsPerClick);
        PlayerPrefs.SetFloat("PerSecond", PointsPerSecond);
    }

}
