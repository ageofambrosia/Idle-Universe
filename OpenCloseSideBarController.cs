using UnityEngine;

public class OpenCloseSideBarController : MonoBehaviour {

    public float fullyOpenBar;
    private float fullyClosedBar = 0;

    private RectTransform rect;
    public Transform slideBarHolderGO;
    private float time;

    public float timeTakenDuringLerp = 0.1f;
    private bool _isLerping;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _timeStartedLerping;


    void Awake() {
        rect = GetComponent<RectTransform>();
    }

    void Update() {

        Vector2 pos = slideBarHolderGO.position;
        pos.x = transform.position.x;
        slideBarHolderGO.position = pos;


        if (_isLerping) {

            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            rect.anchoredPosition = Vector3.Lerp(_startPosition, _endPosition, percentageComplete);

            if (percentageComplete >= 1.0f) {
                _isLerping = false;
            }
        }
    }

    public void OnDrag() {

        if (GameObject.Find("_GreetingPanel") != null) {
            return;
        }

        //Drag Side panel and clamp value.

        Vector3 pos = rect.anchoredPosition;
        pos = new Vector3(Input.mousePosition.x, pos.y, 0);
        pos.x = Mathf.Clamp(pos.x, 0, fullyOpenBar);
        rect.anchoredPosition = pos;
    }

    public void EndDrag() {

        // if > than fullyOpen/2 --> lerp to fully open and vice versa
        if (rect.anchoredPosition.x > fullyOpenBar / 3) {

            //to open
            StartLerping(fullyOpenBar);

        } else {

            //to close
            StartLerping(fullyClosedBar);
        }
    }

    void StartLerping(float direction) {

        _isLerping = true;
        _timeStartedLerping = Time.time;

        _startPosition = rect.anchoredPosition;

        _endPosition = new Vector3(direction, rect.anchoredPosition.y);

    }

}
