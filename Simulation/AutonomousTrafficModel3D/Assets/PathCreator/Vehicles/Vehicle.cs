using System;
using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using UnityEngine;

namespace PathCreator.Vehicles {
    public static class Extensions {
        public static T InstantiateComponent<T>(this GameObject gameObject) where T: Component {
            var component = gameObject.GetComponent<T>();
            if (component == null) {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }

    
    public class Vehicle : MonoBehaviour {
        // TODO: Different implementations, one with PathFollower
        // TODO: one with vertexDriver
        // TODO: Takes / Generates a list of pathNodes,
        // TODO: Drives through them.
        
        // Vehicle should have: BoxCollider, RigidBody, 
        // VehicleController (providing driving-to-point / drviving-to-point-in-time methods)
        public bool shouldLoop = false;
        public bool generateNewPathOnLoopFinished = false;
        public PathNode startNode;
        private PathProviderMethod providerMethod = PathProviderMethod.AlwaysRandomRightForward;

        public PathProviderMethod ProviderMethod
        {
            get => providerMethod;
            set
            {
                InstantiateComponents();
                providerMethod = value;
                vehiclePathProvider.CurrentMethod = providerMethod;
                SetNewPointsToFollow();
            }
        }
        
        [HideInInspector] public IVehiclePathProvider vehiclePathProvider;   // TODO: Change to interface
        [HideInInspector] public VehiclePointsListFollower follower;
        [HideInInspector] public Rigidbody rigidBody;
        [HideInInspector] public BoxCollider boxCollider;
        [HideInInspector] public VehicleController controller; 

        private void InstantiateComponents() {
            rigidBody = gameObject.InstantiateComponent<Rigidbody>();
            boxCollider = gameObject.InstantiateComponent<BoxCollider>();
            controller = gameObject.InstantiateComponent<VehicleController>(); 
            vehiclePathProvider = gameObject.InstantiateComponent<VehiclePathProvider>();
            follower = gameObject.InstantiateComponent<VehiclePointsListFollower>();
        }

        void Reset() {
            Start();
        }

        private void OnValidate() {
            Start();
        }

        private void Start() {
            InstantiateComponents();
            SetNewPointsToFollow();
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
                if (generateNewPathOnLoopFinished) {
                    SetNewPointsToFollow();
                }

                follower.CurrentPoint = follower.PointsToFollow.FirstOrDefault();
                follower.CurrentDriveStatus = VehiclePointsListFollower.DriveStatus.Start;
            }
        }
    }
}