using UnityEngine;

namespace PathCreator.Vehicles {
    public class VehicleDestroyer : MonoBehaviour {
        // TODO: sphere/cube of given size
        // TODO: Destroys spawned vehicles upon reaching 
        [Range(.1f, 10)] public float size = .5f;

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            var position = transform.position;
            // Gizmos.DrawSphere(position, size);
            Gizmos.DrawCube(position, Vector3.one*size);
        }

        private void Start() {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = Vector3.one * size;
            boxCollider.isTrigger = true;
        }
        private void OnTriggerEnter(Collider other) {
            Debug.Log("Entered");
            // var boo = GetComponent<FoonTriggers>() as PathFollower;
            // var boo = GetComponent(other.name) as PathFollower;
            // var obj = new GameObject(other.name, typeof(PathFollower));
            // var rb = obj.GetComponent<PathFollower>();
        }
        private void OnTriggerStay(Collider other) {
            Debug.Log("STayyyys");
        }
        private void OnTriggerExit(Collider other) {
            Debug.Log("Object exit");
        }
    }
}