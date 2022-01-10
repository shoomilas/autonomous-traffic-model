#region "Imports"

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

#endregion


namespace RoadArchitect.EdgeObjects {
    [Serializable]
    public class EdgeObjectMaker {


        public EdgeObjectMaker Copy() {
            var EOM = new EdgeObjectMaker();

            EOM.edgeObjectString = edgeObjectString;

#if UNITY_EDITOR
            EOM.edgeObject = AssetDatabase.LoadAssetAtPath<GameObject>(edgeObjectString);
#endif

            EOM.isDefault = isDefault;

            EOM.isCombinedMesh = isCombinedMesh;
            EOM.isCombinedMeshCollider = isCombinedMeshCollider;
            EOM.subType = subType;
            EOM.meterSep = meterSep;
            EOM.isToggled = isToggled;
            EOM.isMatchingTerrain = isMatchingTerrain;

            EOM.isMaterialOverriden = isMaterialOverriden;
            EOM.edgeMaterial1 = edgeMaterial1;
            EOM.edgeMaterial2 = edgeMaterial2;

            EOM.masterObject = masterObject;
            EOM.edgeObjectLocations = edgeObjectLocations;
            EOM.edgeObjectRotations = edgeObjectRotations;
            EOM.node = node;
            EOM.startTime = startTime;
            EOM.endTime = endTime;
            EOM.startPos = startPos;
            EOM.endPos = endPos;
            EOM.singleOnlyBridgePercent = singleOnlyBridgePercent;
            EOM.isBridge = isBridge;

            EOM.horizontalSep = horizontalSep;
            EOM.horizontalCurve = new AnimationCurve();
            if (horizontalCurve != null && horizontalCurve.keys.Length > 0)
                for (var i = 0; i < horizontalCurve.keys.Length; i++)
                    EOM.horizontalCurve.AddKey(horizontalCurve.keys[i]);

            EOM.verticalRaise = verticalRaise;
            EOM.verticalCurve = new AnimationCurve();
            if (verticalCurve != null && verticalCurve.keys.Length > 0)
                for (var index = 0; index < verticalCurve.keys.Length; index++)
                    EOM.verticalCurve.AddKey(verticalCurve.keys[index]);

            EOM.customRotation = customRotation;
            EOM.isRotationAligning = isRotationAligning;
            EOM.isXRotationLocked = isXRotationLocked;
            EOM.isYRotationLocked = isYRotationLocked;
            EOM.isZRotationLocked = isZRotationLocked;
            EOM.isOncomingRotation = isOncomingRotation;
            EOM.isStatic = isStatic;
            EOM.isSingle = isSingle;
            EOM.singlePosition = singlePosition;

            EOM.isStartMatchRoadDefinition = isStartMatchRoadDefinition;
            EOM.startMatchRoadDef = startMatchRoadDef;

            RootUtils.SetupUniqueIdentifier(ref EOM.UID);

            EOM.objectName = objectName;
            EOM.thumbString = thumbString;
            EOM.desc = desc;
            EOM.displayName = displayName;

            return EOM;
        }


        public void UpdatePositions() {
            startPos = node.spline.GetSplineValue(startTime);
            endPos = node.spline.GetSplineValue(endTime);
        }


        public void SetDefaultTimes(bool _isEndPoint, float _time, float _timeNext, int _idOnSpline, float _dist) {
            if (!_isEndPoint) {
                startTime = _time;
                endTime = _timeNext;
            }
            else {
                if (_idOnSpline < 2) {
                    startTime = _time;
                    endTime = _timeNext;
                }
                else {
                    startTime = _time;
                    endTime = _time - 125f / _dist;
                }
            }
        }


        public class EdgeObjectEditorMaker {


            /// <summary> Setup using _EOM </summary>
            public void Setup(EdgeObjectMaker _EOM) {
                edgeObject = _EOM.edgeObject;
                isCombinedMesh = _EOM.isCombinedMesh;
                isCombinedMeshCollider = _EOM.isCombinedMeshCollider;
                meterSep = _EOM.meterSep;
                isToggled = _EOM.isToggled;

                isMaterialOverriden = _EOM.isMaterialOverriden;
                edgeMaterial1 = _EOM.edgeMaterial1;
                edgeMaterial2 = _EOM.edgeMaterial2;

                // Horizontal
                horizontalSep = _EOM.horizontalSep;
                horizontalCurve = _EOM.horizontalCurve;

                // Vertical
                verticalRaise = _EOM.verticalRaise;
                verticalCurve = _EOM.verticalCurve;

                isMatchingTerrain = _EOM.isMatchingTerrain;

                // Rotation
                customRotation = _EOM.customRotation;
                isRotationAligning = _EOM.isRotationAligning;
                isXRotationLocked = _EOM.isXRotationLocked;
                isYRotationLocked = _EOM.isYRotationLocked;
                isZRotationLocked = _EOM.isZRotationLocked;
                isOncomingRotation = _EOM.isOncomingRotation;

                customScale = _EOM.customScale;
                isStatic = _EOM.isStatic;

                // Is it Single and if yes Position
                isSingle = _EOM.isSingle;
                singlePosition = _EOM.singlePosition;

                // Name of EdgeObject
                objectName = _EOM.objectName;

                // Start and EndTime of EdgeObject
                startTime = _EOM.startTime;
                endTime = _EOM.endTime;

                singleOnlyBridgePercent = _EOM.singleOnlyBridgePercent;
                isStartMatchingRoadDefinition = _EOM.isStartMatchRoadDefinition;
                startMatchRoadDef = _EOM.startMatchRoadDef;
            }


