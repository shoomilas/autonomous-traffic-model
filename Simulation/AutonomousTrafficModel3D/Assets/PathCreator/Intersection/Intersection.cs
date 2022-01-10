using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PathCreator.Aggregator;
using UnityEditor;
using UnityEngine;

namespace PathCreator.Intersection {
    public interface IIntersectionGenerator {
        void RegenerateIntersection(Intersection intersection);
        void RemoveIntersectionSplines(Intersection intersection);

        void AnchorPathNodesToIntersection(Intersection intersection,
            IntersectionPositionData intersectionPositionData);
    }

    public class DefaultIntersectionGenerator : IIntersectionGenerator {
        public void RegenerateIntersection(Intersection intersection) {
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

        public void RemoveIntersectionSplines(Intersection intersection) {
            ClearInputNodes(
                intersection.InputsA, intersection.InputsB, intersection.InputsC, intersection.InputsD
            );
            ClearOutputNodes(
                intersection.OutputsA, intersection.OutputsB, intersection.OutputsC, intersection.OutputsD
            );
        }

        public void AnchorPathNodesToIntersection(Intersection intersection,
            IntersectionPositionData intersectionPositionData) {
            SetPathNodesToGivenCoords(intersection.InputsA, intersectionPositionData.InsOuts.InsA);
            SetPathNodesToGivenCoords(intersection.InputsB, intersectionPositionData.InsOuts.InsB);
            SetPathNodesToGivenCoords(intersection.InputsC, intersectionPositionData.InsOuts.InsC);
            SetPathNodesToGivenCoords(intersection.InputsD, intersectionPositionData.InsOuts.InsD);
            SetPathNodesToGivenCoords(intersection.OutputsA, intersectionPositionData.InsOuts.OutsA);
            SetPathNodesToGivenCoords(intersection.OutputsB, intersectionPositionData.InsOuts.OutsB);
            SetPathNodesToGivenCoords(intersection.OutputsC, intersectionPositionData.InsOuts.OutsC);
            SetPathNodesToGivenCoords(intersection.OutputsD, intersectionPositionData.InsOuts.OutsD);
        }

        public void ClearInputNodes(params List<PathNode>[] outputs) {
            outputs?.SelectMany(_ => _).ToList().ForEach(RemoveSplinesForSinglePathNode);
        }

        public void ClearOutputNodes(params List<PathNode>[] outputs) {
            outputs?.SelectMany(_ => _).ToList().ForEach(node => node.previousPathNodes.Clear());
        }

        public void RemoveSplinesForSinglePathNode(PathNode pathNode) {
            var toRemove = new List<SplineOutData>();
            foreach (var splineOutData in pathNode.SplinesOut) {
                pathNode.nextPathNodes.Remove(splineOutData.dstNode);
                if (splineOutData.spline != null && splineOutData.spline.gameObject != null) {
                    Undo.RecordObject(pathNode, "Remove a splineOutData path node entry");
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

        private void ManyLaneIntersectionGeneratorForGivenInputNodes(List<PathNode> inputs,
            List<PathNode> outputsRight,
            List<PathNode> outputsForward,
            List<PathNode> outputsLeft) {
            inputs?.ForEach(inputNode => {
                outputsRight?.ForEach(outputRight =>
                    GenerateSplinesBetweenIntersectionNodes(inputNode,
                        outputRight,
                        Direction.Right));
                outputsForward?.ForEach(outputForward =>
                    GenerateSplinesBetweenIntersectionNodes(inputNode,
                        outputForward,
                        Direction.Forward));
                outputsLeft?.ForEach(outputLeft =>
                    GenerateSplinesBetweenIntersectionNodes(inputNode,
                        outputLeft,
                        Direction.Left));
            });
        }

        public void GenerateSplinesBetweenIntersectionNodes(PathNode srcNode, PathNode dstNode, Direction direction) {
            var splineGenerated = srcNode.ConnectNodes(dstNode);
            if (splineGenerated is null) return;

            var generatedSplineOutData = new SplineOutData(splineGenerated, direction, dstNode);
            srcNode.SplinesOut.Add(generatedSplineOutData);
        }


        private void SetPathNodesToGivenCoords(List<PathNode> pathNodes, List<Vector3> Coords) {
            var i = 0;
            foreach (var pathNode in pathNodes) {
                pathNode.transform.position = Coords[i];
                i += 1;
            }
        }
    }

    public class Intersection : MonoBehaviour {
        [Range(0.3f, 20f)] public float size = 5f;
        [ItemCanBeNull] public List<PathNode> InputsA = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> InputsB = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> InputsC = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> InputsD = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsA = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsB = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsC = new List<PathNode>();
        [ItemCanBeNull] public List<PathNode> OutputsD = new List<PathNode>();
        [HideInInspector] public bool minimalHandles = true;
        public bool keepFullHandlesWhenDeselected;
        public bool keepHandlesWhenDeselected = true;
        private readonly IIntersectionGenerator IntersectionGenerator = new DefaultIntersectionGenerator();

        private void OnDrawGizmos() {
            if (Event.current.type == EventType.Repaint && minimalHandles) {
                Handles.color = Color.gray;
                var pos = transform.position;
                var ySize = size / 2;
                var sizeVector = new Vector3(size * 2, ySize, size * 2);
                var posVector = pos + Vector3.up * (ySize / 2);
                Handles.DrawWireCube(posVector, sizeVector);
            }
        }

        public void RegenerateIntersection() {
            IntersectionGenerator.RegenerateIntersection(this);
        }

        public void RemoveIntersectionSplines() {
            IntersectionGenerator.RemoveIntersectionSplines(this);
        }

        public void AnchorPathNodesToIntersection(IntersectionPositionData intersectionPositionData) {
            IntersectionGenerator.AnchorPathNodesToIntersection(this, intersectionPositionData);
        }
    }
}