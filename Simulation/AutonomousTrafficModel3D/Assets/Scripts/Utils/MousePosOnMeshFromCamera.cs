using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosOnMeshFromCamera : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField]
    private MeshCollider meshCollider;
    Vector3 worldPosition;
    Ray ray;

    void Start() {
        meshCollider = meshCollider.GetComponent<MeshCollider>();
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if(meshCollider.Raycast(ray, out hitData, 1000))
        {
            worldPosition = hitData.point;
            if (targetTransform != null) {
                targetTransform.position = worldPosition; 
            }
        }
    }
}
