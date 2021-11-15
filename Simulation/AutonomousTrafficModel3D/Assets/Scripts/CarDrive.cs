using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDrive : MonoBehaviour {
    public float speed = 90;
    public float turnSpeed = 65;
    public float gravityMultiplier = 3;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() // using FixedUpdate makes it smoother for physics calculation
    {
        Turn();
        Move();
        Fall();
        
        // if (Input.GetKey(KeyCode.W)) {
        //     rb.AddRelativeForce(Vector3.forward * speed);
        // }
        // if (Input.GetKey(KeyCode.S)) {
        //     rb.AddRelativeForce(-Vector3.forward * speed);
        // }
        // if (Input.GetKey(KeyCode.D)) {
        //     rb.AddTorque(Vector3.up * turnSpeed);
        // }
        // else if (Input.GetKey(KeyCode.A)) {
        //     rb.AddTorque(- Vector3.up * turnSpeed);
        // }
        // rb.AddForce(Vector3.down * gravityMultiplier);

    }

    void Move() {
        if (Input.GetKey(KeyCode.W)) {
            // rb.AddRelativeForce(Vector3.forward * speed);
            rb.AddRelativeForce(new Vector3(Vector3.forward.x, 0, Vector3.forward.z) * speed);
        }
        if (Input.GetKey(KeyCode.S)) {
            rb.AddRelativeForce(-Vector3.forward * speed);
        }

        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0;
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    void Turn() {
        if (Input.GetKey(KeyCode.D)) {
            rb.AddTorque(Vector3.up * turnSpeed);
        }
        else if (Input.GetKey(KeyCode.A)) {
            rb.AddTorque(- Vector3.up * turnSpeed);
        }
    }

    void Fall() {
        rb.AddForce(Vector3.down * gravityMultiplier);
    }
}
