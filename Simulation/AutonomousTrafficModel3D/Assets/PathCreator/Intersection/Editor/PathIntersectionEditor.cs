using System;
using PathCreator.Intersection;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace {
    [CustomEditor(typeof(PathIntersection))]
    public class PathIntersectionEditor : Editor {
        private const string TextRegenerateIntersectionButton = "Regenerate Intersection";
        private const string TextRemoveIntersectionSplinesButton = "Remove Intersection Splines";
        private PathIntersection typedTarget;
        public override void OnInspectorGUI() {            
            base.OnInspectorGUI();
            typedTarget = (PathIntersection)target;
            if (GUILayout.Button(TextRegenerateIntersectionButton)) {
                typedTarget.RegenerateIntersection();
            }
            if (GUILayout.Button(TextRemoveIntersectionSplinesButton)) {
                typedTarget.RemoveIntersectionSplines();
            }
        }

        private void OnSceneGUI() {
            DrawHandles();
        }

        private void DrawHandles() {
            if (Event.current.type == EventType.Repaint) {
                var size = 2f;
                var center = typedTarget.transform.position;
                var rotation = typedTarget.transform.rotation;

                var vertex1 = center + Vector3.forward * (size / 2) + Vector3.right * (size / 2);
                
                var dir = vertex1 - center;
                var vertexRotated = Quaternion.Euler(
                    rotation.x,
                    rotation.y,
                    rotation.z
                    ) * Vector3.up;
                
                
                var finalPos = vertexRotated + Vector3.up * size / 2;
                Handles.CylinderHandleCap(
                    0,
                    finalPos,
                    typedTarget.transform.rotation * Quaternion.LookRotation(Vector3.up),
                    size,
                        EventType.Repaint
                );
                
                // Handles.ConeHandleCap(
                //     0, 
                //     vertex1, 
                //     typedTarget.transform.rotation * Quaternion.LookRotation(Vector3.up),
                //     3f,
                //     EventType.Repaint
                //     );

                Handles.DrawLine(vertex1 + Vector3.up, vertex1 + Vector3.down);
            }
        }
    }
}