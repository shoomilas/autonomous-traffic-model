using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using UnityEngine;

namespace PathCreator.Vehicles {
    public interface IVehiclePathProvider {
        List<PathNode> Provide(PathNode startNode);
    }
    
    // public class VehiclePathRandomProvider : IVehiclePathProvider { }
    // public class VehiclePathNeverLeftProvider : IVehiclePathProvider { }
    public enum PathProviderMethod{
        FirstFound,
        AlwaysRight,
        AlwaysForward,
        AlwaysLeft,
        Random,
        RandomRightForward,
    }

    public class VehiclePathProvider : MonoBehaviour, IVehiclePathProvider {
        private List<PathNode> finalPath = new List<PathNode>();
        public PathProviderMethod currentMethod = PathProviderMethod.FirstFound; 

        public List<PathNode> Provide(PathNode startNode) {
            var path = currentMethod switch
            {
                PathProviderMethod.FirstFound => GetPathNodes(startNode),
                PathProviderMethod.AlwaysRight => GetPathNodesAlwaysRightOnBranched(startNode),
                PathProviderMethod.AlwaysForward => GetPathNodesAlwaysForwardOrUnknownOnBranched(startNode),
                _ => GetPathNodes(startNode)
            };
            return path.ToList();
        }
        
        public IEnumerable<PathNode> GetPathNodes(PathNode firstNode) {
            yield return firstNode;
            var iteratorNode = firstNode.nextPathNodes.FirstOrDefault();
            while (iteratorNode!=null) {
                var currentNode = iteratorNode;
                iteratorNode = currentNode.nextPathNodes.FirstOrDefault();
                yield return currentNode;
            }
        }

        private PathNode FindNextNodeInExpectedDirectionOrGetFirst(PathNode firstNode, Direction direction, bool shouldGetFirstIfNotFound = true) {
            PathNode nextNode;
            try {
                nextNode = firstNode.SplinesOut.Find(_ => _.splineDirection == direction).dstNode;
            }
            catch {
                if (shouldGetFirstIfNotFound) {
                    nextNode = firstNode.SplinesOut.FirstOrDefault()?.dstNode;
                }
                else {
                    nextNode = null;
                }
            }

            return nextNode;
        }

        public IEnumerable<PathNode> GetPathNodesAlwaysRightOrUnknownOnBranched(PathNode firstNode) {
            yield return firstNode;
            var iteratorNode = FindNextNodeInExpectedDirectionOrGetFirst(firstNode, Direction.Right);
            while (iteratorNode!=null) {
                var currentNode = iteratorNode;
                iteratorNode = FindNextNodeInExpectedDirectionOrGetFirst(firstNode, Direction.Right);
                yield return currentNode;
            }
        }
        
        public IEnumerable<PathNode> GetPathNodesAlwaysForwardOrUnknownOnBranched(PathNode firstNode) {
        //     yield return firstNode;
        //     var iteratorNode = FindNextNodeInExpectedDirectionOrGetFirst(firstNode, Direction.Forward);
        //     while (iteratorNode!=null) {
        //         var currentNode = iteratorNode;
        //         iteratorNode = FindNextNodeInExpectedDirectionOrGetFirst(firstNode, Direction.Forward);
        //         yield return currentNode;
        //     }
        // }
            yield return firstNode;
            PathNode iteratorNode;
            try {
                iteratorNode = firstNode.SplinesOut.Find(_ => _.splineDirection == Direction.Forward).dstNode;
            }
            catch {
                iteratorNode = firstNode.SplinesOut.FirstOrDefault()?.dstNode;
            }
                
            while (iteratorNode!=null) {
                var currentNode = iteratorNode;
                try {
                    iteratorNode = currentNode.SplinesOut.Find(_ => _.splineDirection == Direction.Forward).dstNode;
                }
                catch {
                    iteratorNode = currentNode.SplinesOut.FirstOrDefault()?.dstNode;
                }
                yield return currentNode;
            }
        }

        public IEnumerable<PathNode> GetPathNodesAlwaysRightOnBranched(PathNode firstNode) {
            yield return firstNode;
            PathNode iteratorNode;
            try {
                iteratorNode = firstNode.SplinesOut.Find(_ => _.splineDirection == Direction.Right).dstNode;
            }
            catch {
                iteratorNode = firstNode.SplinesOut.FirstOrDefault()?.dstNode;
            }
            
            while (iteratorNode!=null) {
                var currentNode = iteratorNode;
                try {
                    iteratorNode = currentNode.SplinesOut.Find(_ => _.splineDirection == Direction.Right).dstNode;
                }
                catch {
                    iteratorNode = currentNode.SplinesOut.FirstOrDefault()?.dstNode;
                }
                yield return currentNode;
            }
        }
    }
}