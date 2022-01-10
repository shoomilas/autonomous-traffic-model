#region "Imports"

using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

#endregion


namespace RoadArchitect {
    public class RoadSystem : MonoBehaviour {


        /// <summary> Adds a new road to this RoadSystem </summary>
        public GameObject AddRoad(bool _isForceSelected = false) {
            var roads = GetComponentsInChildren<Road>();
            var newRoadNumber = roads.Length + 1;

            //Road:
            var roadObj = new GameObject("Road" + newRoadNumber);

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(roadObj, "Created road");
#endif

            roadObj.transform.parent = transform;
            var road = roadObj.AddComponent<Road>();

            //Spline:
            var splineObj = new GameObject("Spline");
            splineObj.transform.parent = road.transform;
            road.spline = splineObj.AddComponent<SplineC>();
            road.spline.splineRoot = splineObj;
            road.spline.road = road;
            road.splineObject = splineObj;
            road.roadSystem = this;
            RootUtils.SetupUniqueIdentifier(ref road.UID);

            road.ResetTerrainHistory();

#if UNITY_EDITOR
            if (_isForceSelected) Selection.activeGameObject = roadObj;
#endif

            return roadObj;
        }


        /// <summary> Sets the editorPlayCamera to the first camera, if it is the only camera in this scene </summary>
        public void EditorCameraSetSingle() {
            if (editorPlayCamera == null) {
                var editorCams = FindObjectsOfType<Camera>();
                if (editorCams != null && editorCams.Length == 1) editorPlayCamera = editorCams[0];
            }
        }


        /// <summary> Updates all roads of this RoadSystem </summary>
        public void UpdateAllRoads() {
            var allRoadObjs = GetComponentsInChildren<Road>();
            var roadCount = allRoadObjs.Length;
            SplineC[] piggys = null;
            if (roadCount > 1) {
                piggys = new SplineC[roadCount];
                for (var i = 0; i < roadCount; i++) piggys[i] = allRoadObjs[i].spline;
            }

            var road = allRoadObjs[0];
            if (piggys != null && piggys.Length > 0) road.PiggyBacks = piggys;
            road.UpdateRoad();
        }


        //Workaround for submission rules:
        /// <summary> Writes isMultithreaded into roads of this system </summary>
        public void UpdateAllRoadsMultiThreadedOption(bool _isMultithreaded) {
            var roads = GetComponentsInChildren<Road>();
            var roadsCount = roads.Length;
            Road road = null;
            for (var i = 0; i < roadsCount; i++) {
                road = roads[i];
                if (road != null) road.isUsingMultithreading = _isMultithreaded;
            }
        }


        //Workaround for submission rules:
        /// <summary> Writes isSavingMeshes into roads of this system </summary>
        public void UpdateAllRoadsSavingMeshesOption(bool _isSavingMeshes) {
            var roads = GetComponentsInChildren<Road>();
            var roadsCount = roads.Length;
            Road road = null;
            for (var i = 0; i < roadsCount; i++) {
                road = roads[i];
                if (road != null) road.isSavingMeshes = _isSavingMeshes;
            }
        }

        #region "Vars"

        [FormerlySerializedAs("opt_bMultithreading")]
        public bool isMultithreaded = true;

        [FormerlySerializedAs("opt_bSaveMeshes")]
        public bool isSavingMeshes;

        [FormerlySerializedAs("opt_bAllowRoadUpdates")]
        public bool isAllowingRoadUpdates = true;

        public Camera editorPlayCamera;

        #endregion

    }
}