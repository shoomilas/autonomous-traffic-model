using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PathCreation;
using UnityEditor;
using UnityEngine;

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
        private const double waitTime = 0.5;

        public bool splinesAutoUpdate;
        public List<SplineOutData> SplinesOut = new List<SplineOutData>();
        public List<PathNode> nextPathNodes = new List<PathNode>();
        public List<PathNode> previousPathNodes = new List<PathNode>();

        private Vector3 anchorPoint;

        private bool isUpdateable;


        public Vector3 AnchorPoint
        {
            get
            {
                anchorPoint = transform.position;
                return anchorPoint;
            }
            set => anchorPoint = value;
        }

#if UNITY_EDITOR
        private void Update() {
            if (splinesAutoUpdate)
                if (transform.hasChanged && transform.position != anchorPoint) {
                    isUpdateable = true;
                    OnPathNodeTransformedHandler();
                }
        }
#endif

        private void OnEnable() {
            isUpdateable = false;
            PathNodeManager.AllThePathNodes.Add(this);
            PathNodeTransformedEvent += OnPathNodeTransformed;
        }

        private void OnDisable() {
            PathNodeManager.AllThePathNodes.Remove(this);
            PathNodeTransformedEvent -= OnPathNodeTransformed;
        }

        private void OnDrawGizmos() {
            var size = 1f;
            Gizmos.DrawCube(transform.position, Vector3.one / 10f);
            Gizmos.DrawWireSphere(transform.position, size / 5);
        }

        public event Action PathNodeTransformedEvent;

        public void OnPathNodeTransformedHandler() {
            PathNodeTransformedEvent?.Invoke();
        }

        public async void OnPathNodeTransformed() {
            if (isUpdateable) {
                isUpdateable = false;
                UpdatePathNodePosition();
                await WaitSomeAsync();
                isUpdateable = true;
            }
        }

        public void UpdateClosestSplineConnections() {
            var pathNodes = FindObjectsOfType<PathNode>();
            var splines = FindObjectsOfType<PathCreation.PathCreator>();
            Undo.RecordObjects(pathNodes, "Update Positions");
            if (splines != null) Undo.RecordObjects(splines, "Update Positions");
            UpdatePathNodePosition();
            foreach (var previousNode in previousPathNodes) previousNode.UpdatePathNodePosition();
        }

        public static void UpdateAllNodesSplineConnections() {
            var pathNodes = FindObjectsOfType<PathNode>();
            foreach (var pathNode in pathNodes) pathNode.UpdatePathNodePosition();
        }

        public void UpdatePathNodePosition() {
            anchorPoint = transform.position;

            // SPLINES IN: Anchor
            previousPathNodes.ForEach(previousPathNode => {
                previousPathNode.SplinesOut.Where(data => data.dstNode == this).ToList().ForEach(_ => {
                    var numPoints = _.spline.bezierPath.NumPoints;
                    _.spline.bezierPath.SetPoint(numPoints - 1, anchorPoint);
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

        public PathCreation.PathCreator ConnectNodes(PathNode dstNode, bool shouldAddToSplinesOut = false) {
            var isRequestNotValid =
                nextPathNodes.Contains(dstNode)
                || previousPathNodes.Contains(dstNode)
                || this == null
                || dstNode == null;

            var reason = "";
            if (nextPathNodes.Contains(dstNode)) reason += "srcNode.nextPathNodes already contains dstNode; ";

            if (previousPathNodes.Contains(dstNode)) reason += "srcNode.previousPathNodes already contains dstNode; ";

            var srcName = string.Empty;
            var dstName = string.Empty;
            if (this == null) {
                srcName = "-";
                reason += "srcNode is null; ";
            }
            else {
                srcName = name;
            }

            if (dstNode == null) {
                dstName = "-";
                reason += "dstNode is null; ";
            }
            else {
                dstName = dstNode.name;
            }

            if (isRequestNotValid) {
                Debug.Log($"Can't connect {srcName} with {dstName}. Reason: [{reason}]");
                return null;
            }

            var createdSpline = AddSplineBetweenPathNodes(this, dstNode);
            nextPathNodes.Add(dstNode);
            dstNode.previousPathNodes.Add(this);

            if (shouldAddToSplinesOut) {
                var splinesOutData = new SplineOutData(createdSpline, Direction.Unknown, dstNode);
                SplinesOut.Add(splinesOutData);
            }

            return createdSpline;
        }


        public PathCreation.PathCreator AddSplineBetweenPathNodes(PathNode node1, PathNode node2) {
            var pos1 = node1.transform.position;
            var pos2 = node2.transform.position;
            var pos3 = (pos1 + pos2) / 2;
            var listOfPositions = new List<Vector3> { node1.transform.position, pos3, node2.transform.position };

            // Create a new bezier path from the waypoints.
            var bezier = new BezierPath(listOfPositions);

            // Create new GameObject
            var go = new GameObject("Spline");
            Undo.RegisterCreatedObjectUndo(go, "Create spline");
            var foo = go.AddComponent<PathCreation.PathCreator>();
            foo.bezierPath = bezier;
            var name1 = node1.transform.name;
            var name2 = node2.transform.name;
            foo.transform.name = $"Spline: {name1}-{name2}";

            foo.transform.parent = node1.transform;
            return foo;
        }

        private async Task WaitSomeAsync() {
            await Task.Delay(TimeSpan.FromSeconds(waitTime));
        }

        public static PathNode MakePathNodeAsPrevious(PathNode nextNode) {
            var go = new GameObject("Path Node");
            Undo.RegisterCreatedObjectUndo(go, "Make Path Node");
            var foo = go.AddComponent<PathNode>();
            nextNode.previousPathNodes.Add(foo);
            return foo;
        }

        public static PathNode MakePathNode(PathNode previousNode) {
            var go = new GameObject("Path Node");
            Undo.RegisterCreatedObjectUndo(go, "Make Path Node");
            var foo = go.AddComponent<PathNode>();
            foo.previousPathNodes.Add(previousNode);
            return foo;
        }


        public void RemoveConnectionBetweenTwoNodesCaller(PathNode node2) {
            RemoveConnectionBetweenTwoNodes(node2);
            node2.RemoveConnectionBetweenTwoNodes(this);
        }

        public void RemoveConnectionBetweenTwoNodes(PathNode node2) {
            var c = SplinesOut.FirstOrDefault(data => data.dstNode == node2);
            if (c != null) {
                SplinesOut.Remove(c);
                Undo.DestroyObjectImmediate(c.spline);
            }

            var e = SplinesOut.FirstOrDefault(data => data.dstNode == this);
            if (e != null) {
                SplinesOut.Remove(e);
                Undo.DestroyObjectImmediate(e.spline);
            }

            var b = nextPathNodes.Remove(node2);
            var d = nextPathNodes.Remove(this);
            node2.previousPathNodes.Remove(this);
            previousPathNodes.Remove(node2);
        }

        public void RemoveAllConnectionsForPathNode() {
            nextPathNodes.Concat(previousPathNodes).ToList().ForEach(RemoveConnectionBetweenTwoNodesCaller);
        }

        public static void DeletePathNode(PathNode node) {
            var firstPreviousNode = node.previousPathNodes.FirstOrDefault();
            RemoveSplinesLeadingToGivenPathNode(node);
            RemoveSplinesFollowingPathNode(node);
            Undo.DestroyObjectImmediate(node.gameObject);
            if (firstPreviousNode != null) PathNodeHelper.SelectObject(firstPreviousNode.gameObject);
        }

        public static void RemoveSplinesLeadingToGivenPathNode(PathNode removedPathNode) {
            var g = removedPathNode;
            removedPathNode.previousPathNodes
                .ForEach(previousNode => { RemoveDstPathNodeAndSplines(previousNode, removedPathNode); });
        }

        public static void RemoveDstPathNodeAndSplines(PathNode srcNode, PathNode dstNode) {
            srcNode.SplinesOut
                .Where(splineOutData => splineOutData.dstNode == dstNode)
                .ToList()
                .ForEach(splineOutData => srcNode.RemoveSplineOutDataFromPathNode(splineOutData));
        }

        public void RemoveSplineOutDataFromPathNode(SplineOutData splineOut) {
            Undo.RecordObject(this, "Remove a splineOutData path node entry");
            Undo.DestroyObjectImmediate(splineOut.spline.gameObject);

            SplinesOut.Remove(splineOut);
        }

        public static void RemoveSplinesFollowingPathNode(PathNode removedPathNode) {
            removedPathNode.nextPathNodes?.ForEach(next => {
                if (next != null) {
                    Undo.RecordObject(next, "Remove a previousPathNode entry");
                    next.previousPathNodes.Remove(removedPathNode);
                }
            });
        }

        private static void RemovePathNodeFromPreviousAndFollowing(PathNode node) {
            // Obsolete
            node.previousPathNodes?.ForEach(previous => {
                var status = previous.nextPathNodes.Remove(node);
                if (status) Debug.Log("Removed from previous node");
            });
            node.nextPathNodes?.ForEach(next => {
                if (next != null) {
                    var status = next.previousPathNodes.Remove(node);
                    if (status) Debug.Log("Removed from next node");
                }
            });
        }

        public PathNode CreateNewNodeAtPosition(Vector3 newPosition, bool createNewAsDst = false) {
            var tr = this.transform;
            PathNode newNode;
            PathCreation.PathCreator newSpline;

            newNode = !createNewAsDst ? MakePathNode(this) : MakePathNodeAsPrevious(this);

            var transform = newNode.transform;
            transform.parent = tr.transform.parent;
            transform.position = newPosition;
            transform.name = $"Path Node {PathNodeManager.NodeCounter}";
            PathNodeManager.NodeCounter += 1;

            if (!createNewAsDst) {
                nextPathNodes.Add(newNode);
                Undo.RecordObject(this.transform.parent, "Add new Path Node");
                newSpline = AddSplineBetweenPathNodes(this, newNode);
                var splineData = new SplineOutData(newSpline, Direction.Unknown, newNode);
                SplinesOut.Add(splineData);
            }
            else {
                newNode.nextPathNodes.Add(this);
                Undo.RecordObject(this.transform.parent, "Add new Path Node");
                newSpline = AddSplineBetweenPathNodes(newNode, this);
                var splineData = new SplineOutData(newSpline, Direction.Unknown, this); // TODO: Specified direction
                newNode.SplinesOut.Add(splineData);
            }

            PathNodeHelper.SelectObject(newNode.gameObject);
            return newNode;
        }

        public PathNode CreateNewNodeWithForwardRightOffset(float forwardOffset, float rightOffset,
            bool createNewAsDst = false) {
            var newPosition = transform.position + Vector3.forward * forwardOffset + Vector3.right * rightOffset;
            var newNode = CreateNewNodeAtPosition(newPosition, createNewAsDst);
            return newNode;
        }

        public PathNode CreateNewNodeInSpecifiedDirectionWithDefinedOffset(float offset, Vector3 direction,
            bool createNewAsDst = false) {
            var newPosition = transform.position + direction * offset;
            var newNode = CreateNewNodeAtPosition(newPosition, createNewAsDst);
            return newNode;
        }
    }
}