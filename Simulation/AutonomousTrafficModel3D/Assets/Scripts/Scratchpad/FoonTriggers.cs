using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PathCreation.Examples;
using UnityEngine;

public class FoonTriggers : MonoBehaviour {
    private PathFollower follower;
    
    private void OnTriggerEnter(Collider other) {
        // Debug.Log("Entered");
        // var boo = GetComponent<FoonTriggers>() as PathFollower;
        // var boo = GetComponent(other.name) as PathFollower;
        
        // var obj = new GameObject(other.name, typeof(PathFollower));
        // var rb = obj.GetComponent<PathFollower>();
        if(other.name == "BBBB") {
            other.GetComponentInParent<PathFollower>().speed = 20;
        }
        

    }
    
    private void OnTriggerStay(Collider other) {
        Debug.Log("STayyyys");
    }
    
    private void OnTriggerExit(Collider other) {
        Debug.Log("Object exit");
        other.GetComponentInParent<PathFollower>().speed = 5;
    }
}
