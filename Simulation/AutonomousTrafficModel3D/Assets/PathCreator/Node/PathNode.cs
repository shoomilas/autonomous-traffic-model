﻿using System;
using System.Collections.Generic;
using System.Linq;
using PathCreationEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace PathCreator.Aggregator {
    [Serializable]
    public enum Direction {
        Unknown,
        Left,
        Right,
        Forward
    }

    [Serializable]
    public class SplineOutData {
        public PathCreation.PathCreator spline;
        public Direction splineDirection;
        public PathNode dstNode;

        public SplineOutData(PathCreation.PathCreator spline, Direction splineDirection, PathNode dstNode) {
            this.spline = spline;
            this.splineDirection = splineDirection;
            this.dstNode = dstNode;
        }
    }
    
    public class PathNode : MonoBehaviour {
        private Vector3 anchorPoint;
        public List<SplineOutData> SplinesOut = new List<SplineOutData>();
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
            GameObject go = new GameObject($"Path Node");
            var foo = go.AddComponent<PathNode>();
            foo.previousPathNodes.Add(previousNode);
            return foo;
        }

        public static void DeletePathNode(PathNode node) {
            var firstPreviousNode = node.previousPathNodes.FirstOrDefault();
            RemoveSplinesLeadingToGivenPathNode(node);
            RemoveSplinesFollowingPathNode(node);
            if (firstPreviousNode != null) {
                PathNodeHelper.SelectObject(firstPreviousNode.gameObject);
            }
        }
        
        public static void RemoveSplinesLeadingToGivenPathNode(PathNode removedPathNode) {
            var g = removedPathNode;
            removedPathNode.previousPathNodes
                .ForEach(previousNode => {
                        RemoveDstPathNodeAndSplines(previousNode, removedPathNode);
                        Debug.Log($"RemoveDstPathNodeAndSplines({previousNode.name},{removedPathNode.name}) called" +
                                  $" [{removedPathNode == g}]");
                });
        }
        
        public static void RemoveDstPathNodeAndSplines(PathNode srcNode, PathNode dstNode) {
            srcNode.SplinesOut
                .Where(splineOutData => splineOutData.dstNode == dstNode)
                .ToList()
                .ForEach(splineOutData => srcNode.RemoveSplineOutDataFromPathNode(splineOutData));
        }

        public void RemoveSplineOutDataFromPathNode(SplineOutData splineOut) {
            Undo.RecordObject(this,"Remove a splineOutData path node entry");
            Undo.DestroyObjectImmediate(splineOut.spline.gameObject);
            this.SplinesOut.Remove(splineOut);
        }
        
        public static void RemoveSplinesFollowingPathNode(PathNode removedPathNode) {
            removedPathNode.nextPathNodes?.ForEach(next => {
                if (next != null) {
                    Undo.RecordObject(next,"Remove a previousPathNode entry");
                    next.previousPathNodes.Remove(removedPathNode);
                }
            });
            Undo.DestroyObjectImmediate(removedPathNode.gameObject);
        } 
        
        private static void RemovePathNodeFromPreviousAndFollowing(PathNode node) {
            node.previousPathNodes?.ForEach(previous => {
                var status = previous.nextPathNodes.Remove(node);
                if(status) { Debug.Log(("Removed from previous node"));}
            });
            node.nextPathNodes?.ForEach(next => {
                if (next != null) {
                    var status = next.previousPathNodes.Remove(node);
                    if(status) { Debug.Log(("Removed from next node"));}
                }
            });
        }

        public Vector3 AnchorPoint
        {
            get => this.transform.position;
            set => anchorPoint = value;
        }
    }
}