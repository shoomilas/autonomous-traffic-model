using System;
using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using UnityEngine;

namespace PathCreator.Vehicles {
    [ExecuteInEditMode]
    public class Vehicle : MonoBehaviour {
        // TODO: Different implementations, one with PathFollower
        // TODO: one with vertexDriver
        // TODO: Takes / Generates a list of pathNodes,
        // TODO: Drives through them.
        
        // Vehicle should have: BoxCollider, RigidBody, 
        // VehicleController (providing driving-to-point / drviving-to-point-in-time methods)
        // PathFollower
        public IVehiclePathProvider vehiclePathProvider;   // TODO: Change to interface
        public VehiclePointsListFollower follower;
        public PathNode startNode;

        private void Awake() {
           
        }

        private void Start() {
            vehiclePathProvider ??= GetComponent<IVehiclePathProvider>();
            if (vehiclePathProvider == null) {
                vehiclePathProvider = gameObject.AddComponent<VehiclePathProvider>();
            }
            
            follower ??= GetComponent<VehiclePointsListFollower>();
            if (follower == null) {
                follower = gameObject.AddComponent<VehiclePointsListFollower>();
            }
            
            if (follower.PointsToFollow == null || follower.PointsToFollow.Count == 0) {
                Debug.Log("Iti s happeningingifgodfgdf");
                follower.PointsToFollow = new List<Transform>();
                var list = vehiclePathProvider.Provide(startNode).Select(_ => _.transform ).ToList();
                list.ForEach(_=>Debug.Log(_.name));
                follower.PointsToFollow = list;   
            }
        }
    }
}