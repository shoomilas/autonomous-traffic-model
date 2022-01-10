#if UNITY_EDITOR

#region "Imports"

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#endregion


namespace RoadArchitect {
    [CustomEditor(typeof(RoadSystem))]
    public class RoadSystemEditor : Editor {


        private void OnEnable() {
            roadSystem = (RoadSystem)target;

            isTempMultithreading = serializedObject.FindProperty("isMultithreaded");
            isTempSaveMeshAssets = serializedObject.FindProperty("isSavingMeshes");
        }


        public void OnSceneGUI() {
            DoHotKeyCheck();
        }


        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorStyles.label.wordWrap = true;

            if (!isInitialized) {
                isInitialized = true;
                InitChecks();
            }

            //Add road button:
            EditorUtilities.DrawLine();
            if (GUILayout.Button("Add road", loadButton, GUILayout.Width(128f)))
                Selection.activeObject = roadSystem.AddRoad();
            EditorUtilities.DrawLine();

            //Update all roads button:
            if (GUILayout.Button("Update all roads", EditorStyles.miniButton, GUILayout.Width(120f)))
                roadSystem.UpdateAllRoads();

            //Multi-threading input:
            isTempMultithreading.boolValue =
                EditorGUILayout.Toggle("Multi-threading enabled", roadSystem.isMultithreaded);
            if (isTempMultithreading.boolValue != roadSystem.isMultithreaded)
                roadSystem.UpdateAllRoadsMultiThreadedOption(isTempMultithreading.boolValue);

            //Save mesh assets input:
            isTempSaveMeshAssets.boolValue = EditorGUILayout.Toggle("Save mesh assets: ", roadSystem.isSavingMeshes);
            if (isTempSaveMeshAssets.boolValue != roadSystem.isSavingMeshes)
                roadSystem.UpdateAllRoadsSavingMeshesOption(isTempSaveMeshAssets.boolValue);
            if (roadSystem.isSavingMeshes)
                GUILayout.Label(
                    "WARNING: Saving meshes as assets is very slow and can increase road generation time by several minutes.",
                    warningLabelStyle);

            //Online manual button:
            GUILayout.Space(4f);
            if (GUILayout.Button("Online manual", EditorStyles.miniButton, GUILayout.Width(120f)))
                Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/wiki");

            //Offline manual button:
            GUILayout.Space(4f);
            if (GUILayout.Button("Offline manual", EditorStyles.miniButton, GUILayout.Width(120f)))
                EditorUtilities.OpenOfflineManual();

            if (roadSystem.editorPlayCamera == null) roadSystem.EditorCameraSetSingle();
            EditorUtilities.DrawLine();

            //View intersection
            IntersectionView();
            //View bridges
            BridgeView();
            if (bridges.Length > 0 || intersections.Length > 0)
                EditorGUILayout.LabelField(
                    "* Hotkeys only work when this RoadArchitectSystem object is selected and the scene view has focus",
                    EditorStyles.miniLabel);


