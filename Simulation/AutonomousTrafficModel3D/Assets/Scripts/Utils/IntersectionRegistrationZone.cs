using System;
using System.Collections;
using System.Collections.Generic;
using PathCreator.Vehicles;
using UnityEngine;

public class IntersectionRegistrationHandler : MonoBehaviour {
    private float size = 1;
    public float Size { get; }
    private IntersectionController controller;
    public void Start() {
        // var boxCollider = gameObject.InstantiateComponent<BoxCollider>();
        // boxCollider.size = Vector3.one * size;
        // boxCollider.isTrigger = true;
        var sphereCollider = gameObject.InstantiateComponent<SphereCollider>();
        sphereCollider.radius = size;
        sphereCollider.isTrigger = true;
    }
    public void OnTriggerEnter(Collider other) {
        if (other.GetComponent<IVehicle>() == null) {
            return;
        }

        var vehicle = (IVehicle)other;
        var entry = (vehicle, vehicle.ClosestTurnDirection);
        controller.IntersectionQueue.Add(entry);
    }
    public void OnTriggerExit(Collider other) {
        var vehicle = (IVehicle) other;
        var entry = (vehicle, vehicle.ClosestTurnDirection);
        controller.IntersectionQueue.Remove(entry);
        Debug.Log("Object exit");
    }
    
    // private void OnTriggerStay(Collider other) {
    //     Debug.Log("Object in trigger zone");
    // }
}
