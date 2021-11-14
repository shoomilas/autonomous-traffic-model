using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow3rdPerson : MonoBehaviour {
    public Transform player;
    public float rotationSmoothing;

    public float smoothing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        // transform.position = player.position; // very snappy
        transform.position = Vector3.Lerp(transform.position, player.position, smoothing);
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, rotationSmoothing); // Spherical interpolation
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
    }
}