            /// <summary> Copy relevant attributes to _EOM </summary>
            public void LoadTo(EdgeObjectMaker _EOM) {
                _EOM.edgeObject = edgeObject;
                _EOM.isMaterialOverriden = isMaterialOverriden;
                _EOM.edgeMaterial1 = edgeMaterial1;
                _EOM.edgeMaterial2 = edgeMaterial2;

                _EOM.isCombinedMesh = isCombinedMesh;
                _EOM.isCombinedMeshCollider = isCombinedMeshCollider;
                _EOM.subType = subType;
                _EOM.meterSep = meterSep;
                _EOM.isToggled = isToggled;

                _EOM.horizontalSep = horizontalSep;
                _EOM.horizontalCurve = horizontalCurve;
                _EOM.verticalRaise = verticalRaise;
                _EOM.verticalCurve = verticalCurve;
                _EOM.isMatchingTerrain = isMatchingTerrain;

                _EOM.customRotation = customRotation;
                _EOM.isRotationAligning = isRotationAligning;
                _EOM.isXRotationLocked = isXRotationLocked;
                _EOM.isYRotationLocked = isYRotationLocked;
                _EOM.isZRotationLocked = isZRotationLocked;
                _EOM.customScale = customScale;
                _EOM.isOncomingRotation = isOncomingRotation;
                _EOM.isStatic = isStatic;
                _EOM.isSingle = isSingle;


                _EOM.startTime = startTime;
                _EOM.endTime = endTime;


                _EOM.singlePosition = singlePosition;


                _EOM.objectName = objectName;
                _EOM.singleOnlyBridgePercent = singleOnlyBridgePercent;
                _EOM.isStartMatchRoadDefinition = isStartMatchingRoadDefinition;
                _EOM.startMatchRoadDef = startMatchRoadDef;
            }


            public bool IsEqual(EdgeObjectMaker _EOM) {
                if (_EOM.edgeObject != edgeObject) return false;
                if (_EOM.isMaterialOverriden != isMaterialOverriden) return false;
                if (_EOM.edgeMaterial1 != edgeMaterial1) return false;
                if (_EOM.edgeMaterial2 != edgeMaterial2) return false;

                if (_EOM.isCombinedMesh != isCombinedMesh) return false;
                if (_EOM.isCombinedMeshCollider != isCombinedMeshCollider) return false;
                if (_EOM.subType != subType) return false;
                if (!RootUtils.IsApproximately(_EOM.meterSep, meterSep, 0.001f)) return false;
                //if(EOM.isToggled != isToggled)
                //{
                //  return false;
                //}

                if (!RootUtils.IsApproximately(_EOM.horizontalSep, horizontalSep, 0.001f)) return false;
                if (_EOM.horizontalCurve != horizontalCurve) return false;
                if (!RootUtils.IsApproximately(_EOM.verticalRaise, verticalRaise, 0.001f)) return false;
                if (_EOM.verticalCurve != verticalCurve) return false;
                if (_EOM.isMatchingTerrain != isMatchingTerrain) return false;

                if (_EOM.customRotation != customRotation) return false;
                if (_EOM.isRotationAligning != isRotationAligning) return false;
                if (_EOM.isXRotationLocked != isXRotationLocked) return false;
                if (_EOM.isYRotationLocked != isYRotationLocked) return false;
                if (_EOM.isZRotationLocked != isZRotationLocked) return false;
                if (_EOM.customScale != customScale) return false;
                if (_EOM.isOncomingRotation != isOncomingRotation) return false;
                if (_EOM.isStatic != isStatic) return false;
                if (_EOM.isSingle != isSingle) return false;

                if (!RootUtils.IsApproximately(_EOM.singlePosition, singlePosition, 0.001f)) return false;
                if (!RootUtils.IsApproximately(_EOM.startTime, startTime, 0.001f)) return false;
                if (!RootUtils.IsApproximately(_EOM.endTime, endTime, 0.001f)) return false;
                if (_EOM.objectName != objectName) return false;
                if (!RootUtils.IsApproximately(_EOM.singleOnlyBridgePercent, singleOnlyBridgePercent, 0.001f))
                    return false;
                if (_EOM.isStartMatchRoadDefinition != isStartMatchingRoadDefinition) return false;
                if (!RootUtils.IsApproximately(_EOM.startMatchRoadDef, startMatchRoadDef, 0.001f)) return false;

                return true;
            }

            #region "Vars"

            [FormerlySerializedAs("EdgeObject")] public GameObject edgeObject;

            // Should we combine the Mesh?
            [FormerlySerializedAs("bCombineMesh")] public bool isCombinedMesh;

            // Should it also combine the Colliders
            [FormerlySerializedAs("bCombineMeshCollider")]
            public bool isCombinedMeshCollider;

            // Seems to be a List with all Locations for the EdgeObjects
            [FormerlySerializedAs("EdgeObjectLocations")]
            public List<Vector3> edgeObjectLocations;

            // Seems to be a List with all Rotations for the EdgeObjects
            [FormerlySerializedAs("EdgeObjectRotations")]
            public List<Vector3> edgeObjectRotations;

            [FormerlySerializedAs("SubType")] public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;

            // Sounds like Speration
            [FormerlySerializedAs("MeterSep")] public float meterSep = 5f;

            // A Toggle for?
            [FormerlySerializedAs("bToggle")] public bool isToggled;

            [FormerlySerializedAs("bIsBridge")] public bool isBridge = false;

            [FormerlySerializedAs("bIsGSD")] public bool isDefault = false;

            // Materials of EdgeObject
            [FormerlySerializedAs("bMaterialOverride")]
            public bool isMaterialOverriden;

            [FormerlySerializedAs("EdgeMaterial1")]
            public Material edgeMaterial1;

            [FormerlySerializedAs("EdgeMaterial2")]
            public Material edgeMaterial2;

            //Horizontal offsets:
            [FormerlySerializedAs("HorizontalSep")]
            public float horizontalSep = 5f;

            [FormerlySerializedAs("HorizontalCurve")]
            public AnimationCurve horizontalCurve;

            //Vertical offsets:
            [FormerlySerializedAs("VerticalRaise")]
            public float verticalRaise;

            [FormerlySerializedAs("VerticalCurve")]
            public AnimationCurve verticalCurve;

            [FormerlySerializedAs("bOncomingRotation")]
            public bool isOncomingRotation = true;

            [FormerlySerializedAs("bStatic")] public bool isStatic = true;

            [FormerlySerializedAs("bMatchTerrain")]
            public bool isMatchingTerrain = true;

            [FormerlySerializedAs("CustomScale")] public Vector3 customScale = new Vector3(1f, 1f, 1f);

            [FormerlySerializedAs("CustomRotation")]
            public Vector3 customRotation;

            public bool isRotationAligning = true;
            public bool isXRotationLocked = true;
            public bool isYRotationLocked;
            public bool isZRotationLocked;

            // Start and EndTime
            [FormerlySerializedAs("StartTime")] public float startTime;

            [FormerlySerializedAs("EndTime")] public float endTime = 1f;

