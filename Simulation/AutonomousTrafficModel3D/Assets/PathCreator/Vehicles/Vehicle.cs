﻿using System;
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
                follower.PointsToFollow = new List<Transform>();
                var list = vehiclePathProvider.Provide(startNode).Select(_ => _.transform ).ToList();
                follower.PointsToFollow = list;   
            }
        }

        private void Update() {
            if (follower.CurrentDriveStatus == VehiclePointsListFollower.DriveStatus.Finished) {
                follower.CurrentPoint = follower.PointsToFollow[0];
                follower.CurrentDriveStatus = VehiclePointsListFollower.DriveStatus.Start;
            }
        }
    }
}