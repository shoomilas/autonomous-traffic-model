using System;
using System.Collections.Generic;
using System.Linq;
using PathCreationEditor;
using UnityEngine;

namespace PathCreator.Aggregator {
    public enum Direction {
        Left,
        Right,
        Forward,
        Unknown
    }
    
    public class PathNode : MonoBehaviour {
        // List of splines going OUT of the node.
        public List<(PathCreation.PathCreator, Direction?, PathNode)> Splines = new List<(PathCreation.PathCreator, Direction?, PathNode)> {};
        private Vector3 anchorPoint;
        public List<PathNode> nextPathNodes = new List<PathNode>();
        public List<PathNode> previousPathNodes = new List<PathNode>();

        public static PathNode MakePathNode(PathNode previousNode) {
            GameObject go = new GameObject("Path Node");
            var foo = go.AddComponent<PathNode>();
            foo.previousPathNodes.Add(previousNode);
            return foo;
        }
        
        public static void DeletePathNode(PathNode node) {
            var firstPreviousNode = node.previousPathNodes.First();
            node.previousPathNodes.ForEach(previous => previous.nextPathNodes.Remove(node));
            node.nextPathNodes.ForEach(next => next.previousPathNodes.Remove(node));
            DestroyImmediate(node.gameObject);
            if (firstPreviousNode != null) {
                PathNodeHelper.SelectObject(firstPreviousNode.gameObject);
            }
        }

        private void OnDrawGizmos() {
            float size = 1f;
            Gizmos.DrawCube(transform.position, Vector3.one/10f);
            Gizmos.DrawWireSphere(transform.position, size/5);
            Gizmos.DrawWireSphere(transform.position, size/5);
        }
        
        public Vector3 AnchorPoint
        {
            get => this.transform.position;
            set => anchorPoint = value;
        }
    }
}