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

    public class VehiclePathProvider : MonoBehaviour, IVehiclePathProvider {
        private List<PathNode> finalPath = new List<PathNode>();

        public List<PathNode> Provide(PathNode startNode) {
            var path = GetPathNodes(startNode);
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
    }
}