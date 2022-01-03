using PathCreator.Intersection;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace {
    [CustomEditor(typeof(PathIntersection))]
    public class PathIntersectionEditor : Editor {
        private const string TextRegenerateIntersectionButton = "Regenerate Intersection"; 
        public override void OnInspectorGUI() {            
            base.OnInspectorGUI();
            var typedTarget = (PathIntersection)target;
            if (GUILayout.Button(TextRegenerateIntersectionButton)) {
                typedTarget.RegenerateIntersection();
            }
        }
    }
}