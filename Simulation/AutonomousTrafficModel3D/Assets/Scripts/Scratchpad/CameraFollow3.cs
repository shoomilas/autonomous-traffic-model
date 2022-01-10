using UnityEngine;

public class CameraFollow3 : MonoBehaviour {
    public Transform car;
    public float smoothing = 5f;
    public float z;
    private Vector3 offSet;

    // Use this for initialization
    private void Awake() {
        offSet = transform.position - car.position;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        var camPos = car.position + offSet;
        camPos.z = camPos.z - z;
        transform.position = Vector3.Lerp(transform.position, camPos, smoothing * Time.deltaTime);
    }
}