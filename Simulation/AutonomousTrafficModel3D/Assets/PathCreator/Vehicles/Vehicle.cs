using System;
using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using UnityEngine;

namespace PathCreator.Vehicles {
    public static class Extensions {
        public static T InstantiateComponent<T>(this GameObject gameObject) where T : Component {
            var component = gameObject.GetComponent<T>();
            if (component == null) component = gameObject.AddComponent<T>();

            return component;
        }
    }

    public class Vehicle : MonoBehaviour {
        public bool shouldLoop;
        public bool generateNewPathOnLoopFinished;
        public PathNode startNode;
        [HideInInspector] public VehiclePointsFollower follower;
        [HideInInspector] public Rigidbody rigidBody;
        [HideInInspector] public BoxCollider boxCollider;
        [HideInInspector] public VehicleController controller;
        private PathProviderMethod providerMethod = PathProviderMethod.AlwaysRandomRightForward;

        [HideInInspector] public IVehiclePathProvider vehiclePathProvider; // TODO: Change to interface

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

        private void Reset() {
            Start();
        }

        private void Start() {
            InstantiateComponents();
            SetNewPointsToFollow();
        }

        private void Update() {
            OnVehiclePathFinished(OnPathFinishedNewLoopHandler);
        }

        private void OnValidate() {
            Start();
        }

        private void InstantiateComponents() {
            rigidBody = gameObject.InstantiateComponent<Rigidbody>();
            boxCollider = gameObject.InstantiateComponent<BoxCollider>();
            controller = gameObject.InstantiateComponent<VehicleController>();
            vehiclePathProvider = gameObject.InstantiateComponent<VehiclePathProvider>();
            follower = gameObject.InstantiateComponent<VehiclePointsFollower>();
        }

        private void SetNewPointsToFollow() {
            follower.PointsToFollow = new List<Transform>();
            var list = vehiclePathProvider.Provide(startNode).Select(_ => _.transform).ToList();
            follower.PointsToFollow = list;
        }

        public void OnVehiclePathFinished(Action toDo) {
            if (follower.CurrentDriveStatus == VehiclePointsFollower.DriveStatus.Finished) toDo();
        }

        public void OnPathFinishedNewLoopHandler() {
            if (shouldLoop) {
                if (generateNewPathOnLoopFinished) SetNewPointsToFollow();

                follower.CurrentPoint = follower.PointsToFollow.FirstOrDefault();
                follower.CurrentDriveStatus = VehiclePointsFollower.DriveStatus.Start;
            }
        }
    }
}