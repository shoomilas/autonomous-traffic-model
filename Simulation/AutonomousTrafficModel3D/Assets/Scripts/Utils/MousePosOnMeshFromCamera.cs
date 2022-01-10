using UnityEngine;

public class MousePosOnMeshFromCamera : MonoBehaviour {
    [SerializeField] private Transform targetTransform;

    [SerializeField] private MeshCollider meshCollider;

    private Ray ray;
    private Vector3 worldPosition;

    private void Start() {
        meshCollider = meshCollider.GetComponent<MeshCollider>();
    }

    private void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (meshCollider.Raycast(ray, out hitData, 1000)) {
            worldPosition = hitData.point;
            if (targetTransform != null) targetTransform.position = worldPosition;
        }
    }
}