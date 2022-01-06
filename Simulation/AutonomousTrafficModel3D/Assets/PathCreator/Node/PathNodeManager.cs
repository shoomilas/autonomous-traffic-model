using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace PathCreator.Aggregator {
    // [CreateAssetMenu(menuName = "Scriptable Objects/PathNodeManager")]
    // public class PathNodeManager : ScriptableObject {
    public class PathNodeManager : MonoBehaviour {
        public static int NodeCounter = 0;
        public static List<PathNode> AllThePathNodes = new List<PathNode>();

        // private void Execute() { }
        // private void OnValidate() { }
    }
}