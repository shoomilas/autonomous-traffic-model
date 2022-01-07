﻿using System;
using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace PathCreator.Vehicles {
    public interface IVehiclePathProvider {
        List<PathNode> Provide(PathNode startNode);
        public PathProviderMethod CurrentMethod {get;set;}
    }
    
    public enum PathProviderMethod{
        FirstFound,
        AlwaysRight,
        AlwaysForward,
        AlwaysLeft,
        AlwaysRandom,
        AlwaysRandomRightForward,
        AlwaysRandomLeftRight,
        AlwaysRandomLeftForward
    }
    
    public class VehiclePathProvider : MonoBehaviour, IVehiclePathProvider {
        private List<PathNode> finalPath = new List<PathNode>();
        public PathProviderMethod currentMethod = PathProviderMethod.FirstFound;

        public PathProviderMethod CurrentMethod
        {
            get => currentMethod;
            set => currentMethod = value;
        }
        
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
                PathProviderMethod.AlwaysRandom => GetPathNodesAlwaysInDirection(startNode,
                    EnumGetRandomValue(Direction.Left, Direction.Right, Direction.Forward)),
                PathProviderMethod.AlwaysRandomRightForward => GetPathNodesAlwaysInDirection(startNode,
                    EnumGetRandomValue(Direction.Right, Direction.Forward)),
                PathProviderMethod.AlwaysRandomLeftRight => GetPathNodesAlwaysInDirection(startNode,
                    EnumGetRandomValue(Direction.Left, Direction.Right)),
                PathProviderMethod.AlwaysRandomLeftForward => GetPathNodesAlwaysInDirection(startNode,
                    EnumGetRandomValue(Direction.Left, Direction.Forward)),
                _ => new List<PathNode>()
            };
            return path.ToList();
        }
        
        public IEnumerable<PathNode> GetPathNodes(PathNode firstNode) {
            if (firstNode == null) {
                Debug.Log($"Path generated by PathProvider for {transform.name} was null");
                yield break;
            }

            yield return firstNode;
            var iteratorNode = firstNode.nextPathNodes.FirstOrDefault();
            while (iteratorNode!=null) {
                var currentNode = iteratorNode;
                iteratorNode = currentNode.nextPathNodes.FirstOrDefault();
                yield return currentNode;
            }
        }

        private PathNode GetFirstMatchingNextNode(PathNode firstNode, Direction direction, bool returnAnyFirstIfDirNotFound = true) {
            if (firstNode.SplinesOut == null || !firstNode.SplinesOut.Any()) {
                return null;
            }
            foreach (var splineOut in firstNode.SplinesOut) {
                if (splineOut.splineDirection == direction) {
                    return splineOut.dstNode;
                }
            }
            if (returnAnyFirstIfDirNotFound) {
                return firstNode.SplinesOut.FirstOrDefault().dstNode;
            }

            return null;
        }

        public IEnumerable<PathNode> GetPathNodesAlwaysInDirection(PathNode firstNode, Direction direction) {
            if (firstNode == null) {
                Debug.Log($"Path generated by PathProvider for {transform.name} was null");
                yield break;
            }

            var iteratorNode = firstNode;
            yield return firstNode;
            int i = 0;
            while (iteratorNode != null) {
                iteratorNode = GetFirstMatchingNextNode(iteratorNode, direction);
                if(iteratorNode != null) yield return iteratorNode;
            }
        }
    }
}