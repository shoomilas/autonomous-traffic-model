using System;
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
        AlwaysRandom,
        AlwaysRandomRightForward,
    }
    
    public class VehiclePathProvider : MonoBehaviour, IVehiclePathProvider {
        private List<PathNode> finalPath = new List<PathNode>();
        public PathProviderMethod currentMethod = PathProviderMethod.FirstFound; 

        public static T EnumGetRandomValue<T>() where T : Enum => 
            Enum.GetValues(typeof(T))
                .OfType<T>()
                .OrderBy(_ =>Guid.NewGuid())
                .FirstOrDefault();
        
        public static T EnumGetRandomValue<T>(params T[] allowedValues) where T : Enum => 
                Enum.GetValues(typeof(T))
                    .OfType<T>()
                    .Where(allowedValues.Contains)
                    .OrderBy(_ =>Guid.NewGuid())
                    .FirstOrDefault();
        
        public List<PathNode> Provide(PathNode startNode) {
            var path = currentMethod switch
            {
                PathProviderMethod.FirstFound => GetPathNodes(startNode),
                PathProviderMethod.AlwaysRight => GetPathNodesAlwaysInDirection(startNode, Direction.Right),
                PathProviderMethod.AlwaysForward => GetPathNodesAlwaysInDirection(startNode, Direction.Forward),
                PathProviderMethod.AlwaysLeft => GetPathNodesAlwaysInDirection(startNode, Direction.Left),
                PathProviderMethod.AlwaysRandom => GetPathNodesAlwaysInDirection(startNode, EnumGetRandomValue<Direction>()),
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
        
        public IEnumerable<PathNode> GetPathNodesAlwaysInDirection(PathNode firstNode, Direction direction) {
            yield return firstNode;
            PathNode iteratorNode;
            try {
                iteratorNode = firstNode.SplinesOut.Find(_ => _.splineDirection == direction).dstNode;
            }
            catch {
                iteratorNode = firstNode.SplinesOut.FirstOrDefault()?.dstNode;
            }
            
            while (iteratorNode!=null) {
                var currentNode = iteratorNode;
                try {
                    iteratorNode = currentNode.SplinesOut.Find(_ => _.splineDirection == direction).dstNode;
                }
                catch {
                    iteratorNode = currentNode.SplinesOut.FirstOrDefault()?.dstNode;
                }
                yield return currentNode;
            }
        }
    }
}