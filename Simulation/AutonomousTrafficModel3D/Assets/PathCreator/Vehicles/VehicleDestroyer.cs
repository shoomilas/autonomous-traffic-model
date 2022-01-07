using UnityEngine;

namespace PathCreator.Vehicles {
    public class VehicleDestroyer : MonoBehaviour {
        [Range(.1f, 10)] public float size = 2.5f;

        private void OnDrawGizmos() {
            // Gizmos.color = Color.red;
            Gizmos.color = new Color(1, 0, 0, 0.4f);
            var position = transform.position;
            // Gizmos.DrawCube(position, Vector3.one*size);
            Gizmos.DrawSphere(position, size);
        }

        private void Start() {
            // var boxCollider = gameObject.InstantiateComponent<BoxCollider>();
            // boxCollider.size = Vector3.one * size;
            // boxCollider.isTrigger = true;
            var sphereCollider = gameObject.InstantiateComponent<SphereCollider>();
            sphereCollider.radius = size;
            sphereCollider.isTrigger = true;
        }
        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Vehicle>() == null) {
                return;
            }
            Destroy(other.gameObject);
        }
        // private void OnTriggerStay(Collider other) {
        //     Debug.Log("Object in trigger zone");
        // }
        // private void OnTriggerExit(Collider other) {
        //     Debug.Log("Object exit");
        // }
    }
}