            [FormerlySerializedAs("SingleOnlyBridgePercent")]
            public float singleOnlyBridgePercent;

            [FormerlySerializedAs("bSingle")] public bool isSingle;

            [FormerlySerializedAs("SinglePosition")]
            public float singlePosition;

            [FormerlySerializedAs("tName")] public string objectName;

            [FormerlySerializedAs("bStartMatchRoadDefinition")]
            public bool isStartMatchingRoadDefinition;

            [FormerlySerializedAs("StartMatchRoadDef")]
            public float startMatchRoadDef;

            #endregion

        }

        #region "Vars"

        [FormerlySerializedAs("bNeedsUpdate")] public bool isRequiringUpdate;

        public string UID = "";

        [FormerlySerializedAs("tNode")] public SplineN node;

        [FormerlySerializedAs("bIsGSD")] public bool isDefault;

        [FormerlySerializedAs("EdgeObject")] public GameObject edgeObject;

        [FormerlySerializedAs("EdgeObjectString")]
        public string edgeObjectString = "";

        [FormerlySerializedAs("bMaterialOverride")]
        public bool isMaterialOverriden;

        [FormerlySerializedAs("EdgeMaterial1")]
        public Material edgeMaterial1;

        [FormerlySerializedAs("EdgeMaterial2")]
        public Material edgeMaterial2;

        [FormerlySerializedAs("EdgeMaterial1String")]
        public string edgeMaterial1String;

        [FormerlySerializedAs("EdgeMaterial2String")]
        public string edgeMaterial2String;

        [FormerlySerializedAs("bMatchTerrain")]
        public bool isMatchingTerrain = true;

        //Temp editor buffers:
        [FormerlySerializedAs("bEdgeSignLabelInit")]
        public bool isEdgeSignLabelInit;

        [FormerlySerializedAs("bEdgeSignLabel")]
        public bool isEdgeSignLabel;

        [FormerlySerializedAs("EdgeSignLabel")]
        public string edgeSignLabel = "";

        [FormerlySerializedAs("bCombineMesh")] public bool isCombinedMesh;

        [FormerlySerializedAs("bCombineMeshCollider")]
        public bool isCombinedMeshCollider;

        [FormerlySerializedAs("MasterObj")] public GameObject masterObject;

        [FormerlySerializedAs("EdgeObjectLocations")]
        public List<Vector3> edgeObjectLocations;

        [FormerlySerializedAs("EdgeObjectRotations")]
        public List<Vector3> edgeObjectRotations;

        [FormerlySerializedAs("EdgeObjects")] public List<GameObject> edgeObjects;

        [FormerlySerializedAs("SubType")] public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;

        [FormerlySerializedAs("MeterSep")] public float meterSep = 5f;

        [FormerlySerializedAs("bToggle")] public bool isToggled;

        [FormerlySerializedAs("bIsBridge")] public bool isBridge;


        #region "Horizontal offsets"

        [FormerlySerializedAs("HorizontalSep")]
        public float horizontalSep = 5f;

        [FormerlySerializedAs("HorizontalCurve")]
        public AnimationCurve horizontalCurve;

        #endregion


        #region "Vertical offsets"

        [FormerlySerializedAs("VerticalRaise")]
        public float verticalRaise;

        [FormerlySerializedAs("VerticalCurve")]
        public AnimationCurve verticalCurve;

        #endregion


        // Custom Rotation
        [FormerlySerializedAs("CustomRotation")]
        public Vector3 customRotation;

        public bool isRotationAligning = true;
        public bool isXRotationLocked = true;
        public bool isYRotationLocked;
        public bool isZRotationLocked;

        [FormerlySerializedAs("bOncomingRotation")]
        public bool isOncomingRotation = true;

        // EdgeObject is static
        [FormerlySerializedAs("bStatic")] public bool isStatic = true;

        // The CustomScale of the EdgeObject
        [FormerlySerializedAs("CustomScale")] public Vector3 customScale = new Vector3(1f, 1f, 1f);

        // Start and EndTime
        [FormerlySerializedAs("StartTime")] public float startTime;

        [FormerlySerializedAs("EndTime")] public float endTime = 1f;

        [FormerlySerializedAs("SingleOnlyBridgePercent")]
        public float singleOnlyBridgePercent;

        [FormerlySerializedAs("StartPos")] public Vector3 startPos;

        [FormerlySerializedAs("EndPos")] public Vector3 endPos;

        [FormerlySerializedAs("bSingle")] public bool isSingle;

        // Should it be only on a single position
        [FormerlySerializedAs("SinglePosition")]
        public float singlePosition;

        [FormerlySerializedAs("bStartMatchRoadDefinition")]
        public bool isStartMatchRoadDefinition;

        [FormerlySerializedAs("StartMatchRoadDef")]
        public float startMatchRoadDef;

        // EdgeObjectName
        [FormerlySerializedAs("tName")] public string objectName = "EdgeObject";

        [FormerlySerializedAs("ThumbString")] public string thumbString = "";

        [FormerlySerializedAs("Desc")] public string desc = "";

        [FormerlySerializedAs("DisplayName")] public string displayName = "";

        [FormerlySerializedAs("EM")] public EdgeObjectEditorMaker edgeMaker;

        #endregion


        #region "Library"

        /// <summary> Saves object as xml into Library folder. Auto prefixed with EOM and extension .rao </summary>
        public void SaveToLibrary(string _fileName = "", bool _isDefault = false) {
            var EOLM = new EdgeObjectLibraryMaker();
            EOLM.Setup(this);
            RootUtils.CheckCreateSpecialLibraryDirs();
            var libraryPath = RootUtils.GetDirLibrary();
            var filePath = Path.Combine(libraryPath, "EOM" + objectName + ".rao");
            if (_fileName.Length > 0) {
                if (_isDefault)
                    filePath = Path.Combine(Path.Combine(libraryPath, "EdgeObjects"), "EOM" + _fileName + ".rao");
                else
                    filePath = Path.Combine(libraryPath, "EOM" + _fileName + ".rao");
            }

            RootUtils.CreateXML<EdgeObjectLibraryMaker>(ref filePath, EOLM);
        }


