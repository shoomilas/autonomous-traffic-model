using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using PathCreator.Aggregator;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace PathCreator.Intersection {
    public interface IIntersectionGenerator {
        void RegenerateIntersection(PathIntersection intersection);
        void RemoveIntersectionSplines(PathIntersection intersection);
        void AnchorPathNodesToIntersection(PathIntersection intersection, IntersectionPositionData intersectionPositionData);
    }

    public class DefaultIntersectionGenerator : IIntersectionGenerator {
        public void RegenerateIntersection(PathIntersection intersection) {
            Debug.Log("Default Intersection Generation");
            var i = intersection;
            RemoveIntersectionSplines(i);
            ManyLaneIntersectionGeneratorForGivenInputNodes(i.InputsA,
                i.OutputsB,
                i.OutputsC,
                i.OutputsD);
            ManyLaneIntersectionGeneratorForGivenInputNodes(i.InputsB,
                i.OutputsC,
                i.OutputsD,
                i.OutputsA);
            ManyLaneIntersectionGeneratorForGivenInputNodes(i.InputsC,
                i.OutputsD,
                i.OutputsA,
                i.OutputsB);
            ManyLaneIntersectionGeneratorForGivenInputNodes(i.InputsD,
                i.OutputsA,
                i.OutputsB,
                i.OutputsC);
        }

        public void RemoveIntersectionSplines(PathIntersection intersection) {
            intersection.InputsA?.ForEach(inputNode => RemoveSplinesForSinglePathNode(inputNode));
            intersection.InputsB?.ForEach(inputNode => RemoveSplinesForSinglePathNode(inputNode));
            intersection.InputsC?.ForEach(inputNode => RemoveSplinesForSinglePathNode(inputNode));
            intersection.InputsD?.ForEach(inputNode => RemoveSplinesForSinglePathNode(inputNode));
        }

        public void RemoveSplinesForSinglePathNode(PathNode pathNode) {
            var toRemove = new List<SplineOutData>();            
            foreach (var splineOutData in pathNode.SplinesOut) {
                pathNode.nextPathNodes.Remove(splineOutData.dstNode);
                if (splineOutData.spline != null && splineOutData.spline.gameObject != null) {
                    Undo.RecordObject(pathNode,"Remove a splineOutData path node entry");
                    Undo.DestroyObjectImmediate(splineOutData.spline.gameObject);
                }
                else {
                    Debug.Log($"Null value occured for {pathNode.name}");
                }
            }
            pathNode.nextPathNodes.Clear();
            pathNode.SplinesOut.Clear();
        }

        private void OneLaneIntersectionGeneratorForGivenInputNode(PathNode inputNode, PathNode outputRight,
            PathNode outputForward,
            PathNode outputLeft) {
            GenerateSplinesBetweenIntersectionNodes(inputNode, outputRight, Direction.Right);
            GenerateSplinesBetweenIntersectionNodes(inputNode, outputForward, Direction.Forward);
            GenerateSplinesBetweenIntersectionNodes(inputNode, outputLeft, Direction.Left);
        }

        private void ManyLaneIntersectionGeneratorForGivenInputNodes(List<PathNode> inputs, List<PathNode> outputsRight,
            List<PathNode> outputsForward, List<PathNode> outputsLeft) {
            inputs?.ForEach(inputNode => {
                outputsRight?.ForEach(outputRight =>
                    GenerateSplinesBetweenIntersectionNodes(inputNode, outputRight, Direction.Right));
                outputsForward?.ForEach(outputForward =>
                    GenerateSplinesBetweenIntersectionNodes(inputNode, outputForward, Direction.Forward));
                outputsLeft?.ForEach(outputLeft =>
                    GenerateSplinesBetweenIntersectionNodes(inputNode, outputLeft, Direction.Forward));
            });
        }

        public void GenerateSplinesBetweenIntersectionNodes(PathNode srcNode, PathNode dstNode, Direction direction) {
            var splineGenerated = srcNode.ConnectNodes(dstNode);
            if (splineGenerated is null) {
                return;
            }
            var generatedSplineOutData = new SplineOutData(splineGenerated, direction, dstNode);
            srcNode.SplinesOut.Add(generatedSplineOutData);
        }

        
        private void SetPathNodesToGivenCoords(List<PathNode> pathNodes, List<Vector3> Coords) {
            int i = 0;            
            foreach (var pathNode in pathNodes) {
                pathNode.transform.position = Coords[i];
                i += 1;
            }
        }
        
        public void AnchorPathNodesToIntersection(PathIntersection intersection, IntersectionPositionData intersectionPositionData) {
            SetPathNodesToGivenCoords(intersection.InputsA, intersectionPositionData.InsOuts.InsA);
            SetPathNodesToGivenCoords(intersection.InputsB, intersectionPositionData.InsOuts.InsB);
            SetPathNodesToGivenCoords(intersection.InputsC, intersectionPositionData.InsOuts.InsC);
            SetPathNodesToGivenCoords(intersection.InputsD, intersectionPositionData.InsOuts.InsD);
            SetPathNodesToGivenCoords(intersection.OutputsA, intersectionPositionData.InsOuts.OutsA);
            SetPathNodesToGivenCoords(intersection.OutputsB, intersectionPositionData.InsOuts.OutsB);
            SetPathNodesToGivenCoords(intersection.OutputsC, intersectionPositionData.InsOuts.OutsC);
            SetPathNodesToGivenCoords(intersection.OutputsD, intersectionPositionData.InsOuts.OutsD);
        }  
    }
    
    public class PathIntersection : MonoBehaviour {
        // IIntersectionQueueHandler handler;
        // List<VehicleIntersectionVisa> VehicleQueue;
        [Range(0.3f, 20f)] public float size = 5f;
        [ItemCanBeNull] public List<PathNode> InputsA = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> InputsB = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> InputsC = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> InputsD = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsA = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsB = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsC = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsD = new List<PathNode>();
        private readonly IIntersectionGenerator IntersectionGenerator = new DefaultIntersectionGenerator();
        [HideInInspector] public bool minimalHandles = true;
        public bool keepFullHandlesWhenDeselected = false;
        public bool keepHandlesWhenDeselected = true;

        public void RegenerateIntersection() {
            IntersectionGenerator.RegenerateIntersection(this);
        }

        public void RemoveIntersectionSplines() {
            IntersectionGenerator.RemoveIntersectionSplines(this);
        }

        public void AnchorPathNodesToIntersection(IntersectionPositionData intersectionPositionData) {
            IntersectionGenerator.AnchorPathNodesToIntersection(this, intersectionPositionData);
        }

        private void OnDrawGizmos() {
            if (Event.current.type == EventType.Repaint && this.minimalHandles) {
                Handles.color = Color.gray;
                var pos = this.transform.position;
                var ySize = this.size / 2;
                var sizeVector = new Vector3(this.size*2, ySize, this.size*2);
                var posVector = pos + Vector3.up * (ySize/2);
                Handles.DrawWireCube(posVector, sizeVector);
            }
        }
    }
}