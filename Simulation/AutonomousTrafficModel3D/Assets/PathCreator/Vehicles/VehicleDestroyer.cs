using UnityEngine;

namespace PathCreator.Vehicles {
    public class VehicleDestroyer : MonoBehaviour {
        [Range(.1f, 10)] public float size = 2.5f;

        private void Start() {
            var sphereCollider = gameObject.InstantiateComponent<SphereCollider>();
            sphereCollider.radius = size;
            sphereCollider.isTrigger = true;
        }

        private void OnDrawGizmos() {
            Gizmos.color = new Color(1, 0, 0, 0.4f);
            var position = transform.position;
            Gizmos.DrawSphere(position, size);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Vehicle>() == null) return;
            Destroy(other.gameObject);
        }
    }
}