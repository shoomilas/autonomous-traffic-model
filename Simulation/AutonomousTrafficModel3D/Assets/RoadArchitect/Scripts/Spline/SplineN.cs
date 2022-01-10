#region "Imports"

using System.Collections.Generic;
using RoadArchitect.EdgeObjects;
using RoadArchitect.Splination;
using UnityEngine;
using UnityEngine.Serialization;

#endregion


namespace RoadArchitect {
    public class SplineN : MonoBehaviour {


        public void LoadWizardObjectsFromLibrary(string _fileName, bool _isDefault, bool _isBridge) {
            if (_isBridge) {
                RemoveAllSplinatedObjects(true);
                RemoveAllEdgeObjects(true);
            }

            RoadUtility.LoadNodeObjects(_fileName, this, _isDefault, _isBridge);
        }


        public void Setup(Vector3 _pos, Quaternion _rot, Vector2 _io, float _time, string _name) {
            pos = _pos;
            rot = _rot;
            easeIO = _io;
            time = _time;
            name = _name;
            if (EdgeObjects == null) EdgeObjects = new List<EdgeObjectMaker>();
        }


        /// <summary> Hide or unhide this node in hierarchy </summary>
        public void ToggleHideFlags(bool _isHidden) {
            if (_isHidden) {
                hideFlags = HideFlags.HideInHierarchy;
                transform.hideFlags = HideFlags.HideInHierarchy;
            }
            else {
                hideFlags = HideFlags.None;
                transform.hideFlags = HideFlags.None;
            }
        }

        #region "Vars"

        /// <summary> Stores the position data </summary>
        public Vector3 pos;

        public Quaternion rot;
        public Vector3 tangent;

        [FormerlySerializedAs("EaseIO")] public Vector2 easeIO;

        [FormerlySerializedAs("tDist")] public float dist;

        [FormerlySerializedAs("tTime")] public float time;

        [FormerlySerializedAs("NextTime")] public float nextTime = 1f;

        [FormerlySerializedAs("NextTan")] public Vector3 nextTan;

        [FormerlySerializedAs("OldTime")] public float oldTime;

        [FormerlySerializedAs("EditorDisplayString")]
        public string editorDisplayString = "";

        public float tempSegmentTime;

        [FormerlySerializedAs("bSpecialEndNode")]
        public bool isSpecialEndNode;

        [FormerlySerializedAs("SpecialNodeCounterpart")]
        public SplineN specialNodeCounterpart;

        [FormerlySerializedAs("SpecialNodeCounterpart_Master")]
        public SplineN specialNodeCounterpartMaster;

        /// <summary> Connected nodes array </summary>
        [FormerlySerializedAs("OriginalConnectionNodes")]
        public SplineN[] originalConnectionNodes;

        [FormerlySerializedAs("bSpecialEndNode_IsStart")]
        public bool isSpecialEndNodeIsStart;

        [FormerlySerializedAs("bSpecialEndNode_IsEnd")]
        public bool isSpecialEndNodeIsEnd;

        [FormerlySerializedAs("bSpecialIntersection")]
        public bool isSpecialIntersection;

        [FormerlySerializedAs("bSpecialRoadConnPrimary")]
        public bool isSpecialRoadConnPrimary;

        [FormerlySerializedAs("bRoadCut")] public bool isRoadCut;

        [FormerlySerializedAs("MinSplination")]
        public float minSplination;

        [FormerlySerializedAs("MaxSplination")]
        public float maxSplination = 1f;

        [FormerlySerializedAs("bQuitGUI")] public bool isQuitGUI;

        public int idOnSpline = -1;

        [FormerlySerializedAs("GSDSpline")] public SplineC spline;

        //Unique ID
        [FormerlySerializedAs("UID")] public string uID;

        [FormerlySerializedAs("Intersection_OtherNode")]
        public SplineN intersectionOtherNode;
#if UNITY_EDITOR
        [FormerlySerializedAs("bEditorSelected")]
        public bool isEditorSelected;
#endif
        [FormerlySerializedAs("GradeToNext")] public string gradeToNext;

        [FormerlySerializedAs("GradeToPrev")] public string gradeToPrev;

        [FormerlySerializedAs("GradeToNextValue")]
        public float gradeToNextValue;

        [FormerlySerializedAs("GradeToPrevValue")]
        public float gradeToPrevValue;

        [FormerlySerializedAs("bInitialRoadHeight")]
        public float initialRoadHeight = -1f;

        //Navigation:
        [FormerlySerializedAs("bNeverIntersect")]
        public bool isNeverIntersect;

        /// <summary> Is this node used by an intersection </summary>
        [FormerlySerializedAs("bIsIntersection")]
        public bool isIntersection;

        /// <summary> Defines end of road, if special end or start it is the second node/second last node </summary>
        [FormerlySerializedAs("bIsEndPoint")] public bool isEndPoint;

        public int id;

        [FormerlySerializedAs("id_intersection_othernode")]
        public int intersectionOtherNodeID;

        /// <summary> Contains previous and next node ids </summary>
        [FormerlySerializedAs("id_connected")] public List<int> connectedID;

        /// <summary> Contains previous and next node </summary>
        [FormerlySerializedAs("node_connected")]
        public List<SplineN> connectedNode;

        [FormerlySerializedAs("bIgnore")] public bool isIgnore;

        [FormerlySerializedAs("opt_GizmosEnabled")]
        public bool isGizmosEnabled = true;


        #region "Tunnels"

        [FormerlySerializedAs("bIsTunnel")] public bool isTunnel;

        [FormerlySerializedAs("bIsTunnelStart")]
        public bool isTunnelStart;

        [FormerlySerializedAs("bIsTunnelEnd")] public bool isTunnelEnd;

        [FormerlySerializedAs("bIsTunnelMatched")]
        public bool isTunnelMatched;

        [FormerlySerializedAs("TunnelCounterpartNode")]
        public SplineN tunnelCounterpartNode;

        #endregion


        //Bridges:
        [FormerlySerializedAs("bIsBridge")] public bool isBridge;

        [FormerlySerializedAs("bIsBridgeStart")]
        public bool isBridgeStart;

        [FormerlySerializedAs("bIsBridgeEnd")] public bool isBridgeEnd;

        [FormerlySerializedAs("bIsBridgeMatched")]
        public bool isBridgeMatched;

