using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private GameObject currentPlanet;
    public GameObject greetingPanel;

    public Text pointsWhileOfflineText;
    public Text pointsText;
    public Text pointsPerClickText;
    public Text pointsPerSecondText;

    public float Points;
    public float PointsPerClick = 1;
    public float PointsPerSecond;

    private DateTime exitTime, EnterTime;
    public TimeSpan difference;

    void GetValuesFromPrefs() {
        Points = PlayerPrefs.GetFloat("Points", 0);
        PointsPerClick = PlayerPrefs.GetFloat("PerClick", 1);
        PointsPerSecond = PlayerPrefs.GetFloat("PerSecond", 0);
    }

    void GetIdleTime() {

        EnterTime = DateTime.Now;

        //Grab the old time from the player prefs as a long
        long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString", "0"));

        //Convert the old time from binary to a DataTime variable
        DateTime oldDate = DateTime.FromBinary(temp);

        if (oldDate == null) {
            oldDate = DateTime.MinValue;
        }
        //Use the Subtract method and store the result as a timespan variable
        difference = EnterTime.Subtract(oldDate);

    }

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

        GetValuesFromPrefs();
    }

    void Start() {

        // how much time has passed since logging off
        GetIdleTime();

        StartCoroutine(AddPointsPerSecond());

        //Get points from offline
        float pointsFromIdle = (float)difference.TotalSeconds * PointsPerSecond;
        Points += pointsFromIdle;

        //checks if first game(if you have 1 point when you enter the game, only on start hopefully).
        if (Points <= 1) {
            return;
        }
        //show greetingPanel and set text to the points collected while offline
        greetingPanel.SetActive(true);
        pointsWhileOfflineText.text = CurrencyToString("", pointsFromIdle);
    }


    void Update() {

        pointsText.text = CurrencyToString("Points: ", Points);
        pointsPerClickText.text = CurrencyToString("Per Click: ", PointsPerClick);
        pointsPerSecondText.text = CurrencyToString("Per Second: ", PointsPerSecond);

    }

    public string CurrencyToString(string text, float valueToConvert) {

        string converted = null;

        if (valueToConvert >= 1000) {
            converted = text + (valueToConvert / 1000f).ToString("f3") + " K";
        } else if (valueToConvert >= 1000000) {
            converted = text + (valueToConvert / 1000000f).ToString("f3") + " M";
        } else if (valueToConvert >= 1000000000000) {
            converted = text + (valueToConvert / 1000000000000).ToString("f3") + " B";
        } else {
            converted = text + valueToConvert.ToString("f0");
        }

        return converted;
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


    public void OnCloseGreetingPanelClick() {

        greetingPanel.SetActive(false);

    }
}
