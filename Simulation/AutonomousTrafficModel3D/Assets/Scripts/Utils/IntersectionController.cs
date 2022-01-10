using System.Collections.Generic;
using PathCreator.Aggregator;
using UnityEngine;

public interface IIntersectionZone {
    float Size { get; }
    // void OnTriggerEnter(Collider other);
    // void OnTriggerExit(Collider other);
}

public class IntersectionController : MonoBehaviour {
    public List<(IVehicle, Direction)> IntersectionQueue;
    private List<IIntersectionZone> zones;


    // Start is called before the first frame update
    private void Start() { }

    // Update is called once per frame
    private void Update() { }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0, 0, 1, 0.4f);
        var position = transform.position;
        // Gizmos.DrawSphere(position, gizmoSize);
    }
}