        /// <summary> Loads _fileName from Library folder. Auto prefixed with EOM and extension .rao </summary>
        public void LoadFromLibrary(string _fileName, bool _isQuickAdd = false) {
            RootUtils.CheckCreateSpecialLibraryDirs();
            var libraryPath = RootUtils.GetDirLibrary();
            var filePath = Path.Combine(libraryPath, "EOM" + _fileName + ".rao");
            if (_isQuickAdd)
                filePath = Path.Combine(Path.Combine(libraryPath, "EdgeObjects"), "EOM" + _fileName + ".rao");
            var ELM = RootUtils.LoadXML<EdgeObjectLibraryMaker>(ref filePath);
            ELM.LoadTo(this);
            isRequiringUpdate = true;
        }


        public void LoadFromLibraryWizard(string _fileName) {
            RootUtils.CheckCreateSpecialLibraryDirs();
            var libraryPath = RootUtils.GetDirLibrary();
            var filePath = Path.Combine(Path.Combine(libraryPath, "W"), _fileName + ".rao");
            var ELM = RootUtils.LoadXML<EdgeObjectLibraryMaker>(ref filePath);
            ELM.LoadTo(this);
            isRequiringUpdate = true;
        }


        public string ConvertToString() {
            var EOLM = new EdgeObjectLibraryMaker();
            EOLM.Setup(this);
            return RootUtils.GetString<EdgeObjectLibraryMaker>(EOLM);
        }


        /// <summary> Loads _EOLM into this EdgeObjectMaker </summary>
        public void LoadFromLibraryBulk(ref EdgeObjectLibraryMaker _EOLM) {
            _EOLM.LoadTo(this);
        }


        public static EdgeObjectLibraryMaker ELMFromData(string _data) {
            try {
                var ELM = RootUtils.LoadData<EdgeObjectLibraryMaker>(ref _data);
                return ELM;
            }
            catch {
                return null;
            }
        }


        /// <summary> Stores .rao files which begin with EOM from Library folder into _names and _paths </summary>
        public static void GetLibraryFiles(out string[] _names, out string[] _paths, bool _isDefault = false) {
            _names = null;
            _paths = null;
            DirectoryInfo info;
            var libraryPath = RootUtils.GetDirLibrary();
            if (_isDefault)
                info = new DirectoryInfo(Path.Combine(libraryPath, "EdgeObjects"));
            else
                info = new DirectoryInfo(libraryPath);

            var fileInfos = info.GetFiles();
            var count = 0;


            foreach (var tInfo in fileInfos)
                if (tInfo.Name.Contains("EOM") && tInfo.Extension.ToLower().Contains("rao"))
                    count += 1;

            _names = new string[count];
            _paths = new string[count];
            count = 0;
            foreach (var fileInfo in fileInfos)
                if (fileInfo.Name.Contains("EOM") && fileInfo.Extension.ToLower().Contains("rao")) {
                    _names[count] = fileInfo.Name.Replace(".rao", "").Replace("EOM", "");
                    _paths[count] = fileInfo.FullName;
                    count += 1;
                }
        }


        /// <summary> Saves _mesh as an asset into /Mesh/Generated/CombinedEdgeObj folder beside the /Asset folder </summary>
        private void SaveMesh(Mesh _mesh, bool _isCollider) {
            if (!node.spline.road.isSavingMeshes) return;

            string sceneName;
            sceneName = SceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");

            var folderName = Path.Combine(RoadEditorUtility.GetBasePath(), "Mesh");
            folderName = Path.Combine(folderName, "Generated");
            folderName = Path.Combine(folderName, "CombinedEdgeObj");

            var roadName = node.spline.road.transform.name;
            var fileName = sceneName + "-" + roadName + "-" + objectName;
            var finalName = Path.Combine(folderName, fileName + ".asset");
            if (_isCollider) finalName = Path.Combine(folderName, fileName + "-collider.asset");

            var path = Path.GetDirectoryName(Application.dataPath);
            path = Path.Combine(path, folderName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);


#if UNITY_EDITOR
            // Unity works with forward slash so we convert
            // If you want to implement your own Asset creation and saving you should just use finalName
            finalName = finalName.Replace(Path.DirectorySeparatorChar, '/');
            finalName = finalName.Replace(Path.AltDirectorySeparatorChar, '/');
            AssetDatabase.CreateAsset(_mesh, finalName);
            AssetDatabase.SaveAssets();
#endif
        }


        #region "Library object"

        [Serializable]
        public class EdgeObjectLibraryMaker {


            /// <summary> Setup using _EOM </summary>
            public void Setup(EdgeObjectMaker _EOM) {
                edgeObjectString = _EOM.edgeObjectString;
                isCombinedMesh = _EOM.isCombinedMesh;
                isCombinedMeshCollider = _EOM.isCombinedMeshCollider;
                //SignPlacementSubTypeEnum SubType = _EOM.SubType;
                meterSep = _EOM.meterSep;
                isToggled = _EOM.isToggled;
                isDefault = _EOM.isDefault;

                isMaterialOverriden = _EOM.isMaterialOverriden;
                edgeMaterial1String = _EOM.edgeMaterial1String;
                edgeMaterial2String = _EOM.edgeMaterial2String;

                horizontalSep = _EOM.horizontalSep;
                horizontalCurve = _EOM.horizontalCurve;
                verticalRaise = _EOM.verticalRaise;
                verticalCurve = _EOM.verticalCurve;
                isMatchingTerrain = _EOM.isMatchingTerrain;

                customRotation = _EOM.customRotation;
                isRotationAligning = _EOM.isRotationAligning;
                isXRotationLocked = _EOM.isXRotationLocked;
                isYRotationLocked = _EOM.isYRotationLocked;
                isZRotationLocked = _EOM.isZRotationLocked;
                isOncomingRotation = _EOM.isOncomingRotation;
                isStatic = _EOM.isStatic;
                isSingle = _EOM.isSingle;
                singlePosition = _EOM.singlePosition;
                objectName = _EOM.objectName;
                singleOnlyBridgePercent = _EOM.singleOnlyBridgePercent;
                isStartMatchingRoadDefinition = _EOM.isStartMatchRoadDefinition;
                startMatchRoadDef = _EOM.startMatchRoadDef;
                thumbString = _EOM.thumbString;
                desc = _EOM.desc;
                isBridge = _EOM.isBridge;
                displayName = _EOM.displayName;
            }


