using UnityEngine;

namespace PathCreator.Vehicles {
    public class VehicleDestroyer : MonoBehaviour {
        [Range(.1f, 10)] public float size = .5f;

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            var position = transform.position;
            Gizmos.DrawCube(position, Vector3.one*size);
        }

        private void Start() {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = Vector3.one * size;
            boxCollider.isTrigger = true;
        }
        private void OnTriggerEnter(Collider other) {
            Destroy(other.gameObject);
            // Debug.Log("Entered");
            // var boo = GetComponent(other.name) as PathFollower;
            // var obj = new GameObject(other.name, typeof(PathFollower));
            // var rb = obj.GetComponent<PathFollower>();
        }
        // private void OnTriggerStay(Collider other) {
        //     Debug.Log("Object in trigger zone");
        // }
        // private void OnTriggerExit(Collider other) {
        //     Debug.Log("Object exit");
        // }
    }
}