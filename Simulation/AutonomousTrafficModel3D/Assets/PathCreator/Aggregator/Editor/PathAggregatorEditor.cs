using UnityEditor;
using UnityEngine;

namespace PathCreator.Aggregator.Editor {
    [CustomEditor(typeof(PathAggregator))]
    public class PathAggregatorEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var pathAggregator = (PathAggregator)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.IntField(5);
            GUILayout.EndHorizontal();
        }
    }
}