using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool shouldLoop = false;
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

            SetNewPointsToFollow();
            
            // var pointsToFollowIsNullOrEmpty =
            //     follower.PointsToFollow == null || follower.PointsToFollow.Count == 0; 
            //
            // if (pointsToFollowIsNullOrEmpty) {
            //     SetNewPointsToFollow();
            // }
        }

        private void SetNewPointsToFollow() {
            follower.PointsToFollow = new List<Transform>();
            var list = vehiclePathProvider.Provide(startNode).Select(_ => _.transform ).ToList();
            follower.PointsToFollow = list;  
        }

        private void Update() {
            OnVehiclePathFinished(OnPathFinishedNewLoopHandler);
        }

        public void OnVehiclePathFinished(Action toDo) {
            if (follower.CurrentDriveStatus == VehiclePointsListFollower.DriveStatus.Finished) {
                toDo(); 
            }
        }

        public void OnPathFinishedNewLoopHandler() {
            if (shouldLoop) {
                follower.CurrentPoint = follower.PointsToFollow[0];
                follower.CurrentDriveStatus = VehiclePointsListFollower.DriveStatus.Start;
            }
        }
    }
}