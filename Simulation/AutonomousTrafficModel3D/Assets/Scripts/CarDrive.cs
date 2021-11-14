using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDrive : MonoBehaviour {
    public float speed;
    public float turnSpeed;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() // using FixedUpdate makes it smoother for physics calculation
    {
        if (Input.GetKey(KeyCode.W)) {
            Debug.Log("Pressed L");
            rb.AddRelativeForce(Vector3.forward * speed);
        }
        if (Input.GetKey(KeyCode.S)) {
            rb.AddRelativeForce(-Vector3.forward * speed);
        }
    }
}
