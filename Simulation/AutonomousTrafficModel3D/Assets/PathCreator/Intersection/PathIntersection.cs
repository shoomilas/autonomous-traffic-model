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
        void ReanchorPathNodesToIntersection(PathIntersectionPositionManger positionManager);
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
        
        public void ReanchorPathNodesToIntersection(PathIntersectionPositionManger positionManager) {
            return;
        }  
    }

    public class PathIntersection : MonoBehaviour {
        // IIntersectionQueueHandler handler;
        // List<VehicleIntersectionVisa> VehicleQueue;
        [Range(0.3f, 20f)] public float size = 5f;
        [ItemCanBeNull] public List<PathNode> InputsA;
        [ItemCanBeNull] public List<PathNode> InputsB;
        [ItemCanBeNull] public List<PathNode> InputsC;
        [ItemCanBeNull] public List<PathNode> InputsD;
        [ItemCanBeNull] public List<PathNode> OutputsA;
        [ItemCanBeNull] public List<PathNode> OutputsB;
        [ItemCanBeNull] public List<PathNode> OutputsC;
        [ItemCanBeNull] public List<PathNode> OutputsD;
        private readonly IIntersectionGenerator IntersectionGenerator = new DefaultIntersectionGenerator();
        
        private void OnDrawGizmos() {
            var pos = transform.position;
            var ySize = size /2;
            var sizeVector = new Vector3(size*2, ySize, size*2);
            var posVector = pos + Vector3.up * (ySize/2);
            Gizmos.DrawWireCube(posVector, sizeVector);
            Gizmos.color = Color.gray;
            posVector = pos - Vector3.up * (ySize/2);
            Gizmos.DrawWireCube(posVector, sizeVector);
        }

        public void RegenerateIntersection() {
            IntersectionGenerator.RegenerateIntersection(this);
        }

        public void RemoveIntersectionSplines() {
            IntersectionGenerator.RemoveIntersectionSplines(this);
        }

        public void AnchorPathNodesToIntersection(PathIntersectionPositionManger positionManager) {
            IntersectionGenerator.ReanchorPathNodesToIntersection(positionManager);
        }
    }
}