        [FormerlySerializedAs("BridgeCounterpartNode")]
        public SplineN bridgeCounterpartNode;

        [FormerlySerializedAs("GSDRI")] public RoadIntersection intersection;

        [FormerlySerializedAs("iConstruction")]
        public iConstructionMaker intersectionConstruction;

        #endregion


        #region "Edge Objects"

        public List<EdgeObjectMaker> EdgeObjects;


        public void SetupEdgeObjects(bool _isCollecting = true) {
            if (EdgeObjects == null) EdgeObjects = new List<EdgeObjectMaker>();
            var eCount = EdgeObjects.Count;
            EdgeObjectMaker EOM = null;
            for (var index = 0; index < eCount; index++) {
                EOM = EdgeObjects[index];
                EOM.node = this;
                EOM.Setup(_isCollecting);
            }
        }


        public EdgeObjectMaker AddEdgeObject() {
            var EOM = new EdgeObjectMaker();
            EOM.node = this;
            EOM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            EOM.startPos = spline.GetSplineValue(EOM.startTime);
            EOM.endPos = spline.GetSplineValue(EOM.endTime);
            EdgeObjects.Add(EOM);
            return EOM;
        }


        public void CheckRenameEdgeObject(EdgeObjectMaker _eom) {
            // We have _eom already in EdgeObjects
            var names = new List<string>(EdgeObjects.Count - 1);
            for (var i = 0; i < EdgeObjects.Count; i++) {
                if (ReferenceEquals(_eom, EdgeObjects[i])) continue;

                names.Add(EdgeObjects[i].objectName);
            }

            var isNotUnique = true;
            var name = _eom.objectName;
            var counter = 1;
            while (isNotUnique) {
                if (names.Contains(_eom.objectName)) {
                    _eom.objectName = name + counter;
                    counter++;
                    continue;
                }

                break;
            }
        }


        public void EdgeObjectQuickAdd(string _name) {
            var EOM = AddEdgeObject();
            EOM.LoadFromLibrary(_name, true);
            EOM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            EOM.node = this;
            EOM.Setup();
        }


        public void RemoveEdgeObject(int _index = -1, bool _isSkippingUpdate = false) {
            if (EdgeObjects == null) return;
            if (EdgeObjects.Count == 0) return;
            if (_index < 0) {
                if (EdgeObjects.Count > 0) {
                    EdgeObjects[EdgeObjects.Count - 1].ClearEOM();
                    EdgeObjects.RemoveAt(EdgeObjects.Count - 1);
                }
            }
            else {
                if (EdgeObjects.Count > _index) {
                    EdgeObjects[_index].ClearEOM();
                    EdgeObjects.RemoveAt(_index);
                }
            }

            if (!_isSkippingUpdate) SetupEdgeObjects();
        }


        public void RemoveAllEdgeObjects(bool _isSkippingUpdate = false) {
            var SpamCheck = 0;
            while (EdgeObjects.Count > 0 && SpamCheck < 1000) {
                RemoveEdgeObject(-1, _isSkippingUpdate);
                SpamCheck += 1;
            }
        }


        public void CopyEdgeObject(int _index) {
            var EOM = EdgeObjects[_index].Copy();
            EdgeObjects.Add(EOM);
            CheckRenameEdgeObject(EOM);
            SetupEdgeObjects();
        }


        public void EdgeObjectLoadFromLibrary(int _i, string _name) {
            if (EdgeObjects == null) EdgeObjects = new List<EdgeObjectMaker>();
            var eCount = EdgeObjects.Count;
            if (_i > -1 && _i < eCount) {
                var EOM = EdgeObjects[_i];
                EOM.LoadFromLibrary(_name);
                EOM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
                EOM.node = this;
                EOM.Setup();
            }
        }


        public void DetectInvalidEdgeObjects() {
            var mCount = EdgeObjects.Count;
            var InvalidList = new List<int>();

            for (var index = 0; index < mCount; index++)
                if (EdgeObjects[index].edgeObject == null)
                    InvalidList.Add(index);

            for (var index = InvalidList.Count - 1; index >= 0; index--) RemoveEdgeObject(InvalidList[index], true);

            SetupEdgeObjects();
        }

        #endregion


        #region "Extruded objects"

        public List<SplinatedMeshMaker> SplinatedObjects;


        public void SetupSplinatedMeshes(bool _isCollecting = true) {
            if (SplinatedObjects == null) SplinatedObjects = new List<SplinatedMeshMaker>();
            var eCount = SplinatedObjects.Count;
            SplinatedMeshMaker SMM = null;
            for (var index = 0; index < eCount; index++) {
                SMM = SplinatedObjects[index];
                SMM.Setup(true, _isCollecting);
            }
        }


        public int SplinatedMeshGetIndex(ref string _uID) {
            if (SplinatedObjects == null) SplinatedObjects = new List<SplinatedMeshMaker>();
            var eCount = SplinatedObjects.Count;
            SplinatedMeshMaker SMM = null;
            for (var index = 0; index < eCount; index++) {
                SMM = SplinatedObjects[index];
                if (string.CompareOrdinal(SMM.uID, _uID) == 0) return index;
            }

            return -1;
        }


        public void SetupSplinatedMesh(int _i, bool _isGettingStrings = false) {
            if (SplinatedObjects == null) SplinatedObjects = new List<SplinatedMeshMaker>();
            var eCount = SplinatedObjects.Count;
            if (_i > -1 && _i < eCount) {
                var SMM = SplinatedObjects[_i];
                SMM.Setup(_isGettingStrings);
            }
        }


        public SplinatedMeshMaker AddSplinatedObject() {
            if (SplinatedObjects == null) SplinatedObjects = new List<SplinatedMeshMaker>();
            var SMM = new SplinatedMeshMaker();
            SMM.Init(spline, this, transform);
            SplinatedObjects.Add(SMM);
            SMM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            SMM.StartPos = spline.GetSplineValue(SMM.StartTime);
            SMM.EndPos = spline.GetSplineValue(SMM.EndTime);
            return SMM;
        }


