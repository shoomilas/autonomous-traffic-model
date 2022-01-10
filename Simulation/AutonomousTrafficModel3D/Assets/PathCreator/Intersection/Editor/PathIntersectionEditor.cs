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
        public IntersectionPositionData PositionData;
        private PathIntersection typedTarget;

        private void OnEnable() {
            typedTarget = (PathIntersection)target;
            PositionManager = typedTarget.GetComponent<PathIntersectionPositionManger>();
            if (PositionManager == null)
                PositionManager = typedTarget.gameObject.AddComponent<PathIntersectionPositionManger>();

            PositionData = PositionManager.PrepData(typedTarget);
            typedTarget.minimalHandles = false;
            SceneView.duringSceneGui += CustomOnSceneGUI;
        }

        private void OnDisable() {
            if (!typedTarget.keepFullHandlesWhenDeselected) typedTarget.minimalHandles = true;
            if (!typedTarget.keepHandlesWhenDeselected) SceneView.duringSceneGui -= CustomOnSceneGUI;
        }

        private void OnValidate() {
            typedTarget = (PathIntersection)target;
            SceneView.duringSceneGui += CustomOnSceneGUI;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button(TextRegenerateIntersectionButton)) typedTarget.RegenerateIntersection();
            if (GUILayout.Button(TextRemoveIntersectionSplinesButton)) typedTarget.RemoveIntersectionSplines();
            if (GUILayout.Button(TextAnchorPathNodesButton))
                typedTarget.AnchorPathNodesToIntersection(PositionManager.PrepData(typedTarget));
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
                .ToList().ForEach(posVector => { Handles.DrawWireDisc(posVector, Vector3.up, sizeOfMark); });
        }

        public void DrawHandles() {
            if (typedTarget == null) {
                SceneView.duringSceneGui -= CustomOnSceneGUI;
                return;
            }

            if (Event.current.type == EventType.Repaint && typedTarget.minimalHandles) {
                Handles.color = Color.gray;
                var pos = typedTarget.transform.position;
                var ySize = typedTarget.size / 2;
                var sizeVector = new Vector3(typedTarget.size * 2, ySize, typedTarget.size * 2);
                var posVector = pos + Vector3.up * (ySize / 2);
                Handles.DrawWireCube(posVector, sizeVector);
                return;
            }

            if (Event.current.type == EventType.Repaint) {
                PositionData = PositionManager.PrepData(typedTarget);
                var sizeOfInMark = .2f;
                Handles.color = Color.gray;
                Handles.DrawLine(PositionData.Sides.A, PositionData.Sides.C);
                Handles.DrawLine(PositionData.Sides.B, PositionData.Sides.D);
                Handles.Label(PositionData.Sides.A, "A");
                Handles.Label(PositionData.Sides.B, "B");
                Handles.Label(PositionData.Sides.C, "C");
                Handles.Label(PositionData.Sides.D, "D");
                DrawInOutMarks(PositionData.InsOuts, sizeOfInMark);

                Handles.color = Color.white;
                var pos = typedTarget.transform.position;
                var ySize = typedTarget.size / 2;
                var sizeVector = new Vector3(typedTarget.size * 2, ySize, typedTarget.size * 2);
                var posVector = pos + Vector3.up * (ySize / 2);
                Handles.DrawWireCube(posVector, sizeVector);
                Handles.color = Color.gray;
                posVector = pos - Vector3.up * (ySize / 2);
                Handles.DrawWireCube(posVector, sizeVector);
            }
        }
    }
}