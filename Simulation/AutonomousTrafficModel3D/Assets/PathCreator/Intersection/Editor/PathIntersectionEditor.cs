using System;
using System.Linq;
using PathCreator.Intersection;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace {
    [CustomEditor(typeof(PathIntersection))]
    public class PathIntersectionEditor : Editor {
        private const string TextRegenerateIntersectionButton = "Regenerate Intersection";
        private const string TextRemoveIntersectionSplinesButton = "Remove Intersection Splines";
        public PathIntersectionPositionManger _positionManger;
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
        private void OnEnable()
        {
            SceneView.duringSceneGui += CustomOnSceneGUI;
        }
        
        private void OnDisable() {
            SceneView.duringSceneGui -= CustomOnSceneGUI;
        }
        
        private void CustomOnSceneGUI(SceneView view) {
            DrawHandles();
        }

        private void DrawInOutMarks(IntersectionInsOuts posVectors, float sizeOfMark) {
            posVectors.InsA
                .Concat(posVectors.InsB)
                .Concat(posVectors.InsC)
                .Concat(posVectors.InsD)
                .Concat(posVectors.OutsA)
                .Concat(posVectors.OutsB)
                .Concat(posVectors.OutsC)
                .Concat(posVectors.OutsD)
                .ToList().ForEach( posVector =>
            {
                Handles.DrawWireDisc(posVector, Vector3.up, sizeOfMark);
            });
        }

        private void DrawHandles() {
            if (_positionManger == null) {
                _positionManger = typedTarget.GetComponent<PathIntersectionPositionManger>();
                _positionManger = _positionManger.PrepData();
            }
            if (Event.current.type == EventType.Repaint && _positionManger != null) {
                var sizeOfInMark = .2f;
                Handles.color = Color.gray;
                Handles.DrawLine(_positionManger.Sides.A, _positionManger.Sides.C);
                Handles.DrawLine(_positionManger.Sides.B, _positionManger.Sides.D);
                Handles.Label(_positionManger.Sides.A, "A");
                Handles.Label(_positionManger.Sides.B, "B");
                Handles.Label(_positionManger.Sides.C, "C");
                Handles.Label(_positionManger.Sides.D, "D");
                DrawInOutMarks(_positionManger.InsOuts, sizeOfInMark);
            }
        }
    }
}