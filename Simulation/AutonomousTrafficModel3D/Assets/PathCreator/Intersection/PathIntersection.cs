using System.Collections.Generic;
using JetBrains.Annotations;
using PathCreator.Aggregator;
using UnityEngine;

namespace PathCreator.Intersection {
    public class PathIntersection : MonoBehaviour {
        // IIntersectionQueueHandler handler;
        // List<Vehicle> VehicleQueue;
        
        [ItemCanBeNull] private List<PathNode> InputsA;
        [ItemCanBeNull] private List<PathNode> InputsB;
        [ItemCanBeNull] private List<PathNode> InputsC;
        [ItemCanBeNull] private List<PathNode> InputsD;
        [ItemCanBeNull] private List<PathNode> OutputsA;
        [ItemCanBeNull] private List<PathNode> OutputsB;
        [ItemCanBeNull] private List<PathNode> OutputsC;
        [ItemCanBeNull] private List<PathNode> OutputsD;
        private Vector3 intersectionPosition;
    }
}