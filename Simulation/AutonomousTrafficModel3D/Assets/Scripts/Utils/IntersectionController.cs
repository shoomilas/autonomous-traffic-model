using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using PathCreator.Aggregator;
using UnityEngine;
using Color = UnityEngine.Color;

public interface IIntersectionZone {
    float Size { get; }
    // void OnTriggerEnter(Collider other);
    // void OnTriggerExit(Collider other);
}

public class IntersectionController : MonoBehaviour {
    private List<IIntersectionZone> zones;
    public List<(IVehicle, Direction)> IntersectionQueue;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = new Color(0,0,1,0.4f);
        var position = transform.position;
        // Gizmos.DrawSphere(position, gizmoSize);
    }
}
