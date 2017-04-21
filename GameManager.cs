using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Header("Panel for showing points since gone and prefab for click on planet")]
    public GameObject greetingPanel;
    public GameObject PointsOnClickPrefab;

    [Header("Settings Button and Panel Reference")]
    public GameObject settingsButton;
    public GameObject settingsPanel;


    [Header("\"My Canvas\" for planet view \"ZoomedOut\" for solar system")]
    public Transform myCanvas;
    public GameObject zoomedOutCanvasOverlay;

    [Header("Side bar stuff")]
    public GameObject[] SidePanelsArray;
    public ScrollRect sideBar;

    private Camera mainCam;
    [Header("The camera depth to lerp")]
    public float normalDepth;
    public float zoomedOutDepth;

    private float lastPlanetX;
    private float lastPlanetZ = -13;
    private int sideBarIndex;

    [Header("All of the texts")]
    public Text pointsWhileOfflineText;
    public Text pointsText;
    public Text pointsTextForSecondCanvas;
    public Text pointsPerClickText;
    public Text pointsPerSecondText;
    public Text planetNameText;
    public Text currentUnlockCostText;


    [Header("How fast to lerp")]
    public float timeTakenDuringLerp = 0.1f;

    //For lerping
    private float time;
    private bool _isLerping;
    private float _timeStartedLerping;
    private Vector3 _startPosition, _endPosition;

    [Header("Points")]
    public float Points;
    public float PointsPerClick = 1;
    public float PointsPerSecond;

    //For getting time since offline
    private DateTime exitTime, EnterTime;
    public TimeSpan difference;
    private string sysString;

    private string[] planetNames = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };
    [HideInInspector]
    public int planetInSolarViewIndex;

    public GameObject[] planetsGO;
    public bool[] unlockedState;
    public GameObject UnlockPlanetButton;
    public float[] unlockCost;

    void Awake() {

        if (Instance != null) {

            Debug.LogError("More than one GameManager in scene");
            Destroy(this.gameObject);
        }

        Instance = this;

        mainCam = Camera.main;

        Load();

        if (unlockedState.Length == 0) {
            unlockedState = new bool[8];
        }
        unlockedState[0] = true;
    }

    void Start() {
        //set the camera to the last planet position.
        mainCam.transform.position = new Vector3(lastPlanetX, mainCam.transform.position.y, lastPlanetZ);

        if (lastPlanetZ == zoomedOutDepth) {
            OnSolarSystemClick();
        }

        foreach (GameObject go in SidePanelsArray) {
            go.SetActive(false);
        }

        for (int i = 0; i < planetsGO.Length; i++) {

            if (unlockedState[i]) {
                planetsGO[i].transform.tag = "Planet";

            } else {
                planetsGO[i].transform.tag = "Locked";
            }
        }


        for (int i = 0; i < SidePanelsArray.Length; i++) {

            if (i == sideBarIndex) {

                SidePanelsArray[i].SetActive(true);
                sideBar.content = SidePanelsArray[i].GetComponent<RectTransform>();
            }
        }

        //Get how much time has passed since logging off
        GetIdleTime();

        StartCoroutine(AddPointsPerSecond());

        //Get points from offline
        float pointsFromIdle = (float)difference.TotalSeconds * PointsPerSecond / 3;
        Points += pointsFromIdle;

        if (pointsFromIdle > 0) {
            //show greetingPanel and set text to the points collected while offline
            greetingPanel.SetActive(true);
            pointsWhileOfflineText.text = CurrencyToString("", pointsFromIdle);
        }

        //Intial state
        settingsPanel.SetActive(false);
        settingsButton.SetActive(true);

    }

    void GetIdleTime() {

        EnterTime = DateTime.Now;

        //Grab the old time from the player prefs as a long
        long temp = Convert.ToInt64(sysString);

        //Convert the old time from binary to a DataTime variable
        DateTime oldDate = DateTime.FromBinary(temp);

        if (oldDate == null) {
            oldDate = DateTime.MinValue;
        }
        //Use the Subtract method and store the result as a timespan variable
        difference = EnterTime.Subtract(oldDate);
    }

    void Update() {

        pointsText.text = CurrencyToString("Points: ", Points);
        pointsTextForSecondCanvas.text = CurrencyToString("Points: ", Points);

        pointsPerClickText.text = CurrencyToString("Per Click: ", PointsPerClick);
        pointsPerSecondText.text = CurrencyToString("Per Second: ", PointsPerSecond);

        planetNameText.text = planetNames[planetInSolarViewIndex].ToString();
        currentUnlockCostText.text = CurrencyToString("Cost: ", unlockCost[planetInSolarViewIndex]);

        lastPlanetX = mainCam.transform.position.x;
        lastPlanetZ = mainCam.transform.position.z;

        if (mainCam.transform.position.z == zoomedOutDepth) {
            zoomedOutCanvasOverlay.gameObject.SetActive(true);

        } else {
            zoomedOutCanvasOverlay.gameObject.SetActive(false);
        }
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


    //Lerp Camera
    public void CameraLerp(Vector3 startPos, Vector3 endPos) {

        _isLerping = true;
        _timeStartedLerping = Time.time;

        _startPosition = startPos;
        _endPosition = endPos;

    }

    //add K,M,B endings to the raw numbers
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

    //Add points per click and per second
    public void AddPointsEachClick() {


        if (!myCanvas.gameObject.activeInHierarchy)
            return;

        Points += PointsPerClick;

        //Points Effect instantiation
        Vector3 pos = Input.mousePosition;
        pos.z = 0;

        GameObject prefab = Instantiate(PointsOnClickPrefab, pos, Quaternion.identity, myCanvas);
    }

    IEnumerator AddPointsPerSecond() {

        while (true) {
            Points += PointsPerSecond;

            yield return new WaitForSeconds(1);
        }
    }

    #region Buttons

    //Handle Button presses.
    public void OnCloseGreetingPanelClick() {

        greetingPanel.SetActive(false);
    }

    public void OnSolarSystemClick() {

        //disable canvas(easy way, change later) and zoom camera out.
        myCanvas.gameObject.SetActive(false);

        //Lerp to Solar system
        CameraLerp(mainCam.transform.position, new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, zoomedOutDepth));
    }

    public void ChangeSideBar(string nameOfPlanet) {

        foreach (GameObject go in SidePanelsArray) {
            go.SetActive(false);
        }

        for (int i = 0; i < SidePanelsArray.Length; i++) {

            if (SidePanelsArray[i].name == nameOfPlanet) {
                SidePanelsArray[i].SetActive(true);
                sideBar.content = SidePanelsArray[i].GetComponent<RectTransform>();
                sideBarIndex = i;
                return;
            }
        }

    }

    public void UnlockPlanet() {

        string planetName = planetNameText.text;

        //subtract cost from total points

        if (Points >= unlockCost[planetInSolarViewIndex]) {

            Points -= unlockCost[planetInSolarViewIndex];

            for (int i = 0; i < planetsGO.Length; i++) {
                if (planetsGO[i].name == planetName) {
                    planetsGO[i].transform.tag = "Planet";
                    unlockedState[i] = true;
                    UnlockPlanetButton.SetActive(false);
                    return;
                }
            }
        }

    }

    public void OnSettingsOpen() {
        //activate Panel(animations for later)
        settingsPanel.SetActive(true);
        settingsButton.SetActive(false);
    }

    public void OnSettingsClose() {
        settingsPanel.SetActive(false);
        settingsButton.SetActive(true);
    }

    #endregion

    #region Saving And Loading Data
    //Save Data
    void OnApplicationPause(bool pauseStatus) {

        if (pauseStatus) {
            sysString = System.DateTime.Now.ToBinary().ToString();
            Save();
        }
    }

    void OnDestroy() {

        sysString = System.DateTime.Now.ToBinary().ToString();
        Save();
    }


    public void Load() {

        if (File.Exists(Application.persistentDataPath + "/gameData.dat")) {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.dat", FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();
            Points = data.Points;
            PointsPerClick = data.PointsPerClick;
            PointsPerSecond = data.PointsPerSecond;
            lastPlanetX = data.lastPlanetX;
            lastPlanetZ = data.lastPlanetZ;
            sysString = data.sysString;
            planetInSolarViewIndex = data.planetIndex;
            sideBarIndex = data.sideBarIndex;
            unlockedState = data.unlockedState;
        }
    }

    public void Save() {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameData.dat");

        GameData data = new GameData();
        data.Points = Points;
        data.PointsPerClick = PointsPerClick;
        data.PointsPerSecond = PointsPerSecond;
        data.lastPlanetX = lastPlanetX;
        data.lastPlanetZ = lastPlanetZ;
        data.sysString = sysString;
        data.planetIndex = planetInSolarViewIndex;
        data.sideBarIndex = sideBarIndex;
        data.unlockedState = unlockedState;

        bf.Serialize(file, data);
        file.Close();
    }

    [Serializable]
    class GameData {

        public float Points;
        public float PointsPerClick;
        public float PointsPerSecond;

        public float lastPlanetX;
        public float lastPlanetZ;

        public string sysString;

        public int planetIndex;
        public int sideBarIndex;

        public bool[] unlockedState;
    }

    #endregion

}