            /// <summary> Copy relevant attributes to _EOM </summary>
            public void LoadTo(EdgeObjectMaker _EOM) {
                _EOM.edgeObjectString = edgeObjectString;
#if UNITY_EDITOR
                _EOM.edgeObject = AssetDatabase.LoadAssetAtPath<GameObject>(edgeObjectString);

                if (edgeMaterial1String.Length > 0)
                    _EOM.edgeMaterial1 = AssetDatabase.LoadAssetAtPath<Material>(edgeMaterial1String);
                if (edgeMaterial2String.Length > 0)
                    _EOM.edgeMaterial2 = AssetDatabase.LoadAssetAtPath<Material>(edgeMaterial2String);
#endif

                _EOM.isMaterialOverriden = isMaterialOverriden;

                _EOM.isCombinedMesh = isCombinedMesh;
                _EOM.isCombinedMeshCollider = isCombinedMeshCollider;
                _EOM.subType = subType;
                _EOM.meterSep = meterSep;
                _EOM.isToggled = isToggled;
                _EOM.isDefault = isDefault;

                _EOM.horizontalSep = horizontalSep;
                _EOM.horizontalCurve = horizontalCurve;
                _EOM.verticalRaise = verticalRaise;
                _EOM.verticalCurve = verticalCurve;
                _EOM.isMatchingTerrain = isMatchingTerrain;

                _EOM.customRotation = customRotation;
                _EOM.isRotationAligning = isRotationAligning;
                _EOM.isXRotationLocked = isXRotationLocked;
                _EOM.isYRotationLocked = isYRotationLocked;
                _EOM.isZRotationLocked = isZRotationLocked;
                _EOM.isOncomingRotation = isOncomingRotation;
                _EOM.isStatic = isStatic;
                _EOM.isSingle = isSingle;
                _EOM.singlePosition = singlePosition;
                _EOM.objectName = objectName;
                _EOM.singleOnlyBridgePercent = singleOnlyBridgePercent;
                _EOM.isStartMatchRoadDefinition = isStartMatchingRoadDefinition;
                _EOM.startMatchRoadDef = startMatchRoadDef;
                _EOM.thumbString = thumbString;
                _EOM.desc = desc;
                _EOM.isBridge = isBridge;
                _EOM.displayName = displayName;
            }

            #region "Vars"

            [FormerlySerializedAs("EdgeObjectString")]
            public string edgeObjectString = "";

            [FormerlySerializedAs("bCombineMesh")] public bool isCombinedMesh;

            [FormerlySerializedAs("bCombineMeshCollider")]
            public bool isCombinedMeshCollider;

            [FormerlySerializedAs("EdgeObjectLocations")]
            public List<Vector3> edgeObjectLocations;

            [FormerlySerializedAs("EdgeObjectRotations")]
            public List<Vector3> edgeObjectRotations;

            [FormerlySerializedAs("SubType")] public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;

            [FormerlySerializedAs("MeterSep")] public float meterSep = 5f;

            [FormerlySerializedAs("bToggle")] public bool isToggled;

            [FormerlySerializedAs("bIsBridge")] public bool isBridge;

            [FormerlySerializedAs("bIsGSD")] public bool isDefault;

            [FormerlySerializedAs("bOncomingRotation")]
            public bool isOncomingRotation = true;

            [FormerlySerializedAs("bStatic")] public bool isStatic = true;

            [FormerlySerializedAs("bMatchTerrain")]
            public bool isMatchingTerrain = true;

            [FormerlySerializedAs("bSingle")] public bool isSingle;

            [FormerlySerializedAs("bMaterialOverride")]
            public bool isMaterialOverriden;

            [FormerlySerializedAs("EdgeMaterial1String")]
            public string edgeMaterial1String = "";

            [FormerlySerializedAs("EdgeMaterial2String")]
            public string edgeMaterial2String = "";

            //Horizontal offsets:
            [FormerlySerializedAs("HorizontalSep")]
            public float horizontalSep = 5f;

            [FormerlySerializedAs("HorizontalCurve")]
            public AnimationCurve horizontalCurve;

            //Vertical offsets:
            [FormerlySerializedAs("VerticalRaise")]
            public float verticalRaise;

            [FormerlySerializedAs("VerticalCurve")]
            public AnimationCurve verticalCurve;

            [FormerlySerializedAs("CustomRotation")]
            public Vector3 customRotation;

            public bool isRotationAligning = true;
            public bool isXRotationLocked = true;
            public bool isYRotationLocked;
            public bool isZRotationLocked;

            [FormerlySerializedAs("StartTime")] public float startTime;

            [FormerlySerializedAs("EndTime")] public float endTime = 1f;

            [FormerlySerializedAs("SingleOnlyBridgePercent")]
            public float singleOnlyBridgePercent;

            [FormerlySerializedAs("SinglePosition")]
            public float singlePosition;

            [FormerlySerializedAs("bStartMatchRoadDefinition")]
            public bool isStartMatchingRoadDefinition;

            [FormerlySerializedAs("StartMatchRoadDef")]
            public float startMatchRoadDef;

            [FormerlySerializedAs("tName")] public string objectName = "EdgeObject";

            [FormerlySerializedAs("ThumbString")] public string thumbString = "";

            [FormerlySerializedAs("Desc")] public string desc = "";

            [FormerlySerializedAs("DisplayName")] public string displayName = "";

            #endregion

        }

        #endregion

        #endregion


        #region "Setup and processing"

        public void Setup(bool _isCollecting = true) {
            var errorObjs = new List<GameObject>();
            try {
                SetupDo(_isCollecting, ref errorObjs);
            }
            catch (Exception exception) {
                if (errorObjs != null && errorObjs.Count > 0) {
                    var objCount = errorObjs.Count;
                    for (var index = 0; index < objCount; index++)
                        if (errorObjs[index] != null)
                            Object.DestroyImmediate(errorObjs[index]);
                    throw exception;
                }
            }
        }


