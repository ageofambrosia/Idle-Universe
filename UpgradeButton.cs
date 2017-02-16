using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {

    public string itemName;
    public float cost;
    public int upgradePower;
    public bool idle;
    public int boughtCount;

    private float _newCost;
    private Text txt;

    void Start() {

        _newCost = cost;
        txt = GetComponentInChildren<Text>();

        cost = PlayerPrefs.GetFloat("cost:" + itemName, cost);
        boughtCount = PlayerPrefs.GetInt("boughCount:" + itemName, boughtCount);


    }

    //Save playerprefs.

    void Update() {

        txt.text = "Cost:" + cost + " " + boughtCount;

        if (GameManager.Instance.Points < cost) {
            GetComponent<Button>().interactable = false;
        } else
            GetComponent<Button>().interactable = true;


    }

    public void PurchaseUpgrade() {

        //If user has enough money to buy the upgrade.
        if (idle) {
            if (GameManager.Instance.Points >= cost) {

                GameManager.Instance.Points -= cost;
                GameManager.Instance.PointsPerSecond += upgradePower;
                boughtCount++;
                cost = Mathf.Round(cost * 1.35f);
                _newCost = Mathf.Pow(cost, _newCost = cost);
            }

        } else {
            if (GameManager.Instance.Points >= cost) {

                GameManager.Instance.Points -= cost;
                GameManager.Instance.PointsPerClick += upgradePower;
                boughtCount++;
                cost = Mathf.Round(cost * 1.35f);
                _newCost = Mathf.Pow(cost, _newCost = cost);

            }
        }


    }

    void OnApplicationPause(bool pauseStatus) {

        if (pauseStatus) {
            PlayerPrefs.SetFloat("cost:" + itemName, cost);
            PlayerPrefs.SetInt("boughCount:" + itemName, boughtCount);
        }


    }

    void OnDestroy() {
        PlayerPrefs.SetFloat("cost:" + itemName, cost);
        PlayerPrefs.SetInt("boughCount:" + itemName, boughtCount);
    }


}
