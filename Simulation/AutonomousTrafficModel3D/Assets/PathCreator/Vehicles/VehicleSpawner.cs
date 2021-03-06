using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using UnityEngine;

namespace PathCreator.Vehicles {
    public class VehicleSpawner : MonoBehaviour {
        public bool recurring;
        public int hardInstantiationLimit = 5;

        [InspectorName("Offset (Initial Delay)")] [Range(0f, 10)]
        public float offset;

        [Range(.1f, 10)] public float interval = 1;
        [Range(.01f, 10f)] public float reachedTargetDistance = 3.5f;
        [Range(.1f, 3000f)] public float stoppingSpeed = 40f;
        [Range(.1f, 3000f)] public float onReachedTargetBreakEngageSpeed = 15f;

        [Range(.1f, 10)] public float gizmoSize = 2.5f;
        [Range(.1f, 10000)] public float speedMax = 8f;
        [Range(.1f, 10000)] public float speedMin;
        [Range(.1f, 10000)] public float acceleration = 30f;
        [Range(.1f, 10000)] public float brakeSpeed = 100f;
        [Range(.1f, 10000)] public float idleSlowdown = 10f;
        [Range(.1f, 10000)] public float turnSpeedMax = 900f;
        [Range(.1f, 10000)] public float turnSpeedAcceleration = 900f;
        [Range(.1f, 10000)] public float turnIdleSlowdown = 500f;


        public GameObject vehiclePrefab;
        public List<Vehicle> Vehicles = new List<Vehicle>(); // TODO: Extract to "VehicleManager"?
        public PathProviderMethod providerMethod = PathProviderMethod.AlwaysRandomRightForward;


        private void Start() {
            StartCoroutine(InstantiatorWithPathProviderMethod());
        }

        private void OnDrawGizmos() {
            Gizmos.color = new Color(0, 0, 1, 0.4f);
            var position = transform.position;
            Gizmos.DrawSphere(position, gizmoSize);
        }

        private IEnumerator Instantiator() {
            var position = transform.position;
            var vehicle = Instantiate(vehiclePrefab, position + Vector3.one, Quaternion.identity);
            yield return new WaitForSeconds(interval);
            vehicle.AddComponent<Rigidbody>();
            yield return new WaitForSeconds(interval);
            if (recurring)
                for (var i = 1; i < hardInstantiationLimit; i++) {
                    var anotherVehicle = Instantiate(vehiclePrefab, position + Vector3.up, Quaternion.identity);
                    anotherVehicle.AddComponent<Rigidbody>();
                    yield return new WaitForSeconds(interval);
                }
        }

        private Quaternion PrepVehicleSpawnQuaternion() {
            var spawnNode = gameObject.InstantiateComponent<PathNode>();
            var spawnPoint = transform.position;
            var nextPoint = spawnNode.nextPathNodes.First().transform.position;
            var difference = nextPoint - spawnPoint;
            var quaternion = Quaternion.LookRotation(difference, Vector3.up);
            return quaternion;
        }

        private IEnumerator InstantiatorWithPathProviderMethod(bool shouldLoop = true) {
            var spawnHeight = 0.05f;
            var position = transform.position;
            var quaternion = PrepVehicleSpawnQuaternion();

            yield return new WaitForSeconds(0.33f); // Without this delay, the first car is always a bit behind

            yield return new WaitForSeconds(offset);
            for (var i = 0; i < hardInstantiationLimit; i++) {
                var vehicle = Instantiate(vehiclePrefab, position + Vector3.one * spawnHeight, quaternion);
                var vehicleComponent = vehicle.InstantiateComponent<Vehicle>();

                vehicleComponent.ProviderMethod = providerMethod;
                vehicleComponent.startNode = gameObject.GetComponent<PathNode>();
                vehicleComponent.follower.reachedTargetDistance = 2.5f;
                vehicleComponent.shouldLoop = shouldLoop;
                vehicleComponent.controller.speedMax = speedMax;
                vehicleComponent.controller.speedMax = speedMax;
                vehicleComponent.controller.speedMin = speedMin;
                vehicleComponent.controller.acceleration = acceleration;
                vehicleComponent.controller.brakeSpeed = brakeSpeed;
                vehicleComponent.controller.idleSlowdown = idleSlowdown;
                vehicleComponent.controller.turnSpeedMax = turnSpeedMax;
                vehicleComponent.controller.turnSpeedAcceleration = turnSpeedAcceleration;
                vehicleComponent.controller.turnIdleSlowdown = turnIdleSlowdown;
                vehicleComponent.follower.reachedTargetDistance = reachedTargetDistance;
                vehicleComponent.follower.stoppingSpeed = stoppingSpeed;
                vehicleComponent.follower.onReachedTargetBreakEngageSpeed = onReachedTargetBreakEngageSpeed;

                if (!recurring) yield break;

                yield return new WaitForSeconds(interval);
            }
        }
    }
}