using System.Collections.Generic;
using JetBrains.Annotations;
using PathCreator.Aggregator;
using UnityEditor;
using UnityEngine;

namespace PathCreator.Intersection {
    public class PathIntersection : MonoBehaviour {
        // IIntersectionQueueHandler handler;
        // List<VehicleIntersectionVisa> VehicleQueue;
        [ItemCanBeNull] public List<PathNode> InputsA;
        [ItemCanBeNull] public List<PathNode> InputsB;
        [ItemCanBeNull] public List<PathNode> InputsC;
        [ItemCanBeNull] public List<PathNode> InputsD;
        [ItemCanBeNull] public List<PathNode> OutputsA;
        [ItemCanBeNull] public List<PathNode> OutputsB;
        [ItemCanBeNull] public List<PathNode> OutputsC;
        [ItemCanBeNull] public List<PathNode> OutputsD;
        
        [Range(0.3f,20f)]
        public float size = 5f;
        // public Vector3 intersectionSize = new Vector3(defaultSize, defaultSize, defaultSize);
        
        private void OnDrawGizmos() {
            var pos = transform.position;
            var sizeVector = new Vector3(size, size / 2, size);
            Gizmos.DrawWireCube( pos, sizeVector);
            // Gizmos.DrawWireSphere( pos, size*2); // TODO: These will come with the handler though
            // Gizmos.DrawWireSphere( pos, size);
        }

        public PathIntersection() {
        }

        public void RegenerateIntersection() {
            Debug.Log("Regenerating intersection");
        }
    }
}