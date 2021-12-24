using System.IO;
using UnityEngine;
using UnityEditor;

namespace PathCreator.Aggregator.Editor {
    [CustomEditor(typeof(PathAggregator))]
    public class PathAggregatorEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var pathAggregator = (PathAggregator)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.IntField(5);
            if (GUILayout.Button("Yo")) {
                Debug.Log("Pressed");
            }
            
            GUILayout.EndHorizontal();
        }
    }
}