        private void SetupDo(bool _isCollecting, ref List<GameObject> _errorObjs) {
            if (edgeObjects == null) edgeObjects = new List<GameObject>();
            if (horizontalCurve == null) {
                horizontalCurve = new AnimationCurve();
                horizontalCurve.AddKey(0f, 1f);
                horizontalCurve.AddKey(1f, 1f);
            }

            if (verticalCurve == null) {
                verticalCurve = new AnimationCurve();
                verticalCurve.AddKey(0f, 1f);
                verticalCurve.AddKey(1f, 1f);
            }

            RootUtils.SetupUniqueIdentifier(ref UID);

            SetupLocations();

            edgeObjectString = RootUtils.GetPrefabString(edgeObject);

#if UNITY_EDITOR
            if (edgeMaterial1 != null) edgeMaterial1String = AssetDatabase.GetAssetPath(edgeMaterial1);
            if (edgeMaterial2 != null) edgeMaterial2String = AssetDatabase.GetAssetPath(edgeMaterial2);
#endif

            edgeObjects = new List<GameObject>();

            var xRot = default(Quaternion);
            xRot = Quaternion.identity;
            xRot.eulerAngles = customRotation;
            var lCount = edgeObjectLocations.Count;
            //Quaternion OrigRot = Quaternion.identity;

            Material[] tMats = null;
            GameObject tObj = null;

            if (edgeObject != null) {
                var mObj = new GameObject(edgeObject.name);
                masterObject = mObj;
                _errorObjs.Add(masterObject);
                mObj.transform.position = node.transform.position;
                mObj.transform.parent = node.transform;
                mObj.name = objectName;
                var OrigMR = edgeObject.GetComponent<MeshRenderer>();
                for (var j = 0; j < lCount; j++) {
                    if (edgeObjectRotations[j] == default) {
#if UNITY_EDITOR
                        // Instantiate prefab instead of object
                        tObj = (GameObject)PrefabUtility.InstantiatePrefab(edgeObject);
#else
                        // Line to instantiate the object instead of an prefab
                        tObj = GameObject.Instantiate(edgeObject);
#endif
                        _errorObjs.Add(tObj);
                        tObj.transform.position = edgeObjectLocations[j];
                    }
                    else {
#if UNITY_EDITOR
                        // Instantiate prefab instead of object
                        tObj = (GameObject)PrefabUtility.InstantiatePrefab(edgeObject);
#else
                        // Line to instantiate the object instead of an prefab
                        tObj = GameObject.Instantiate(edgeObject);
#endif
                        tObj.transform.position = edgeObjectLocations[j];
                        if (isRotationAligning) {
                            tObj.transform.rotation = Quaternion.LookRotation(edgeObjectRotations[j]);
                        }
                        else {
                            var rotation = Quaternion.LookRotation(edgeObjectRotations[j]);
                            var eulerRotation = rotation.eulerAngles;

                            if (isXRotationLocked) eulerRotation.x = 0;
                            if (isYRotationLocked) eulerRotation.y = 0;
                            if (isZRotationLocked) eulerRotation.z = 0;

                            rotation = Quaternion.Euler(eulerRotation);
                            tObj.transform.rotation = rotation;
                        }

                        _errorObjs.Add(tObj);
                    }

                    //OrigRot = tObj.transform.rotation;
                    tObj.transform.rotation *= xRot;
                    tObj.transform.localScale = customScale;

                    // Turn object by 180 for other side of road
                    if (isOncomingRotation && subType == SignPlacementSubTypeEnum.Left) {
                        var tRot = new Quaternion(0f, 0f, 0f, 0f);
                        tRot = Quaternion.identity;
                        tRot.eulerAngles = new Vector3(0f, 180f, 0f);
                        tObj.transform.rotation *= tRot;
                    }

                    tObj.isStatic = isStatic;
                    tObj.transform.parent = mObj.transform;
                    edgeObjects.Add(tObj);

                    var NewMR = tObj.GetComponent<MeshRenderer>();
                    if (NewMR == null) NewMR = tObj.AddComponent<MeshRenderer>();

                    if (!isMaterialOverriden && OrigMR != null && OrigMR.sharedMaterials.Length > 0 && NewMR != null) {
                        NewMR.sharedMaterials = OrigMR.sharedMaterials;
                    }
                    else {
                        if (edgeMaterial1 != null) {
                            if (edgeMaterial2 != null) {
                                tMats = new Material[2];
                                tMats[0] = edgeMaterial1;
                                tMats[1] = edgeMaterial2;
                            }
                            else {
                                tMats = new Material[1];
                                tMats[0] = edgeMaterial1;
                            }

                            NewMR.sharedMaterials = tMats;
                        }
                    }
                }
            }

            lCount = edgeObjects.Count;
            if (lCount > 1 && isCombinedMesh) {
                Material[] tMat = null;
                Mesh xMeshBuffer = null;
                xMeshBuffer = edgeObject.GetComponent<MeshFilter>().sharedMesh;
                if (isMaterialOverriden) {
                    if (edgeMaterial1 != null) {
                        if (edgeMaterial2 != null) {
                            tMat = new Material[2];
                            tMat[0] = edgeMaterial1;
                            tMat[1] = edgeMaterial2;
                        }
                        else {
                            tMat = new Material[1];
                            tMat[0] = edgeMaterial1;
                        }
                    }
                }
                else {
                    tMat = edgeObject.GetComponent<MeshRenderer>().sharedMaterials;
                }

                var kVerts = xMeshBuffer.vertices;
                var kTris = xMeshBuffer.triangles;
                var kUV = xMeshBuffer.uv;
                var OrigMVL = kVerts.Length;
                var OrigTriCount = xMeshBuffer.triangles.Length;

                var hVerts = new List<Vector3[]>();
                var hTris = new List<int[]>();
                var hUV = new List<Vector2[]>();


                Transform tTrans;
                for (var j = 0; j < lCount; j++) {
                    tTrans = edgeObjects[j].transform;
                    hVerts.Add(new Vector3[OrigMVL]);
                    hTris.Add(new int[OrigTriCount]);
                    hUV.Add(new Vector2[OrigMVL]);

                    //Vertex copy:
                    Array.Copy(kVerts, hVerts[j], OrigMVL);
                    //Tri copy:
                    Array.Copy(kTris, hTris[j], OrigTriCount);
                    //UV copy:
                    Array.Copy(kUV, hUV[j], OrigMVL);

                    var tVect = default(Vector3);
                    for (var index = 0; index < OrigMVL; index++) {
                        tVect = hVerts[j][index];
                        hVerts[j][index] = tTrans.rotation * tVect;
                        hVerts[j][index] += tTrans.localPosition;
                    }
                }

                var xObj = new GameObject(objectName);
                var MR = xObj.GetComponent<MeshRenderer>();
                if (MR == null) MR = xObj.AddComponent<MeshRenderer>();
                xObj.isStatic = isStatic;
                xObj.transform.parent = masterObject.transform;
                _errorObjs.Add(xObj);
                xObj.transform.name = xObj.transform.name + "Combined";
                xObj.transform.name = xObj.transform.name.Replace("(Clone)", "");
                var MF = xObj.GetComponent<MeshFilter>();
                if (MF == null) MF = xObj.AddComponent<MeshFilter>();
                MF.sharedMesh = CombineMeshes(ref hVerts, ref hTris, ref hUV, OrigMVL, OrigTriCount);
                var MC = xObj.GetComponent<MeshCollider>();
                if (MC == null) MC = xObj.AddComponent<MeshCollider>();
                xObj.transform.position = node.transform.position;
                xObj.transform.rotation = Quaternion.identity;

                for (var j = lCount - 1; j >= 0; j--) Object.DestroyImmediate(edgeObjects[j]);
                for (var j = 0; j < edgeObjects.Count; j++) edgeObjects[j] = null;
                edgeObjects.RemoveRange(0, lCount);
                edgeObjects.Add(xObj);

                if (tMat != null && MR != null) MR.sharedMaterials = tMat;

                var BC = xObj.GetComponent<BoxCollider>();
                if (BC != null) Object.DestroyImmediate(BC);
                var cCount = xObj.transform.childCount;
                var spamc = 0;
                while (cCount > 0 && spamc < 10) {
                    Object.DestroyImmediate(xObj.transform.GetChild(0).gameObject);
                    cCount = xObj.transform.childCount;
                    spamc += 1;
                }

                if (isCombinedMeshCollider) {
                    if (MC == null) MC = xObj.AddComponent<MeshCollider>();
                    MC.sharedMesh = MF.sharedMesh;
                }
                else {
                    if (MC != null) {
                        Object.DestroyImmediate(MC);
                        MC = null;
                    }
                }

                if (node.spline.road.isSavingMeshes && MF != null && isCombinedMesh) {
                    SaveMesh(MF.sharedMesh, false);
                    if (MC != null)
                        if (MF.sharedMesh != MC.sharedMesh)
                            SaveMesh(MC.sharedMesh, true);
                }

                //tMesh = null;
            }

            //Zero these out, as they are not needed anymore:
            if (edgeObjectLocations != null) {
                edgeObjectLocations.Clear();
                edgeObjectLocations = null;
            }

            if (edgeObjectRotations != null) {
                edgeObjectRotations.Clear();
                edgeObjectRotations = null;
            }

            if (_isCollecting) node.spline.road.isTriggeringGC = true;
        }


