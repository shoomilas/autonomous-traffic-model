using System.Collections.Generic;
using System.Linq;
using PathCreationEditor;
using UnityEngine;

namespace PathCreator.Aggregator {
    public enum Direction {
        Left,
        Right,
        Forward
    }
    
    public class PathNode : MonoBehaviour {
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
                // PathNodeEditor.SelectObject(firstPreviousNode.gameObject);
            }
        }

        public Vector3 AnchorPoint
        {
            get => this.transform.position;
            set => anchorPoint = value;
        }
    }
}