using UnityEngine;
using System.Collections;

public class PlanetSelectionScroll : MonoBehaviour {

    [Header("How fast to lerp")]
    public float timeTakenDuringLerp = 0.1f;

    //For lerping
    private float time;
    private bool _isLerping;
    private float _timeStartedLerping;
    private Vector3 _startPosition, _endPosition;

    public float minPos;
    public float maxPos;
    private Vector3 nextPos;

    private Camera cam;

    public float panSpeed = 5.0f;

    private bool bDragging;
    private Vector3 oldPos;
    private Vector3 panOrigin;
    private float maxDragAllowed;

    public GameObject previousButton, nextButton;

    void Start() {
        cam = GetComponent<Camera>();

        if (GameManager.Instance.unlockedState[GameManager.Instance.planetInSolarViewIndex]) {

            GameManager.Instance.UnlockPlanetButton.SetActive(false);
        } else {
            GameManager.Instance.UnlockPlanetButton.SetActive(true);
        }
    }

    void Update() {

        if (transform.position.x - 25 < minPos) {
            previousButton.SetActive(false);
        } else
            previousButton.SetActive(true);
        if (transform.position.x + 25 > maxPos) {
            nextButton.SetActive(false);
        } else
            nextButton.SetActive(true);

        Vector3 clampPos = transform.position;
        clampPos.x = Mathf.Clamp(transform.position.x, minPos, maxPos);
        transform.position = clampPos;

        if (_isLerping) {

            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            cam.transform.position = Vector3.Lerp(_startPosition, _endPosition, percentageComplete);

            if (percentageComplete >= 1.0f) {
                _isLerping = false;
            }
        }

    }

    public void CameraLerp(Vector3 startPos, Vector3 endPos) {

        _isLerping = true;
        _timeStartedLerping = Time.time;

        _startPosition = startPos;
        _endPosition = endPos;

    }

    public void PreviousPlanetClick() {

        if (_isLerping) return;

        Vector3 newPos = new Vector3(transform.position.x - 25, transform.position.y, transform.position.z);

        if (newPos.x <= minPos)
            previousButton.SetActive(false);

        if (nextPos.x < maxPos)
            nextButton.SetActive(true);

        CameraLerp(transform.position, newPos);
        StartCoroutine(ChangePlanetName(-1));

    }

    public void NextPlanetClick() {

        if (_isLerping) return;

        Vector3 newPos = new Vector3(transform.position.x + 25, transform.position.y, transform.position.z);

        if (newPos.x >= maxPos)
            nextButton.SetActive(false);

        if (newPos.x > minPos)
            previousButton.SetActive(true);

        CameraLerp(transform.position, newPos);
        StartCoroutine(ChangePlanetName(1));

    }

    IEnumerator ChangePlanetName(int amount) {

        yield return new WaitForSeconds(0.3f);

        GameManager.Instance.planetInSolarViewIndex += amount;


        if (GameManager.Instance.unlockedState[GameManager.Instance.planetInSolarViewIndex]) {

            GameManager.Instance.UnlockPlanetButton.SetActive(false);
        } else {
            GameManager.Instance.UnlockPlanetButton.SetActive(true);
        }

    }

}
