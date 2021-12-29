using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PathCreator.Aggregator {
    [Serializable]
    public class PathNodeManager : MonoBehaviour {
        [SerializeField]
        public static List<PathNode> AllThePathNodes = new List<PathNode>();
        
        // // Draw gizmos works, handles doesn't
        // private void OnDrawGizmos() {
        //     foreach (var pathNode in AllThePathNodes) {
        //         
        //         var color = Color.red;
        //         var radius = 1;
        //         Gizmos.DrawWireSphere(pathNode.transform.position, radius);
        //         
        //         var size = 1f;
        //         var pathNodeTransform = pathNode.transform;
        //         Handles.color = Color.red;
        //         Handles.ConeHandleCap(
        //             0,
        //             pathNodeTransform.position + new Vector3(0f, size / 2, 0f),
        //             pathNodeTransform.rotation * Quaternion.LookRotation(Vector3.up),
        //             size,
        //             EventType.Repaint
        //         );
        //     }
        // }
    }
}