            //Hotkey check:
            DoHotKeyCheck();

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(roadSystem);
            }
        }


        private void InitChecks() {
            var basePath = RoadEditorUtility.GetBasePath();

            EditorUtilities.LoadTexture(ref warningLabelBG, basePath + "/Editor/Icons/WarningLabelBG.png");
            EditorUtilities.LoadTexture(ref loadButtonBG, basePath + "/Editor/Icons/otherbg.png");
            EditorUtilities.LoadTexture(ref loadButtonBGGlow, basePath + "/Editor/Icons/otherbg2.png");

            if (loadButton == null) {
                loadButton = new GUIStyle(GUI.skin.button);
                loadButton.contentOffset = new Vector2(0f, 1f);
                loadButton.normal.textColor = new Color(1f, 1f, 1f, 1f);
                loadButton.normal.background = loadButtonBG;
                loadButton.active.background = loadButtonBGGlow;
                loadButton.focused.background = loadButtonBGGlow;
                loadButton.hover.background = loadButtonBGGlow;
                loadButton.fixedHeight = 16f;
                loadButton.fixedWidth = 128f;
                loadButton.padding = new RectOffset(0, 0, 0, 0);
            }

            if (warningLabelStyle == null) {
                warningLabelStyle = new GUIStyle(GUI.skin.textArea);
                warningLabelStyle.normal.textColor = Color.red;
                warningLabelStyle.active.textColor = Color.red;
                warningLabelStyle.hover.textColor = Color.red;
                warningLabelStyle.normal.background = warningLabelBG;
                warningLabelStyle.active.background = warningLabelBG;
                warningLabelStyle.hover.background = warningLabelBG;
                warningLabelStyle.padding = new RectOffset(8, 8, 8, 8);
            }
        }


        private void IntersectionView() {
            //View intersection
            if (!isIntersectionInitialized) {
                isIntersectionInitialized = true;
                intersections = roadSystem.GetComponentsInChildren<RoadIntersection>();
            }

            if (intersections.Length > 0) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("View next intersection", GUILayout.Width(150f))) IncrementIntersection();
                EditorGUILayout.LabelField("Hotkey K");
                EditorGUILayout.EndHorizontal();
            }
        }


        private void IncrementIntersection() {
            if (intersections.Length > 0) {
                intersectionIndex += 1;
                if (intersectionIndex >= intersections.Length) intersectionIndex = 0;
                ShowIntersection();
            }
        }


        private void BridgeView() {
            //View bridges
            if (!isBridgeInitialized) {
                isBridgeInitialized = true;
                var nodes = roadSystem.transform.GetComponentsInChildren<SplineN>();
                var nodeList = new List<SplineN>();
                foreach (var node in nodes)
                    if (node.isBridgeStart && node.isBridgeMatched)
                        nodeList.Add(node);
                bridges = nodeList.ToArray();
                bridgesIndex = 0;
            }

            if (bridges.Length > 0) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("View next bridge", GUILayout.Width(150f))) IncrementBridge();
                EditorGUILayout.LabelField("Hotkey L");
                EditorGUILayout.EndHorizontal();
                if (EditorApplication.isPlaying) {
                    var isCameraFlipped = EditorGUILayout.Toggle("Flip camera Y:", isEditorCameraFlipped);
                    if (isCameraFlipped != isEditorCameraFlipped) {
                        isEditorCameraFlipped = isCameraFlipped;
                        ShowBridge();
                    }

                    var changeChecker = EditorGUILayout.Slider("Zoom factor:", cameraZoomFactor, 0.02f, 10f);
                    if (!RootUtils.IsApproximately(changeChecker, cameraZoomFactor, 0.001f)) {
                        cameraZoomFactor = changeChecker;
                        ShowBridge();
                    }

                    changeChecker = EditorGUILayout.Slider("Height offset:", cameraHeightOffset, 0f, 8f);
                    if (!RootUtils.IsApproximately(changeChecker, cameraHeightOffset, 0.001f)) {
                        cameraHeightOffset = changeChecker;
                        ShowBridge();
                    }

                    var isCustomRotated = EditorGUILayout.Toggle("Custom camera rot:", isCameraCustomRotated);
                    if (isCustomRotated != isCameraCustomRotated) {
                        isCameraCustomRotated = isCustomRotated;
                        ShowBridge();
                    }

                    if (isCameraCustomRotated) {
                        var changedRotation = default(Vector3);
                        changedRotation.x = EditorGUILayout.Slider("Rotation X:", customCameraRotation.x, -1f, 1f);
                        changedRotation.z = EditorGUILayout.Slider("Rotation Z:", customCameraRotation.z, -1f, 1f);

                        if (changedRotation != customCameraRotation) {
                            customCameraRotation = changedRotation;
                            ShowBridge();
                        }
                    }
                }
            }
        }


        private void IncrementBridge() {
            if (bridges.Length > 0) {
                bridgesIndex += 1;
                if (bridgesIndex >= bridges.Length) bridgesIndex = 0;
                ShowBridge();
            }
        }


        private void ShowIntersection() {
            if (EditorApplication.isPlaying && roadSystem.editorPlayCamera != null) {
                roadSystem.editorPlayCamera.transform.position = intersections[intersectionIndex].transform.position +
                                                                 new Vector3(-40f, 20f, -40f);
                roadSystem.editorPlayCamera.transform.rotation = Quaternion.LookRotation(
                    intersections[intersectionIndex].transform.position -
                    (intersections[intersectionIndex].transform.position + new Vector3(-40f, 20f, -40f)));
            }
            else {
                SceneView.lastActiveSceneView.pivot = intersections[intersectionIndex].transform.position;
                SceneView.lastActiveSceneView.Repaint();
            }
        }


        private void ShowBridge() {
            if (EditorApplication.isPlaying && roadSystem.editorPlayCamera != null) {
                var bridgePosition =
                    (bridges[bridgesIndex].pos - bridges[bridgesIndex].bridgeCounterpartNode.pos) * 0.5f +
                    bridges[bridgesIndex].bridgeCounterpartNode.pos;
                var bridgeLength = Vector3.Distance(bridges[bridgesIndex].pos,
                    bridges[bridgesIndex].bridgeCounterpartNode.pos);

                //Rotation:
                var cameraRotation =
                    Vector3.Cross(bridges[bridgesIndex].pos - bridges[bridgesIndex].bridgeCounterpartNode.pos,
                        Vector3.up);
                if (isCameraCustomRotated)
                    cameraRotation = customCameraRotation;
                else
                    cameraRotation = cameraRotation.normalized;

                //Calc offset:
                var bridgeOffset = cameraRotation * (bridgeLength * 0.5f * cameraZoomFactor);

                //Height offset:
                bridgeOffset.y = Mathf.Lerp(20f, 120f, bridgeLength * 0.001f) * cameraZoomFactor * cameraHeightOffset;

                roadSystem.editorPlayCamera.transform.position = bridgePosition + bridgeOffset;
                roadSystem.editorPlayCamera.transform.rotation =
                    Quaternion.LookRotation(bridgePosition - (bridgePosition + bridgeOffset));
            }
            else {
                SceneView.lastActiveSceneView.pivot = bridges[bridgesIndex].transform.position;
                SceneView.lastActiveSceneView.Repaint();
            }
        }


        private void DoHotKeyCheck() {
            var current = Event.current;

            if (current.type == EventType.KeyDown) {
                if (current.keyCode == KeyCode.K)
                    IncrementIntersection();
                else if (current.keyCode == KeyCode.L) IncrementBridge();
            }
        }

        #region "Vars"

        //Main target for this editor file:
        private RoadSystem roadSystem;

        //Serialized properties:
        private SerializedProperty isTempMultithreading;
        private SerializedProperty isTempSaveMeshAssets;

        //Editor only variables:
        private bool isInitialized;

        //	//Editor only camera variables:
        private RoadIntersection[] intersections;
        private int intersectionIndex;
        private SplineN[] bridges;
        private int bridgesIndex;
        private bool isBridgeInitialized;
        private bool isIntersectionInitialized;
        private bool isEditorCameraFlipped;
        private float cameraZoomFactor = 1f;
        private float cameraHeightOffset = 1f;
        private bool isCameraCustomRotated;
        private Vector3 customCameraRotation = new Vector3(0.5f, 0f, -0.5f);

        //Editor only graphic variables:
        private Texture2D loadButtonBG;
        private Texture2D loadButtonBGGlow;
        private Texture2D warningLabelBG;
        private GUIStyle warningLabelStyle;
        private GUIStyle loadButton;

        #endregion

    }
}
#endif