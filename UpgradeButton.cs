using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {

    public string itemName;
    public float cost;
    public float upgradePower;
    public bool idle;
    public GameObject spawnGO;
    public int boughtCount;
    public int maxBuyTimes;

    private float _newCost;
    private Text costTxt, upgradePowerTxt;
    //private Image upgradeProgressSlider;

    void Start() {
        _newCost = cost;
        costTxt = GetComponentInChildren<Text>();
        upgradePowerTxt = GetComponentsInChildren<Text>()[1];

        Load();

        if (boughtCount == maxBuyTimes && spawnGO != null) {

            spawnGO.SetActive(true);

        } else if (spawnGO != null)
            spawnGO.SetActive(false);
    }

    void Update() {

        costTxt.text = CurrencyToString("", cost);
        upgradePowerTxt.text = boughtCount.ToString();

        HandleButton();
    }

    void HandleButton() {

        if (boughtCount >= maxBuyTimes) {
            upgradePowerTxt.text = "Max!";
            GetComponent<Button>().interactable = false;
        } else if (GameManager.Instance.Points < cost) {
            GetComponent<Button>().interactable = false;
        } else
            GetComponent<Button>().interactable = true;

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
            converted = text + valueToConvert.ToString("over 1 B(handle..)");
        }
        return converted;
    }

    public void PurchaseUpgrade() {

        if (spawnGO != null && boughtCount == maxBuyTimes - 1) {

            spawnGO.SetActive(true);
        }

        string newString = "";

        newString = itemName.Substring(0, 4);

        if (newString == "Size") {

            if (GameManager.Instance.Points >= cost) {

                GameManager.Instance.Points -= cost;
                boughtCount++;

                GameObject planet = GameObject.Find(GetComponent<Button>().name);
                planet.GetComponent<Planet>().planetSize += upgradePower;
                planet.GetComponent<Planet>().Save(GetComponent<Button>().name);

                cost = Mathf.Round(cost * 1.45f);
                _newCost = Mathf.Pow(cost, _newCost = cost);
                return;
                //GameManager.Instance.PointsPerSecond += upgradePower;
            }
        }
        if (idle) {
            //If user has enough money to buy the upgrade.
            if (GameManager.Instance.Points >= cost) {

                GameManager.Instance.Points -= cost;
                GameManager.Instance.PointsPerSecond += upgradePower;
                boughtCount++;
                cost = Mathf.Round(cost * 1.45f);
                _newCost = Mathf.Pow(cost, _newCost = cost);
            }

        } else {
            if (GameManager.Instance.Points >= cost) {

                GameManager.Instance.Points -= cost;
                GameManager.Instance.PointsPerClick += upgradePower;
                boughtCount++;
                cost = Mathf.Round(cost * 1.45f);
                _newCost = Mathf.Pow(cost, _newCost = cost);

            }
        }


    }


    void OnApplicationPause(bool pauseStatus) {

        if (pauseStatus) {
            Save();
        }
    }
    void OnDestroy() {
        Save();
    }
    void OnEnable() {
        //sync changes
        Load();
    }
    void OnDisable() {
        Save();
    }

    //Load and Save
    void Load() {
        cost = PlayerPrefs.GetFloat("cost:" + itemName, cost);
        boughtCount = PlayerPrefs.GetInt("boughCount:" + itemName, boughtCount);
    }

    void Save() {
        PlayerPrefs.SetFloat("cost:" + itemName, cost);
        PlayerPrefs.SetInt("boughCount:" + itemName, boughtCount);
    }


}
