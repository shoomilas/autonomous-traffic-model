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
    }

    public class VehiclePathProvider : MonoBehaviour, IVehiclePathProvider {
        private List<PathNode> finalPath = new List<PathNode>();
        public PathProviderMethod currentMethod = PathProviderMethod.FirstFound; 

        public List<PathNode> Provide(PathNode startNode) {
            var path = currentMethod switch
            {
                PathProviderMethod.FirstFound => GetPathNodes(startNode),
                PathProviderMethod.AlwaysRight => GetPathNodesAlwaysRightOnBranched(startNode),
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