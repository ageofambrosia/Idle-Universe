using UnityEngine;

public class SpinSun : MonoBehaviour {

    public float rotateSpeed;

    void Update() {


        transform.RotateAround(Vector3.forward, rotateSpeed * Time.deltaTime);

    }
}
