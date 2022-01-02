using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PathCreation;
using PathCreationEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngineInternal;
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
    
    [ExecuteInEditMode]
    public class PathNode : MonoBehaviour {
        
        public bool splinesAutoUpdate = false;
        private const double waitTime = 0.5;

#if UNITY_EDITOR
        private void Update() {
            if(splinesAutoUpdate) {
                if(transform.hasChanged && (transform.position != anchorPoint)) {
                    isUpdateable = true;
                    OnPathNodeTransformedHandler();
                }
            }
        }
#endif
        
        public event System.Action PathNodeTransformedEvent;
        public void OnPathNodeTransformedHandler() {
            PathNodeTransformedEvent?.Invoke();
        }

        private bool isUpdateable;
        
        public async void OnPathNodeTransformed() {
            if(isUpdateable) {
                isUpdateable = false;
                UpdatePathNodePosition();
                // anchorPoint = transform.position;
                // Debug.Log($"{transform.name}");
                await WaitSomeAsync();
                isUpdateable = true;
            }
        }

        public void UpdateClosestSplineConnections() {
            var pathNodes = FindObjectsOfType<PathNode>();
            var splines = FindObjectsOfType<PathCreation.PathCreator>();
            Undo.RecordObjects(pathNodes, "Update Positions");
            if(splines != null ) {Undo.RecordObjects(splines, "Update Positions");}
            UpdatePathNodePosition();
            foreach (var previousNode in previousPathNodes) {
                previousNode.UpdatePathNodePosition();
            }
        }
        
        public static void UpdateAllNodesSplineConnections() {
            var pathNodes = FindObjectsOfType<PathNode>();
            foreach (var pathNode in pathNodes) {
                pathNode.UpdatePathNodePosition();
            }
        }
        
        public void UpdatePathNodePosition() {
            anchorPoint = transform.position;
            
            // SPLINES IN: Anchor
            previousPathNodes.ForEach(previousPathNode => {
                previousPathNode.SplinesOut.Where(data => data.dstNode==this).ToList().ForEach(_ => {
                    var numPoints = _.spline.bezierPath.NumPoints; 
                    _.spline.bezierPath.SetPoint(numPoints-1, anchorPoint);
                });
            });
            
            // SPLINES OUT: Add and remove:
            SplinesOut.ForEach(data => {
                var oldSpline = data.spline;
                var dstNode = data.dstNode;
                var newSpline = AddSplineBetweenPathNodes(this, dstNode);
                data.spline = newSpline;
                DestroyImmediate(oldSpline.gameObject);
            });
        }
        public PathCreation.PathCreator AddSplineBetweenPathNodes(PathNode node1, PathNode node2) {
            var pos1 = node1.transform.position;
            var pos2 = node2.transform.position;
            var pos3 = (pos1 + pos2) / 2;
            var listOfPositions = new List<Vector3> { node1.transform.position, pos3, node2.transform.position};
        
            // Create a new bezier path from the waypoints.
            var bezier = new BezierPath(listOfPositions);
        
            // Create new GameObject
            GameObject go = new GameObject("Spline");
            var foo = go.AddComponent<PathCreation.PathCreator>();
            foo.bezierPath = bezier;
            var name1 = node1.transform.name;
            var name2 = node2.transform.name;
            foo.transform.name = $"Spline: {name1}-{name2}";
        
            foo.transform.parent = node1.transform; // as child
            // foo.transform.parent = node1.transform.parent; // as sibling
            // THIS ONE DOES NOT ADD TO SPLINES OUT DATA.
            return foo;
        }
        
        private async Task WaitSomeAsync() {
            await Task.Delay(TimeSpan.FromSeconds(waitTime));
        }
        
        private void OnEnable() {
            isUpdateable = false;
            PathNodeManager.AllThePathNodes.Add(this);
            PathNodeTransformedEvent += OnPathNodeTransformed;
        }

        private void OnDisable() {
            PathNodeManager.AllThePathNodes.Remove(this);
            PathNodeTransformedEvent -= OnPathNodeTransformed;
        }
        
        private Vector3 anchorPoint;
        public List<SplineOutData> SplinesOut = new List<SplineOutData>();
        public List<PathNode> nextPathNodes = new List<PathNode>();
        public List<PathNode> previousPathNodes = new List<PathNode>();

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
        
        private static void RemovePathNodeFromPreviousAndFollowing(PathNode node) { // Obsolete
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
            get
            {
                anchorPoint = transform.position;
                return anchorPoint;
            }
            set => anchorPoint = value;
        }
    }
}