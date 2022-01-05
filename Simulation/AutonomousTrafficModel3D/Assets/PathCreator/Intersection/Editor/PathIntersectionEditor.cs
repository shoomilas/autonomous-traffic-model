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
        private const string TextAnchorPathNodesButton = "Anchor Path Nodes";
        public PathIntersectionPositionManger PositionManager;
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
            if (GUILayout.Button(TextAnchorPathNodesButton)) {
                typedTarget.AnchorPathNodesToIntersection(PositionManager);
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
            if (PositionManager == null) {
                PositionManager = typedTarget.GetComponent<PathIntersectionPositionManger>();
                PositionManager = PositionManager.PrepData();
            }
            if (Event.current.type == EventType.Repaint && PositionManager != null) {
                var sizeOfInMark = .2f;
                Handles.color = Color.gray;
                Handles.DrawLine(PositionManager.Sides.A, PositionManager.Sides.C);
                Handles.DrawLine(PositionManager.Sides.B, PositionManager.Sides.D);
                Handles.Label(PositionManager.Sides.A, "A");
                Handles.Label(PositionManager.Sides.B, "B");
                Handles.Label(PositionManager.Sides.C, "C");
                Handles.Label(PositionManager.Sides.D, "D");
                DrawInOutMarks(PositionManager.InsOuts, sizeOfInMark);
            }
        }
    }
}