        /// <summary> Setup objects positions and rotations </summary>
        private void SetupLocations() {
            var origHeight = 0f;
            startTime = node.spline.GetClosestParam(startPos);
            endTime = node.spline.GetClosestParam(endPos);

            var fakeStartTime = startTime;
            if (isStartMatchRoadDefinition) {
                var index = node.spline.GetClosestRoadDefIndex(startTime, false, true);
                var time1 = node.spline.TranslateInverseParamToFloat(node.spline.RoadDefKeysArray[index]);
                var time2 = time1;
                if (index + 1 < node.spline.RoadDefKeysArray.Length)
                    time2 = node.spline.TranslateInverseParamToFloat(node.spline.RoadDefKeysArray[index + 1]);
                fakeStartTime = time1 + (time2 - time1) * startMatchRoadDef;
            }


            //int eCount = EdgeObjects.Count;
            //Vector3 rVect = default(Vector3);
            //Vector3 lVect = default(Vector3);
            //float fTimeMax = -1f;
            var mCount = node.spline.GetNodeCount();
            if (node.idOnSpline >= mCount - 1) return;
            //fTimeMax = tNode.spline.mNodes[tNode.idOnSpline+1].tTime;
            //float tStep = -1f;
            var tVect = default(Vector3);
            var POS = default(Vector3);


            //tStep = MeterSep / tNode.spline.distance;
            //Destroy old objects:
            ClearEOM();
            //Make sure old locs and rots are fresh:
            if (edgeObjectLocations != null) {
                edgeObjectLocations.Clear();
                edgeObjectLocations = null;
            }

            edgeObjectLocations = new List<Vector3>();
            if (edgeObjectRotations != null) {
                edgeObjectRotations.Clear();
                edgeObjectRotations = null;
            }

            edgeObjectRotations = new List<Vector3>();
            var bIsCenter = RootUtils.IsApproximately(horizontalSep, 0f, 0.02f);


            //Set rotation and locations:
            //Vector2 temp2DVect = default(Vector2);
            var tRay = default(Ray);
            RaycastHit[] tRayHit = null;
            float[] tRayYs = null;
            if (isSingle) {
                // If the Object is a SingleObject


                node.spline.GetSplineValueBoth(singlePosition, out tVect, out POS);
                origHeight = tVect.y;

                //Horizontal offset:
                if (!bIsCenter)
                    //if(HorizontalSep > 0f)
                    //{
                    tVect = tVect + new Vector3(horizontalSep * POS.normalized.z, 0, horizontalSep * -POS.normalized.x);
                //}
                //else
                //{
                //  tVect = (tVect + new Vector3(HorizontalSep*-POS.normalized.z,0,HorizontalSep*POS.normalized.x));
                //}


                //Vertical:
                if (isMatchingTerrain) {
                    tRay = new Ray(tVect + new Vector3(0f, 1f, 0f), Vector3.down);
                    tRayHit = Physics.RaycastAll(tRay);
                    if (tRayHit.Length > 0) {
                        tRayYs = new float[tRayHit.Length];
                        for (var g = 0; g < tRayHit.Length; g++) tRayYs[g] = tRayHit[g].point.y;
                        tVect.y = Mathf.Max(tRayYs);
                    }
                }

                tVect.y += verticalRaise;

                startPos = tVect;
                endPos = tVect;

                if (float.IsNaN(tVect.y)) tVect.y = origHeight;

                edgeObjectLocations.Add(tVect);
                edgeObjectRotations.Add(POS);
            }
            else {
                // If this Object is not marked as a single Object

                //Get the vector series that this mesh is interpolated on:
                var tTimes = new List<float>();
                var cTime = fakeStartTime;
                tTimes.Add(cTime);
                var SpamGuard = 5000;
                var SpamGuardCounter = 0;
                var pDiffTime = endTime - fakeStartTime;
                var CurrentH = 0f;
                var fHeight = 0f;
                var xVect = default(Vector3);
                while (cTime < endTime && SpamGuardCounter < SpamGuard) {
                    node.spline.GetSplineValueBoth(cTime, out tVect, out POS);

                    fHeight = horizontalCurve.Evaluate((cTime - fakeStartTime) / pDiffTime);
                    CurrentH = fHeight * horizontalSep;

                    // Hoirzontal1:
                    if (CurrentH < 0f) {
                        // So we get a positiv Number again
                        CurrentH *= -1f;
                        tVect = tVect + new Vector3(CurrentH * (-POS.normalized.x + POS.normalized.y / 2), 0,
                            CurrentH * (POS.normalized.z + +(POS.normalized.y / 2)));
                        // I implemented the POS.normalized.y value to make sure we get to a value of 1 overall to ensure 50m distance, is this mathematicly correct?
                        // Original: tVect = (tVect + new Vector3(CurrentH * -POS.normalized.z, 0, CurrentH * POS.normalized.x));
                    }
                    else if (CurrentH > 0f) {
                        tVect = tVect + new Vector3(CurrentH * (-POS.normalized.x + POS.normalized.y / 2), 0,
                            CurrentH * (POS.normalized.z + POS.normalized.y / 2));
                        // I implemented the POS.normalized.y value to make sure we get to a value of 1 overall to ensure 50m distance, is this mathematicly correct?
                        //Original: tVect = (tVect + new Vector3(CurrentH * POS.normalized.z, 0, CurrentH * -POS.normalized.x));
                    }

                    xVect = POS.normalized * meterSep + tVect;

                    cTime = node.spline.GetClosestParam(xVect);

                    if (cTime > endTime) break;
                    tTimes.Add(cTime);
                    SpamGuardCounter += 1;
                }

                var vSeriesCount = tTimes.Count;

                var min = fakeStartTime;
                var max = endTime;
                float percent = 0;
                for (var index = 0; index < vSeriesCount; index++) {
                    node.spline.GetSplineValueBoth(tTimes[index], out tVect, out POS);

                    percent = (tTimes[index] - min) / (max - min);

                    //Horiz:
                    CurrentH = horizontalCurve.Evaluate(percent) * horizontalSep;
                    if (CurrentH < 0f) {
                        CurrentH *= -1f;
                        // Why has this Code a "wrong" logic, it multiplies z to x and x to z.
                        // Original Code: tVect = (tVect + new Vector3(CurrentH * -POS.normalized.z, 0, CurrentH * POS.normalized.x));
                        tVect = tVect + new Vector3(CurrentH * (-POS.normalized.z + POS.normalized.y / 2), 0,
                            CurrentH * (POS.normalized.x + POS.normalized.y / 2));
                    }
                    else if (CurrentH > 0f) {
                        // Original Code: tVect = (tVect + new Vector3(CurrentH * POS.normalized.z, 0, CurrentH * -POS.normalized.x));
                        // Look at the Bug embeddedt/RoadArchitect/issues/4
                        tVect = tVect + new Vector3(CurrentH * (POS.normalized.z + POS.normalized.y / 2), 0,
                            CurrentH * (-POS.normalized.x + POS.normalized.y / 2));
                    }

                    //Vertical:
                    if (isMatchingTerrain) {
                        tRay = new Ray(tVect + new Vector3(0f, 1f, 0f), Vector3.down);
                        tRayHit = Physics.RaycastAll(tRay);
                        if (tRayHit.Length > 0) {
                            tRayYs = new float[tRayHit.Length];
                            for (var g = 0; g < tRayHit.Length; g++) tRayYs[g] = tRayHit[g].point.y;
                            tVect.y = Mathf.Max(tRayYs);
                        }
                    }

                    // Adds the Height to the Node including the VerticalRaise
                    tVect.y += verticalCurve.Evaluate(percent) * verticalRaise;

                    // Adds the Vector and the POS to the List of the EdgeObjects, so they can be created
                    edgeObjectLocations.Add(tVect);
                    edgeObjectRotations.Add(POS);
                }

                startPos = node.spline.GetSplineValue(startTime);
                endPos = node.spline.GetSplineValue(endTime);
            }
        }