        public void CheckRenameSplinatedObject(SplinatedMeshMaker _smm) {
            // We have _smm already in SplinatedObjects
            var names = new List<string>(SplinatedObjects.Count - 1);
            for (var i = 0; i < SplinatedObjects.Count; i++) {
                if (ReferenceEquals(_smm, SplinatedObjects[i])) continue;

                names.Add(SplinatedObjects[i].objectName);
            }

            var isNotUnique = true;
            var name = _smm.objectName;
            var counter = 1;
            while (isNotUnique) {
                if (names.Contains(_smm.objectName)) {
                    _smm.objectName = name + counter;
                    counter++;
                    continue;
                }

                break;
            }
        }


        public void SplinatedObjectQuickAdd(string _name) {
            var SMM = AddSplinatedObject();
            SMM.LoadFromLibrary(_name, true);
            SMM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            SMM.Setup(true);
        }


        public void SplinatedObjectLoadFromLibrary(int _i, string _name) {
            if (SplinatedObjects == null) SplinatedObjects = new List<SplinatedMeshMaker>();
            var eCount = SplinatedObjects.Count;
            if (_i > -1 && _i < eCount) {
                var SMM = SplinatedObjects[_i];
                SMM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
                SMM.LoadFromLibrary(_name);
                SMM.Setup(true);
            }
        }


        public void CopySplinatedObject(ref SplinatedMeshMaker _SMM) {
            var SMM = _SMM.Copy();
            SplinatedObjects.Add(SMM);
            CheckRenameSplinatedObject(SMM);
            SetupSplinatedMeshes();
        }


        public void RemoveSplinatedObject(int _index = -1, bool _isSkippingUpdate = false) {
            if (SplinatedObjects == null) return;
            if (SplinatedObjects.Count == 0) return;
            SplinatedMeshMaker SMM = null;
            if (_index < 0) {
                if (SplinatedObjects.Count > 0) {
                    SMM = SplinatedObjects[SplinatedObjects.Count - 1];
                    SMM.Kill();
                    SplinatedObjects.RemoveAt(SplinatedObjects.Count - 1);
                    SMM = null;
                }
            }
            else {
                if (SplinatedObjects.Count > _index) {
                    SMM = SplinatedObjects[_index];
                    SMM.Kill();
                    SplinatedObjects.RemoveAt(_index);
                    SMM = null;
                }
            }

            if (!_isSkippingUpdate) SetupSplinatedMeshes();
        }


        public void RemoveAllSplinatedObjects(bool _isSkippingUpdate = false) {
            var SpamCheck = 0;
            if (SplinatedObjects != null)
                while (SplinatedObjects.Count > 0 && SpamCheck < 1000) {
                    RemoveSplinatedObject(-1, _isSkippingUpdate);
                    SpamCheck += 1;
                }
        }


        public void DetectInvalidSplinatedObjects() {
            var mCount = SplinatedObjects.Count;
            var InvalidList = new List<int>();

            for (var index = 0; index < mCount; index++)
                if (SplinatedObjects[index].Output == null)
                    InvalidList.Add(index);

            for (var index = InvalidList.Count - 1; index >= 0; index--)
                RemoveSplinatedObject(InvalidList[index], true);

            SetupSplinatedMeshes();
        }

        #endregion


        #region "Gizmos"

