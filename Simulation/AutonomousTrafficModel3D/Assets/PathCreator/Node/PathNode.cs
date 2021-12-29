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
        public List<(PathCreation.PathCreator spline, Direction? splineDirection, PathNode dstNode)> SplinesOut = new List<(PathCreation.PathCreator, Direction?, PathNode)> {};
        
        private Vector3 anchorPoint;
        public List<PathNode> nextPathNodes = new List<PathNode>();
        public List<PathNode> previousPathNodes = new List<PathNode>();
        
        private void OnEnable() {
            PathNodeManager.AllThePathNodes.Add(this);
        }

        private void OnDisable() {
            PathNodeManager.AllThePathNodes.Remove(this);
        }
        
        private void OnDrawGizmos() {
            float size = 1f;
            Gizmos.DrawCube(transform.position, Vector3.one/10f);
            Gizmos.DrawWireSphere(transform.position, size/5);
        }

        public static PathNode MakePathNode(PathNode previousNode) {
            GameObject go = new GameObject("Path Node");
            var foo = go.AddComponent<PathNode>();
            foo.previousPathNodes.Add(previousNode);
            return foo;
        }

        public static void RemoveSplinesLeadingToGivenPathNode1Try(PathNode node) {
            // find all PathNode gameobjects
            var allPathNodes = FindObjectsOfType<PathNode>();
            
            foreach (var pathNode in allPathNodes) {
                var nodesLeadingToRemovedNode = 
                    pathNode.SplinesOut.Where(
                        currentNode => currentNode.dstNode == node)
                        .ToList();
                
                if (nodesLeadingToRemovedNode.Count != 0) {
                    nodesLeadingToRemovedNode.ForEach(n => {
                        
                        DestroyImmediate(n.spline.gameObject);
                    });
                }
            }
            
            // allPathNodes
            //     .Where(pathNode => pathNode.SplinesOut.)
            //     .ToList()
            //     .ForEach(n => {
            //         SplinesOut.Remove(n);
            //         DestroyImmediate(n.spline.gameObject);
            //     });
        }

        public static void RemoveSplinesLeadingToGivenPathNode(PathNode removedPathNode) {
            removedPathNode.previousPathNodes.ForEach(previousPathNode => {
                previousPathNode.SplinesOut.Where(tuple => tuple.dstNode == removedPathNode).ToList().ForEach( _ =>
                     DestroyImmediate(_.spline.gameObject));
                });
        }

        public static void DeletePathNode(PathNode node) {
            // 1. Remove splines leading to this node
            RemoveSplinesLeadingToGivenPathNode(node);
            
            // 2. Remove from previous and following path nodes
            var firstPreviousNode = node.previousPathNodes.First();
            node.previousPathNodes.ForEach(previous => {
                
                previous.nextPathNodes.Remove(node);
            });
            node.nextPathNodes.ForEach(next => next.previousPathNodes.Remove(node));
            DestroyImmediate(node.gameObject);
            if (firstPreviousNode != null) {
                PathNodeHelper.SelectObject(firstPreviousNode.gameObject);
            }
        }

        public Vector3 AnchorPoint
        {
            get => this.transform.position;
            set => anchorPoint = value;
        }
    }
}