        //ref _verts, ref _tris, ref hNormals, ref _uv, ref hTangents
        private Mesh CombineMeshes(ref List<Vector3[]> _verts, ref List<int[]> _tris, ref List<Vector2[]> _uv,
            int _origMVL, int _origTriCount) {
            var mCount = _verts.Count;
            var NewMVL = _origMVL * mCount;
            var tVerts = new Vector3[NewMVL];
            var tTris = new int[_origTriCount * mCount];
            var tNormals = new Vector3[NewMVL];
            var tUV = new Vector2[NewMVL];

            var CurrentMVLIndex = 0;
            var CurrentTriIndex = 0;
            for (var j = 0; j < mCount; j++) {
                CurrentMVLIndex = _origMVL * j;
                CurrentTriIndex = _origTriCount * j;

                if (j > 0)
                    for (var index = 0; index < _origTriCount; index++)
                        _tris[j][index] += CurrentMVLIndex;

                Array.Copy(_verts[j], 0, tVerts, CurrentMVLIndex, _origMVL);
                Array.Copy(_tris[j], 0, tTris, CurrentTriIndex, _origTriCount);
                Array.Copy(_uv[j], 0, tUV, CurrentMVLIndex, _origMVL);
            }

            var mesh = new Mesh();
            mesh.vertices = tVerts;
            mesh.triangles = tTris;
            mesh.uv = tUV;
            mesh.normals = tNormals;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.tangents = RootUtils.ProcessTangents(tTris, tNormals, tUV, tVerts);
            return mesh;
        }


        /// <summary> Destroys edgeObjects and masterObject </summary>
        public void ClearEOM() {
            if (edgeObjects != null) {
                var hCount = edgeObjects.Count - 1;
                for (var h = hCount; h >= 0; h--)
                    if (edgeObjects[h] != null)
                        Object.DestroyImmediate(edgeObjects[h].transform.gameObject);
            }

            if (masterObject != null) Object.DestroyImmediate(masterObject);
        }

        #endregion

    }
}