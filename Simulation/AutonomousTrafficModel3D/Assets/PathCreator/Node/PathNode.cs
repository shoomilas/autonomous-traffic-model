using PathCreationEditor;
using UnityEngine;

namespace PathCreator.Aggregator {
    enum Direction {
        Left,
        Right,
        Forward
    }
    
    public class PathNode : MonoBehaviour {
        private (PathEditor, Direction?)[] splines;
        private Vector3 anchorPoint;
    }
}