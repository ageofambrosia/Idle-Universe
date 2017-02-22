using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private GameObject currentPlanet;
    public GameObject greetingPanel;
    public GameObject PointsOnClickPrefab;

    public Transform myCanvas;
    public Transform zoomedOutCanvas;

    private Camera mainCam;
    public float normalDepth, zoomedOutDepth;
    private float lastPlanetX, lastPlanetZ;

    public Text pointsWhileOfflineText;
    public Text pointsText;
    public Text pointsPerClickText;
    public Text pointsPerSecondText;

    private float time;
    public float timeTakenDuringLerp = 0.1f;
    private bool _isLerping;
    private float _timeStartedLerping;
    private Vector3 _startPosition, _endPosition;

    public float Points;
    public float PointsPerClick = 1;
    public float PointsPerSecond;

    private DateTime exitTime, EnterTime;
    public TimeSpan difference;

    void GetValuesFromPrefs() {
        Points = PlayerPrefs.GetFloat("Points", 0);
        PointsPerClick = PlayerPrefs.GetFloat("PerClick", 1);
        PointsPerSecond = PlayerPrefs.GetFloat("PerSecond", 0);

        lastPlanetX = PlayerPrefs.GetFloat("lastX", 0);
        lastPlanetZ = PlayerPrefs.GetFloat("lastZ", -10);
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

        mainCam = Camera.main;
    }

    void Start() {

        mainCam.transform.position = new Vector3(lastPlanetX, mainCam.transform.position.y, lastPlanetZ);

        // how much time has passed since logging off
        GetIdleTime();

        StartCoroutine(AddPointsPerSecond());

        zoomedOutCanvas.gameObject.SetActive(false);

        //Get points from offline
        float pointsFromIdle = (float)difference.TotalSeconds * PointsPerSecond;
        Points += pointsFromIdle;

        if (pointsFromIdle > 0) {
            //show greetingPanel and set text to the points collected while offline
            greetingPanel.SetActive(true);
            pointsWhileOfflineText.text = CurrencyToString("", pointsFromIdle);
        }
    }

    void Update() {

        pointsText.text = CurrencyToString("Points: ", Points);
        pointsPerClickText.text = CurrencyToString("Per Click: ", PointsPerClick);
        pointsPerSecondText.text = CurrencyToString("Per Second: ", PointsPerSecond);


        //Lerp
        if (_isLerping) {

            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            mainCam.transform.position = Vector3.Lerp(_startPosition, _endPosition, percentageComplete);

            if (percentageComplete >= 1.0f) {
                _isLerping = false;
            }
        }

    }

    public string CurrencyToString(string text, float valueToConvert) {

        string converted = null;

        if (valueToConvert < 1000) {
            converted = text + valueToConvert.ToString("f0");
        } else if (valueToConvert >= 1000 && valueToConvert <= 1000000) {
            converted = text + (valueToConvert / 1000f).ToString("f2") + " K";
        } else if (valueToConvert >= 1000000 && valueToConvert <= 1000000000) {
            converted = text + (valueToConvert / 1000000f).ToString("f2") + " M";
        } else if (valueToConvert >= 1000000000) {
            converted = text + (valueToConvert / 1000000000).ToString("f2") + " B";
        } else {
            //value over 1B
            converted = text + valueToConvert.ToString("over 1 B(Handle)");
        }
        return converted;
    }

    public void AddPointsEachClick() {


        if (!myCanvas.gameObject.activeInHierarchy)
            return;

        Points += PointsPerClick;

        //Points Effect instantiation
        Vector3 pos = Input.mousePosition;
        pos.z = 0;

        GameObject prefab = (GameObject)Instantiate(PointsOnClickPrefab, pos, Quaternion.identity, myCanvas);

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

            PlayerPrefs.SetFloat("lastX", mainCam.transform.position.x);
            PlayerPrefs.SetFloat("lastZ", mainCam.transform.position.z);

        }
    }

    void OnDestroy() {

        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetFloat("Points", Points);
        PlayerPrefs.SetFloat("PerClick", PointsPerClick);
        PlayerPrefs.SetFloat("PerSecond", PointsPerSecond);

        PlayerPrefs.SetFloat("lastX", mainCam.transform.position.x);
        PlayerPrefs.SetFloat("lastZ", mainCam.transform.position.z);
    }


    //Handle Button presses.
    public void OnCloseGreetingPanelClick() {

        greetingPanel.SetActive(false);
    }

    public void OnPlanetSelectClick() {

        //disable canvas(easy way, change later) and zoom camera out.
        myCanvas.gameObject.SetActive(false);

        //Lerp to Solar system
        CameraLerp(mainCam.transform.position, new Vector3(0, mainCam.transform.position.y, zoomedOutDepth));

        StartCoroutine("ActivateZoomedOutCanvas");
    }

    public void CameraLerp(Vector3 startPos, Vector3 endPos) {

        _isLerping = true;
        _timeStartedLerping = Time.time;

        _startPosition = startPos;
        _endPosition = endPos;

    }

    IEnumerator ActivateZoomedOutCanvas() {

        //activate second canvas
        yield return new WaitForSeconds(0.7f);

        zoomedOutCanvas.gameObject.SetActive(true);

    }

}
