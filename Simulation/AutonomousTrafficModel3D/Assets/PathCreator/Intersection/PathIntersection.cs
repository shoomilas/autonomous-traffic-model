using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using PathCreator.Aggregator;
using UnityEditor;
using UnityEngine;

namespace PathCreator.Intersection {
    public interface IIntersectionGenerator {
        void RegenerateIntersection(PathIntersection intersection);
        void RemoveIntersectionSplines(PathIntersection intersection);
    }

    public class DefaultIntersectionGenerator : IIntersectionGenerator {
        public void RegenerateIntersection(PathIntersection intersection) {
            Debug.Log("Default Intersection Generation");
            var i = intersection;
            RemoveIntersectionSplines(i); // TODO: REMOVE INTERSECTION SPLINES
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
                Undo.RecordObject(pathNode,"Remove a splineOutData path node entry");
                Undo.DestroyObjectImmediate(splineOutData.spline.gameObject);
                toRemove.Add(splineOutData);
            }
            foreach (var splineOutData in toRemove) {
                pathNode.SplinesOut.Remove(splineOutData);
            }
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
            var generatedSplineOutData = new SplineOutData(splineGenerated, direction, dstNode);
            srcNode.SplinesOut.Add(generatedSplineOutData);
        }
    }

    public class PathIntersection : MonoBehaviour {
        [ItemCanBeNull] public List<PathNode> InputsA;
        [ItemCanBeNull] public List<PathNode> InputsB;
        [ItemCanBeNull] public List<PathNode> InputsC;
        [ItemCanBeNull] public List<PathNode> InputsD;
        [ItemCanBeNull] public List<PathNode> OutputsA;
        [ItemCanBeNull] public List<PathNode> OutputsB;
        [ItemCanBeNull] public List<PathNode> OutputsC;
        [ItemCanBeNull] public List<PathNode> OutputsD;

        [Range(0.3f, 20f)] public float size = 5f;

        // IIntersectionQueueHandler handler;
        // List<VehicleIntersectionVisa> VehicleQueue;
        private readonly IIntersectionGenerator IntersectionGenerator = new DefaultIntersectionGenerator();

        // public Vector3 intersectionSize = new Vector3(defaultSize, defaultSize, defaultSize);

        private void OnDrawGizmos() {
            var pos = transform.position;
            var sizeVector = new Vector3(size, size / 2, size);
            Gizmos.DrawWireCube(pos, sizeVector);
            // Gizmos.DrawWireSphere( pos, size*2); // TODO: These will come with the handler though
            // Gizmos.DrawWireSphere( pos, size);
        }

        public void RegenerateIntersection() {
            IntersectionGenerator.RegenerateIntersection(this);
        }

        public void RemoveIntersectionSplines() {
            IntersectionGenerator.RemoveIntersectionSplines(this);
        }
    }
}