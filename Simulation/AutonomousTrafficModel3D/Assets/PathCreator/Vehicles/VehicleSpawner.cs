using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using PathCreator.Aggregator;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace PathCreator.Vehicles {
    // TODO: Spawns cars on this PathNode
    // TODO: with [some/random] directions and rules to go (interface)
    // TODO: All Vehicles as children
    // TODO: 3 types of path chooser: "always-one-direction" "to a point" "always random"
    public interface IPathChooser {
        // -  Could return (intersection,direction) tuples list or complete list of path nodes
        // List<PathNode>  
        // - Later on could spawn on any point (Transform) rather than on a path node and  
    }

    public class VehicleSpawner : MonoBehaviour {
        public bool recurring = false;
        public int hardInstantiationLimit = 5;
        [Range(.1f, 10)] public float interval = 1;
        [Range(.1f, 10)] public float gizmoSize = .5f;

        public GameObject vehiclePrefab;
        public List<Vehicle> Vehicles = new List<Vehicle>(); // TODO: Extract to "VehicleManager"?
        void Start() {
            StartCoroutine(InstantiatorWithPathProviderMethod());
        }
        
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            var position = transform.position;
            Gizmos.DrawSphere(position, gizmoSize);
        }

        IEnumerator Instantiator() {
            var position = transform.position;
            var vehicle = Instantiate(vehiclePrefab, position + Vector3.one, Quaternion.identity);
            yield return new WaitForSeconds(interval);
            vehicle.AddComponent<Rigidbody>();
            yield return new WaitForSeconds(interval);
            if (recurring) {
                for (int i = 1; i < hardInstantiationLimit; i++) {
                    // position += Vector3.one * i;
                    var anotherVehicle = Instantiate(vehiclePrefab, position + Vector3.up, Quaternion.identity);
                    anotherVehicle.AddComponent<Rigidbody>();
                    yield return new WaitForSeconds(interval);
                }
            }
        }

        Quaternion PrepVehicleSpawnQuaternion() {
            var spawnNode = gameObject.InstantiateComponent<PathNode>();
            var spawnPoint = transform.position;
            var nextPoint = spawnNode.nextPathNodes.First().transform.position;
            Vector3 difference = nextPoint - spawnPoint;
            var quaternion =  Quaternion.LookRotation(difference, Vector3.up);
            return quaternion;
        }
        
        IEnumerator InstantiatorWithPathProviderMethod(bool shouldLoop = true) {
            var spawnHeight = 0.05f;
            var position = transform.position;
            var quaternion = PrepVehicleSpawnQuaternion();
            var vehicle = Instantiate(vehiclePrefab, position + Vector3.one * spawnHeight, quaternion);
            var vehicleComponent = vehicle.InstantiateComponent<Vehicle>();
            // vehicleComponent.vehiclePathProvider.CurrentMethod = providerMethod;
            vehicleComponent.follower.reachedTargetDistance = 2.5f;
            vehicleComponent.startNode = GetComponent<PathNode>();
            vehicleComponent.shouldLoop = shouldLoop;

            yield return new WaitForSeconds(interval);
            if (recurring) {
                for (int i = 1; i < hardInstantiationLimit; i++) {
                    var anotherVehicleGO = Instantiate(vehiclePrefab, position + Vector3.one * spawnHeight, quaternion);
                    var anotherVehicle = anotherVehicleGO.InstantiateComponent<Vehicle>();
                    // anotherVehicle.vehiclePathProvider.CurrentMethod = providerMethod;
                    anotherVehicle.startNode = GetComponent<PathNode>();
                    anotherVehicle.shouldLoop = shouldLoop;
                    yield return new WaitForSeconds(interval);
                }
            }
        }
    }
}