        //	private void TerrainDebugging(){
        //			Construction3DTri tTri = null;
        //			Vector3 tHeightVec = new Vector3(0f,10f,0f);
        //			if(tTriList != null){
        //				
        //
        ////				bool bOddToggle = false;
        ////				for(int i=0;i<tTriList.Count;i+=2){
        //////					if(i < 210){ continue; }
        //////					if(i > 230){ break; }
        ////					if(i < 1330){ continue; }
        ////					if(i > 1510 || i > (tTriList.Count-10)){ break; }
        ////					tTri = tTriList[i];
        ////					
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1,tTri.P2);
        ////					Gizmos.DrawLine(tTri.P1,tTri.P3);
        ////					Gizmos.DrawLine(tTri.P2,tTri.P3);
        //////					Gizmos.color = new Color(0f,1f,0f,1f);
        //////					Gizmos.DrawLine(tTri.pMiddle,(tTri.normal*100f)+tTri.pMiddle);
        ////
        ////					
        ////					
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P3+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P3+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.color = new Color(0f,1f,0f,1f);
        ////					Gizmos.DrawLine(tTri.pMiddle+tHeightVec,(tTri.normal*100f)+tTri.pMiddle+tHeightVec);
        ////					
        //////					
        ////					
        ////					tTri = tTriList[i+1];
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1,tTri.P2);
        ////					Gizmos.DrawLine(tTri.P1,tTri.P3);
        ////					Gizmos.DrawLine(tTri.P2,tTri.P3);
        ////					
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P3+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P3+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.color = new Color(0f,1f,0f,1f);
        ////					Gizmos.DrawLine(tTri.pMiddle+tHeightVec,(tTri.normal*100f)+tTri.pMiddle+tHeightVec);
        ////					
        ////	
        ////					
        ////					bOddToggle = !bOddToggle;
        //////					Gizmos.DrawCube(tRectList[i].pMiddle,new Vector3(1f,0.5f,1f));
        ////				}
        //				
        //				
        ////				Gizmos.color = new Color(0f,1f,0f,1f);
        ////				Terrain tTerrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        ////				Vector3 HMVect = default(Vector3);
        ////				float tHeight = 0f;
        ////				for(int i=0;i<tHMList.Count;i++){
        ////					Gizmos.color = new Color(0f,1f,0f,1f);
        ////					Gizmos.DrawCube(tHMList[i] + new Vector3(0f,1f,0f),new Vector3(0.1f,2f,0.1f));
        ////				
        ////					if(tTerrain){
        ////						tHeight = tTerrain.SampleHeight(tHMList[i]) + tTerrain.transform.position.y;
        ////						
        ////						if(tHeight != tHMList[i].y){
        ////							HMVect = new Vector3(tHMList[i].x,tHeight,tHMList[i].z);
        ////							
        ////							if(Mathf.Approximately(9.584141f,tHMList[i].y)){
        ////								int sdagsdgsd =1;	
        ////							}
        ////							
        ////							Gizmos.color = new Color(0f,0f,1f,1f);
        ////							Gizmos.DrawWireCube(HMVect + new Vector3(0f,1f,0f),new Vector3(0.15f,2f,0.15f));
        ////						}
        ////					}
        ////				}
        //			}
        //			
        ////			Vector3 P1 = new Vector3(480.7f,50f,144.8f);
        ////			Vector3 P2 = new Vector3(517.3f,60f,128.9f);
        ////			Vector3 P3 = new Vector3(518.8f,60f,132.7f);
        ////			Vector3 P4 = new Vector3(481.3f,50f,146.4f);
        ////			
        ////			
        ////			Gizmos.color = new Color(1f,0f,0f,1f);
        ////			Gizmos.DrawCube(P1,new Vector3(1f,1f,1f));
        ////			Gizmos.DrawCube(P2,new Vector3(1f,1f,1f));
        ////			Gizmos.DrawCube(P3,new Vector3(1f,1f,1f));
        ////			Gizmos.DrawCube(P4,new Vector3(1f,1f,1f));
        ////			
        ////			tRect = new Construction3DRect(P1,P2,P3,P4);
        ////
        ////			Gizmos.color = new Color(0f,0f,1f,1f);
        ////			Gizmos.DrawCube(tRect.pMiddle,new Vector3(0.1f,0.1f,0.1f));
        ////			
        ////			//This creates normalized line:
        ////			Gizmos.color = new Color(0f,1f,0f,1f);
        ////			Vector3 tVect2 = (tRect.normal.normalized*100f)+tRect.pMiddle;
        ////			Gizmos.DrawLine(tRect.pMiddle,tVect2);
        ////			
        ////			//This creates emulated terrain point and straight up line:
        ////			Gizmos.color = new Color(0f,1f,1f,1f);
        ////			Vector3 F1 = new Vector3(500f,0f,138.5f);
        ////			Gizmos.DrawLine(F1,F1+ new Vector3(0f,100f,0f));
        ////			
        ////			//This creates diagonal line of plane.
        ////			Gizmos.color = new Color(1f,0f,1f,1f);
        ////			Gizmos.DrawLine(P1,P3);
        ////			Gizmos.DrawLine(P2,P4);
        ////			
        ////			//This creates intersection point:
        ////			Vector3 tPos = default(Vector3);
        ////			LinePlaneIntersection(out tPos,F1,Vector3.up,tRect.normal.normalized,tRect.pMiddle);
        ////			Gizmos.color = new Color(1f,1f,0f,1f);
        ////			Gizmos.DrawCube(tPos,new Vector3(0.3f,0.3f,0.3f));
        //	}
        //	

        public List<Construction3DTri> tTriList;
        public List<Vector3> tHMList;

        [FormerlySerializedAs("bGizmoDrawIntersectionHighlight")]
        public bool isDrawingIntersectionHighlightGizmos;


        private void OnDrawGizmos() {
            DrawGizmos(false);
        }


        private void OnDrawGizmosSelected() {
            DrawGizmos(true);
        }


        private void DrawGizmos(bool _isSelected) {
            if (spline == null) return;
            if (spline.road == null) return;
            if (!spline.road.isGizmosEnabled) return;
            if (isIgnore) return;
            if (isIntersection) return;
            if (isSpecialEndNode) return;
            if (isSpecialEndNodeIsEnd || isSpecialEndNodeIsStart) return;


            if (_isSelected) {
                if (isDrawingIntersectionHighlightGizmos && !isSpecialEndNode && isIntersection) {
                    Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
                    Gizmos.DrawCube(transform.position + new Vector3(0f, 2f, 0f), new Vector3(32f, 4f, 32f));
                }

                Gizmos.color = spline.road.selectedColor;
                Gizmos.DrawCube(transform.position + new Vector3(0f, 6.25f, 0f), new Vector3(3.5f, 12.5f, 3.5f));
                return;
            }

            // Not Selected
            if (isDrawingIntersectionHighlightGizmos && !isSpecialEndNode && isIntersection) {
                Gizmos.color = spline.road.Color_NodeInter;
                Gizmos.DrawCube(transform.position + new Vector3(0f, 2f, 0f), new Vector3(32f, 4f, 32f));
                return;
            }

            if (isSpecialRoadConnPrimary) {
                Gizmos.color = spline.road.Color_NodeConnColor;
                Gizmos.DrawCube(transform.position + new Vector3(0f, 7.5f, 0f), new Vector3(5f, 15f, 5f));
            }
            else {
                Gizmos.color = spline.road.defaultNodeColor;
                Gizmos.DrawCube(transform.position + new Vector3(0f, 6f, 0f), new Vector3(2f, 11f, 2f));
            }
        }

        #endregion


        #region "Grade"

        public void SetGradePercent(int _nodeCount) {
            var P1 = default(Vector3);
            var P2 = default(Vector3);
            float dist;
            float grade;
            var isNone = false;

            if (_nodeCount < 2) {
                gradeToPrev = "NA";
                gradeToNext = "NA";
                gradeToNextValue = 0f;
                gradeToPrevValue = 0f;
            }

            if (!isEndPoint && !isSpecialEndNode && _nodeCount > 1 && idOnSpline + 1 < spline.nodes.Count) {
                P1 = pos;
                P2 = spline.nodes[idOnSpline + 1].pos;

                if (isNone) {
                    grade = 0f;
                }
                else {
                    dist = Vector2.Distance(new Vector2(P1.x, P1.z), new Vector2(P2.x, P2.z));
                    grade = (P2.y - P1.y) / dist * 100;
                }

                gradeToNextValue = grade;
                gradeToNext = grade.ToString("0.##\\%");
            }
            else {
                gradeToNext = "NA";
                gradeToNextValue = 0f;
            }

            if (idOnSpline > 0 && !isSpecialEndNode && _nodeCount > 1 && idOnSpline - 1 >= 0) {
                P1 = pos;
                P2 = spline.nodes[idOnSpline - 1].pos;

                if (isNone) {
                    grade = 0f;
                }
                else {
                    dist = Vector2.Distance(new Vector2(P1.x, P1.z), new Vector2(P2.x, P2.z));
                    grade = (P2.y - P1.y) / dist * 100;
                }

                gradeToPrevValue = grade;
                gradeToPrev = grade.ToString("0.##\\%");
            }
            else {
                gradeToPrev = "NA";
                gradeToPrevValue = 0f;
            }
        }


        public Vector3 FilterMaxGradeHeight(Vector3 _pos, out float _minY, out float _maxY) {
            var tVect = _pos;
            tVect.y = pos.y;
            var CurrentDistance = Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(_pos.x, _pos.z));
            //		float CurrentDistance2 = Vector3.Distance(pos,tVect);
            //		float CurrentYDiff = tPos.y - pos.y;
            //		float CurrentGrade = CurrentYDiff/CurrentDistance;
            //Get max/min grade height position for this currrent tDist distance:
            _maxY = spline.road.maxGrade * CurrentDistance + pos.y;
            _minY = pos.y - spline.road.maxGrade * CurrentDistance;

            //(tPos.y-pos.y)/CurrentDistance


            //		float DifferenceFromMax = -1;
            if (_pos.y > _maxY)
                //			DifferenceFromMax = tPos.y - MaximumY;
                _pos.y = _maxY;
            if (_pos.y < _minY)
                //			DifferenceFromMax = MinimumY - tPos.y;
                _pos.y = _minY;

            return _pos;
        }


        public void EnsureGradeValidity(int _iStart = -1, bool _isAddToEnd = false) {
            if (spline == null) return;
            SplineN PrevNode = null;
            SplineN NextNode = null;

            if (_isAddToEnd && spline.GetNodeCount() > 0) {
                PrevNode = spline.nodes[spline.GetNodeCount() - 1];
            }
            else {
                if (_iStart == -1)
                    PrevNode = spline.GetPrevLegitimateNode(idOnSpline);
                else
                    PrevNode = spline.GetPrevLegitimateNode(_iStart);
            }

            if (PrevNode == null) return;
            var tVect = transform.position;

            var tMinY1 = 0f;
            var tMinY2 = 0f;
            var tMaxY1 = 0f;
            var tMaxY2 = 0f;
            if (PrevNode != null) PrevNode.FilterMaxGradeHeight(tVect, out tMinY1, out tMaxY1);
            if (NextNode != null) NextNode.FilterMaxGradeHeight(tVect, out tMinY2, out tMaxY2);

            var bPrevNodeGood = false;
            var bNextNodeGood = false;

            if (tVect.y > tMinY1 && tVect.y < tMaxY1) bPrevNodeGood = true;
            if (tVect.y > tMinY2 && tVect.y < tMaxY2) bNextNodeGood = true;

            if (!bPrevNodeGood && !bNextNodeGood && PrevNode != null && NextNode != null) {
                var tMaxY3 = Mathf.Min(tMaxY1, tMaxY2);
                var tMinY3 = Mathf.Max(tMinY1, tMinY2);
                if (tVect.y < tMinY3)
                    tVect.y = tMinY3;
                else if (tVect.y > tMaxY3) tVect.y = tMaxY3;
            }
            else {
                if (!bPrevNodeGood && PrevNode != null) {
                    if (tVect.y < tMinY1)
                        tVect.y = tMinY1;
                    else if (tVect.y > tMaxY1) tVect.y = tMaxY1;
                }
                else if (!bNextNodeGood && NextNode != null) {
                    if (tVect.y < tMinY2)
                        tVect.y = tMinY2;
                    else if (tVect.y > tMaxY2) tVect.y = tMaxY2;
                }
            }

            transform.position = tVect;
        }

        #endregion


        #region "Util"

        public void ResetNavigationData() {
            connectedID = null;
            connectedID = new List<int>();
            connectedNode = null;
            connectedNode = new List<SplineN>();
        }


        public void BreakConnection() {
            var tNode2 = specialNodeCounterpart;

            if (isSpecialEndNodeIsStart) {
                spline.isSpecialStartControlNode = false;
                spline.isSpecialEndNodeIsStartDelay = false;
            }
            else if (isSpecialEndNodeIsEnd) {
                spline.isSpecialEndControlNode = false;
                spline.isSpecialEndNodeIsEndDelay = false;
            }

            if (tNode2.isSpecialEndNodeIsStart) {
                tNode2.spline.isSpecialStartControlNode = false;
                tNode2.spline.isSpecialEndNodeIsStartDelay = false;
            }
            else if (tNode2.isSpecialEndNodeIsEnd) {
                tNode2.spline.isSpecialEndControlNode = false;
                tNode2.spline.isSpecialEndNodeIsEndDelay = false;
            }

            specialNodeCounterpart = null;
            isSpecialEndNode = false;
            isSpecialEndNodeIsEnd = false;
            isSpecialEndNodeIsStart = false;
            isSpecialRoadConnPrimary = false;
            tNode2.specialNodeCounterpart = null;
            tNode2.isSpecialEndNode = false;
            tNode2.isSpecialEndNodeIsEnd = false;
            tNode2.isSpecialEndNodeIsStart = false;
            tNode2.isSpecialRoadConnPrimary = false;

            tNode2.specialNodeCounterpartMaster.isSpecialRoadConnPrimary = false;
            specialNodeCounterpartMaster.isSpecialRoadConnPrimary = false;
            try {
                DestroyImmediate(tNode2.transform.gameObject);
                DestroyImmediate(transform.gameObject);
            }
            catch (MissingReferenceException) { }
        }


        public void SetupSplinationLimits() {
            //Disallowed nodes:
            if (!CanSplinate()) {
                minSplination = time;
                maxSplination = time;
                return;
            }

            //Figure out min splination:
            SplineN node = null;
            minSplination = time;
            for (var index = idOnSpline; index >= 0; index--) {
                node = spline.nodes[index];
                if (node.CanSplinate())
                    minSplination = node.time;
                else
                    break;
            }

            //Figure out max splination:
            maxSplination = time;
            var nodeCount = spline.GetNodeCount();
            for (var index = idOnSpline; index < nodeCount; index++) {
                node = spline.nodes[index];
                if (node.CanSplinate())
                    maxSplination = node.time;
                else
                    break;
            }
        }

        #endregion


        #region "Cut materials storage and setting"

        [FormerlySerializedAs("RoadCut_world")]
        public GameObject roadCutWorld;

        [FormerlySerializedAs("ShoulderCutR_world")]
        public GameObject shoulderCutRWorld;

        [FormerlySerializedAs("ShoulderCutL_world")]
        public GameObject shoulderCutLWorld;

        [FormerlySerializedAs("RoadCut_world_Mats")]
        public Material[] roadCutWorldMats;

        [FormerlySerializedAs("ShoulderCutR_world_Mats")]
        public Material[] shoulderCutRWorldMats;

        [FormerlySerializedAs("ShoulderCutL_world_Mats")]
        public Material[] shoulderCutLWorldMats;

        [FormerlySerializedAs("RoadCut_marker")]
        public GameObject roadCutMarker;

        [FormerlySerializedAs("ShoulderCutR_marker")]
        public GameObject shoulderCutRMarker;

        [FormerlySerializedAs("ShoulderCutL_marker")]
        public GameObject shoulderCutLMarker;

        [FormerlySerializedAs("RoadCut_marker_Mats")]
        public Material[] roadCutMarkerMats;

        [FormerlySerializedAs("ShoulderCutR_marker_Mats")]
        public Material[] shoulderCutRMarkerMats;

        [FormerlySerializedAs("ShoulderCutL_marker_Mats")]
        public Material[] shoulderCutLMarkerMats;

        [FormerlySerializedAs("RoadCut_PhysicMat")]
        public PhysicMaterial roadCutPhysicMat;

        [FormerlySerializedAs("ShoulderCutR_PhysicMat")]
        public PhysicMaterial shoulderCutRPhysicMat;

        [FormerlySerializedAs("ShoulderCutL_PhysicMat")]
        public PhysicMaterial shoulderCutLPhysicMat;


        /// <summary>
        ///     Stores the cut materials. For use in UpdateCuts(). See UpdateCuts() in this code file for further description
        ///     of this system.
        /// </summary>
        public void StoreCuts() {
            //Buffers:
            MeshRenderer MR = null;
            MeshCollider MC = null;

            //World cuts first:
            if (roadCutWorld != null) {
                MR = roadCutWorld.GetComponent<MeshRenderer>();
                MC = roadCutWorld.GetComponent<MeshCollider>();
                if (MR != null) roadCutWorldMats = MR.sharedMaterials;
                if (MC != null) {
                    roadCutPhysicMat = MC.material;
                    roadCutPhysicMat.name = roadCutPhysicMat.name.Replace(" (Instance)", "");
                }

                //Nullify reference only
                roadCutWorld = null;
            }

            if (shoulderCutRWorld != null) {
                MR = shoulderCutRWorld.GetComponent<MeshRenderer>();
                MC = shoulderCutRWorld.GetComponent<MeshCollider>();
                if (MR != null) shoulderCutRWorldMats = MR.sharedMaterials;
                if (MC != null) {
                    shoulderCutRPhysicMat = MC.material;
                    shoulderCutRPhysicMat.name = shoulderCutRPhysicMat.name.Replace(" (Instance)", "");
                }

                shoulderCutRWorld = null;
            }

            if (shoulderCutLWorld != null) {
                MR = shoulderCutLWorld.GetComponent<MeshRenderer>();
                MC = shoulderCutLWorld.GetComponent<MeshCollider>();
                if (MR != null) shoulderCutLWorldMats = MR.sharedMaterials;
                if (MC != null) {
                    shoulderCutLPhysicMat = MC.material;
                    shoulderCutLPhysicMat.name = shoulderCutLPhysicMat.name.Replace(" (Instance)", "");
                }

                shoulderCutLWorld = null;
            }

            //Markers:
            if (roadCutMarker != null) {
                MR = roadCutMarker.GetComponent<MeshRenderer>();
                if (MR != null) roadCutMarkerMats = MR.sharedMaterials;
                roadCutMarker = null;
            }

            if (shoulderCutRMarker != null) {
                MR = shoulderCutRMarker.GetComponent<MeshRenderer>();
                if (MR != null) shoulderCutRMarkerMats = MR.sharedMaterials;
                shoulderCutRMarker = null;
            }

            if (shoulderCutLMarker != null) {
                MR = shoulderCutLMarker.GetComponent<MeshRenderer>();
                if (MR != null) shoulderCutLMarkerMats = MR.sharedMaterials;
                shoulderCutLMarker = null;
            }
        }


        /// <summary>
        ///     Updates the cut materials. Called upon creation of the cuts to set the newly cut materials to previously set
        ///     materials.
        ///     For instance, if the user set a material on a road cut, and then regenerated the road, this function will apply the
        ///     mats that the user applied.
        /// </summary>
        public void UpdateCuts() {
            //Buffers:
            MeshRenderer MR = null;
            MeshCollider MC = null;

            #region "Physic Materials"

            //World:
            if (roadCutPhysicMat != null && roadCutWorld) {
                MC = roadCutWorld.GetComponent<MeshCollider>();
                if (MC != null) {
                    MC.material = roadCutPhysicMat;
                    MC.material.name = MC.material.name.Replace(" (Instance)", "");
                }
            }

            if (shoulderCutRPhysicMat != null && shoulderCutRWorld) {
                MC = shoulderCutRWorld.GetComponent<MeshCollider>();
                if (MC != null) {
                    MC.material = shoulderCutRPhysicMat;
                    MC.material.name = MC.material.name.Replace(" (Instance)", "");
                }
            }

            if (shoulderCutLPhysicMat != null && shoulderCutLWorld) {
                MC = shoulderCutLWorld.GetComponent<MeshCollider>();
                if (MC != null) {
                    MC.material = shoulderCutLPhysicMat;
                    MC.material.name = MC.material.name.Replace(" (Instance)", "");
                }
            }

            #endregion

            #region "Get or Add MeshRenderer"

            if (roadCutWorldMats != null && roadCutWorldMats.Length > 0 && roadCutWorld) {
                MR = roadCutWorld.GetComponent<MeshRenderer>();
                if (!MR) roadCutWorld.AddComponent<MeshRenderer>();
                if (MR != null) MR.materials = roadCutWorldMats;
            }

            if (shoulderCutRWorldMats != null && shoulderCutRWorldMats.Length > 0 && shoulderCutRWorld) {
                MR = shoulderCutRWorld.GetComponent<MeshRenderer>();
                if (!MR) shoulderCutRWorld.AddComponent<MeshRenderer>();
                if (MR != null) MR.materials = shoulderCutRWorldMats;
            }

            if (shoulderCutLWorldMats != null && shoulderCutLWorldMats.Length > 0 && shoulderCutLWorld) {
                MR = shoulderCutLWorld.GetComponent<MeshRenderer>();
                if (!MR) shoulderCutLWorld.AddComponent<MeshRenderer>();
                if (MR != null) MR.materials = shoulderCutLWorldMats;
            }


            //Markers:
            if (roadCutMarkerMats != null && roadCutMarkerMats.Length > 0 && roadCutMarker) {
                MR = roadCutMarker.GetComponent<MeshRenderer>();
                if (!MR) roadCutMarker.AddComponent<MeshRenderer>();
                if (MR != null) MR.materials = roadCutMarkerMats;
            }

            if (shoulderCutRMarkerMats != null && shoulderCutRMarkerMats.Length > 0 && shoulderCutRMarker) {
                MR = shoulderCutRMarker.GetComponent<MeshRenderer>();
                if (!MR) shoulderCutRMarker.AddComponent<MeshRenderer>();
                if (MR != null) MR.materials = shoulderCutRMarkerMats;
            }

            if (shoulderCutLMarkerMats != null && shoulderCutLMarkerMats.Length > 0 && shoulderCutLMarker) {
                MR = shoulderCutLMarker.GetComponent<MeshRenderer>();
                if (!MR) shoulderCutLMarker.AddComponent<MeshRenderer>();
                if (MR != null) MR.materials = shoulderCutLMarkerMats;
            }

            #endregion

            #region "Destroy if empty"

            if (roadCutMarker != null) {
                MR = roadCutMarker.GetComponent<MeshRenderer>();
                if (MR == null || MR.sharedMaterial == null) DestroyImmediate(roadCutMarker);
            }

            if (shoulderCutRMarker != null) {
                MR = shoulderCutRMarker.GetComponent<MeshRenderer>();
                if (MR == null || MR.sharedMaterial == null) DestroyImmediate(shoulderCutRMarker);
            }

            if (shoulderCutLMarker != null) {
                MR = shoulderCutLMarker.GetComponent<MeshRenderer>();
                if (MR == null || MR.sharedMaterial == null) DestroyImmediate(shoulderCutLMarker);
            }

            #endregion
        }


        /// <summary> Clears the cut materials </summary>
        public void ClearCuts() {
            roadCutWorldMats = null;
            shoulderCutRWorldMats = null;
            shoulderCutLWorldMats = null;
            roadCutMarkerMats = null;
            shoulderCutRMarkerMats = null;
            shoulderCutLMarkerMats = null;
            roadCutPhysicMat = null;
            shoulderCutRPhysicMat = null;
            shoulderCutLPhysicMat = null;
        }

        #endregion


        #region "Bridges"

        public void BridgeToggleStart() {
            //If switching to end, find associated bridge 
            if (isBridgeStart)
                BridgeStart();
            else
                BridgeDestroy();
        }


        public void BridgeToggleEnd() {
            //If switching to end, find associated bridge 
            if (isBridgeEnd) {
                var nodeCount = spline.GetNodeCount();
                SplineN node = null;
                for (var i = 1; i < nodeCount - 1; i++) {
                    node = spline.nodes[i];
                    if (node.isBridgeStart && !node.isBridgeMatched) {
                        node.BridgeToggleStart();
                        if (node.isBridgeMatched && node.bridgeCounterpartNode == this) return;
                    }
                }
            }
            else {
                BridgeDestroy();
            }
        }


        private void BridgeStart() {
            //Cycle through nodes until you find another end or another start (in this case, no creation, encountered another bridge):
            var EndID = idOnSpline + 1;
            var nodeCount = spline.GetNodeCount();
            if (isEndPoint) {
                //Attempted to make end point node a bridge node:
                isBridgeStart = false;
                return;
            }

            if (EndID >= nodeCount) {
                //Attempted to make last node a bridge node:
                isBridgeStart = false;
                return;
            }

            if (idOnSpline == 0) {
                //Attempted to make first node a bridge node:
                isBridgeStart = false;
                return;
            }

            isBridgeMatched = false;
            bridgeCounterpartNode = null;
            var StartI = idOnSpline + 1;
            SplineN tNode = null;
            for (var i = StartI; i < nodeCount; i++) {
                tNode = spline.nodes[i];
                if (tNode.isIntersection)
                    //Encountered intersection. End search.
                    return;
                if (tNode.isBridgeStart)
                    //Encountered another bridge. Return:
                    return;
                if (tNode.isIgnore) continue;
                if (tNode.isBridgeEnd) {
                    isBridgeMatched = true;
                    tNode.isBridgeMatched = true;
                    bridgeCounterpartNode = tNode;
                    tNode.bridgeCounterpartNode = this;
                    spline.TriggerSetup();
                    return;
                }
            }
        }


        private void BridgeDestroy() {
            if (bridgeCounterpartNode != null) bridgeCounterpartNode.BridgeResetValues();
            BridgeResetValues();
            spline.TriggerSetup();
        }


        public void BridgeResetValues() {
            isBridge = false;
            isBridgeStart = false;
            isBridgeEnd = false;
            isBridgeMatched = false;
            bridgeCounterpartNode = null;
        }


        public bool CanBridgeStart() {
            if (isBridgeStart) return true;
            if (isBridgeEnd) return false;
            if (isEndPoint) return false;

            var mCount = spline.GetNodeCount();

            if (idOnSpline > 0)
                if (spline.nodes[idOnSpline - 1].isBridgeStart)
                    return false;

            if (idOnSpline < mCount - 1) {
                if (spline.nodes[idOnSpline + 1].isBridgeStart) return false;

                if (spline.isSpecialEndControlNode) {
                    if (mCount - 3 > 0 && idOnSpline == mCount - 3) return false;
                }
                else {
                    if (mCount - 2 > 0 && idOnSpline == mCount - 2) return false;
                }
            }

            if (spline.IsInBridge(time)) return false;

            return true;
        }


        public bool CanBridgeEnd() {
            if (isBridgeEnd) return true;
            if (isBridgeStart) return false;
            if (isEndPoint) return false;

            var mCount = spline.GetNodeCount();

            if (idOnSpline < mCount - 1) {
                if (spline.isSpecialStartControlNode) {
                    if (idOnSpline == 2) return false;
                }
                else {
                    if (idOnSpline == 1) return false;
                }
            }

            for (var i = idOnSpline; i >= 0; i--)
                if (spline.nodes[i].isBridgeStart) {
                    if (!spline.nodes[i].isBridgeMatched)
                        return true;
                    return false;
                }

            return false;
        }

        #endregion


        #region "Tunnels"

        public void TunnelToggleStart() {
            //If switching to end, find associated Tunnel 
            if (isTunnelStart)
                TunnelStart();
            else
                TunnelDestroy();
        }


        public void TunnelToggleEnd() {
            //If switching to end, find associated Tunnel 
            if (isTunnelEnd) {
                var nodeCount = spline.GetNodeCount();
                SplineN node = null;
                for (var index = 1; index < nodeCount - 1; index++) {
                    node = spline.nodes[index];
                    if (node.isTunnelStart && !node.isTunnelMatched) {
                        node.TunnelToggleStart();
                        if (node.isTunnelMatched && node.tunnelCounterpartNode == this) return;
                    }
                }
            }
            else {
                TunnelDestroy();
            }
        }


        private void TunnelStart() {
            //Cycle through nodes until you find another end or another start (in this case, no creation, encountered another Tunnel):
            var EndID = idOnSpline + 1;
            var mCount = spline.GetNodeCount();
            if (isEndPoint) {
                //Attempted to make end point node a Tunnel node:
                isTunnelStart = false;
                return;
            }

            if (EndID >= mCount) {
                //Attempted to make last node a Tunnel node:
                isTunnelStart = false;
                return;
            }

            if (idOnSpline == 0) {
                //Attempted to make first node a Tunnel node:
                isTunnelStart = false;
                return;
            }

            isTunnelMatched = false;
            tunnelCounterpartNode = null;
            var StartI = idOnSpline + 1;
            SplineN node = null;
            for (var i = StartI; i < mCount; i++) {
                node = spline.nodes[i];
                if (node.isIntersection)
                    //Encountered intersection. End search.
                    return;
                if (node.isTunnelStart)
                    //Encountered another Tunnel. Return:
                    return;
                if (node.isIgnore) continue;
                if (node.isTunnelEnd) {
                    isTunnelMatched = true;
                    node.isTunnelMatched = true;
                    tunnelCounterpartNode = node;
                    node.tunnelCounterpartNode = this;
                    spline.TriggerSetup();
                    return;
                }
            }
        }


        private void TunnelDestroy() {
            if (tunnelCounterpartNode != null) tunnelCounterpartNode.TunnelResetValues();
            TunnelResetValues();
            spline.TriggerSetup();
        }


        public void TunnelResetValues() {
            isTunnel = false;
            isTunnelStart = false;
            isTunnelEnd = false;
            isTunnelMatched = false;
            tunnelCounterpartNode = null;
        }


        public bool CanTunnelStart() {
            if (isTunnelStart) return true;
            if (isTunnelEnd) return false;
            if (isEndPoint) return false;

            var mCount = spline.GetNodeCount();

            if (idOnSpline > 0)
                if (spline.nodes[idOnSpline - 1].isTunnelStart)
                    return false;

            if (idOnSpline < mCount - 1) {
                if (spline.nodes[idOnSpline + 1].isTunnelStart) return false;

                if (spline.isSpecialEndControlNode) {
                    if (mCount - 3 > 0 && idOnSpline == mCount - 3) return false;
                }
                else {
                    if (mCount - 2 > 0 && idOnSpline == mCount - 2) return false;
                }
            }

            if (spline.IsInTunnel(time)) return false;

            return true;
        }


        public bool CanTunnelEnd() {
            if (isTunnelEnd) return true;
            if (isTunnelStart) return false;
            if (isEndPoint) return false;

            var nodeCount = spline.GetNodeCount();

            if (idOnSpline < nodeCount - 1) {
                if (spline.isSpecialStartControlNode) {
                    if (idOnSpline == 2) return false;
                }
                else {
                    if (idOnSpline == 1) return false;
                }
            }

            for (var i = idOnSpline; i >= 0; i--)
                if (spline.nodes[i].isTunnelStart) {
                    if (!spline.nodes[i].isTunnelMatched)
                        return true;
                    return false;
                }

            return false;
        }

        #endregion


        #region "Is straight line to next node"

        /// <summary> Returns true if two of 3 pos Vectors to previous and next 2 nodes approx. match </summary>
        public bool IsStraight() {
            var id1 = idOnSpline - 1;
            var id2 = idOnSpline + 1;
            var id3 = idOnSpline + 2;
            var nodeCount = spline.GetNodeCount();

            if (id1 > -1 && id1 < nodeCount)
                if (!IsApproxTwoThirds(ref pos, spline.nodes[id1].pos))
                    return false;

            if (id2 > -1 && id2 < nodeCount)
                if (!IsApproxTwoThirds(ref pos, spline.nodes[id2].pos))
                    return false;

            if (id3 > -1 && id3 < nodeCount)
                if (!IsApproxTwoThirds(ref pos, spline.nodes[id3].pos))
                    return false;

            return true;
        }


        /// <summary> Returns <see langword="true" /> if exactly 2 values are approximately the same </summary>
        private static bool IsApproxTwoThirds(ref Vector3 _v1, Vector3 _v2) {
            var cCount = 0;
            if (RootUtils.IsApproximately(_v1.x, _v2.x, 0.02f)) cCount += 1;
            if (RootUtils.IsApproximately(_v1.y, _v2.y, 0.02f)) cCount += 1;
            if (RootUtils.IsApproximately(_v1.z, _v2.z, 0.02f)) cCount += 1;

            if (cCount == 2)
                return true;
            return false;
        }

        #endregion


        #region "Non-editor util"

        /// <summary> Returns false when isSpecialEndNode </summary>
        public bool CanSplinate() {
            if (isSpecialEndNode)
                // || bIsBridge_PreNode || bIsBridge_PostNode){
                return false;
            return true;
        }


        /// <summary> Returns false when isIntersection or isSpecialEndNode </summary>
        public bool IsLegitimate() {
            if (isIntersection || isSpecialEndNode)
                // || bIsBridge_PreNode || bIsBridge_PostNode){
                return false;
            return true;
        }


        /// <summary> Returns false when isSpecialEndNode </summary>
        public bool IsLegitimateGrade() {
            if (isSpecialEndNode)
                // || bIsBridge_PreNode || bIsBridge_PostNode){
                return false;
            return true;
        }

        #endregion

    }
}