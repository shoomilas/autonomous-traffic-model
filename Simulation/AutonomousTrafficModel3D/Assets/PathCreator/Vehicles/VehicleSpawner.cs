using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public GameObject vehiclePrefab; // public Vehicle vehiclePrefab;
        public List<Vehicle> Vehicles = new List<Vehicle>(); // TODO: Extract to "VehicleManager"?
        void Start() {
            StartCoroutine(InstantiatorWithDirection());
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
        
        IEnumerator InstantiatorWithDirection() {
            var spawnHeight = 10f;
            var position = transform.position;
            var vehicle = Instantiate(vehiclePrefab, position + Vector3.one * spawnHeight, Quaternion.identity);
            // var vehicleComponent = vehicle.AddComponent<Vehicle>();
            var vehicleComponent = vehicle.InstantiateComponent<Vehicle>();
            vehicleComponent.startNode = GetComponent<PathNode>();
            vehicleComponent.shouldLoop = true;

            yield return new WaitForSeconds(interval);
            if (recurring) {
                for (int i = 1; i < hardInstantiationLimit; i++) {
                    var anotherVehicle = Instantiate(vehiclePrefab, position + Vector3.up * spawnHeight, Quaternion.identity);
                    anotherVehicle.AddComponent<Rigidbody>();
                    vehicle.AddComponent<Vehicle>();
                    yield return new WaitForSeconds(interval);
                }
            }
        }
    }
}

// public PathCreator pathPrefab;
// public PathFollower followerPrefab;
// public Transform[] spawnPoints;
//
// void Start () {
//     foreach (Transform t in spawnPoints) {
//         var path = Instantiate (pathPrefab, t.position, t.rotation);
//         var follower = Instantiate (followerPrefab);
//         follower.pathCreator = path;
//         path.transform.parent = t;
//     }
// }