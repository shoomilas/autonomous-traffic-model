#region "Imports"

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

#endregion


namespace RoadArchitect {
    public class SplineC : MonoBehaviour {


        #region "Road connections"

        /// <summary> Creates a conncetion between first and last node </summary>
        public void ActivateEndNodeConnection(SplineN _node1, SplineN _node2) {
            var spline = _node2.spline;
            var nodeCount = spline.GetNodeCount();
            var mCount = GetNodeCount();
            //Don't allow connection with less than 3 nodes:
            if (mCount < 3 || nodeCount < 3) {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Cannot connect roads", "Roads must have at least 3 nodes to be connected.",
                    "ok");
#endif

                return;
            }

            var node1ExtraPos = default(Vector3);
            var node2ExtraPos = default(Vector3);

            var isNode1Start = false;
            //bool isNode1End = false;
            if (_node1.idOnSpline == 0) {
                isNode1Start = true;
                node2ExtraPos = nodes[1].transform.position;
            }
            else {
                //isNode1End = true;
                node2ExtraPos = nodes[mCount - 2].transform.position;
            }


            var isNode2Start = false;
            //bool isNode2End = false;
            if (_node2.idOnSpline == 0) {
                isNode2Start = true;
                node1ExtraPos = spline.nodes[1].transform.position;
            }
            else {
                //isNode2End = true;
                node1ExtraPos = spline.nodes[nodeCount - 2].transform.position;
            }

            SplineN NodeCreated1 = null;
            SplineN NodeCreated2 = null;

            if (isNode1Start) {
                isSpecialStartControlNode = true;
                if (nodes[0].isSpecialEndNode) {
                    nodes[0].transform.position = node1ExtraPos;
                    nodes[0].pos = node1ExtraPos;
                    NodeCreated1 = nodes[0];
                }
                else {
                    NodeCreated1 = Construction.InsertNode(road, true, node1ExtraPos, false, 0, true);
                }
            }
            else {
                isSpecialEndControlNode = true;
                var zNode1 = spline.GetLastNodeAll();
                if (zNode1 != null && zNode1.isSpecialEndNode) {
                    zNode1.transform.position = node1ExtraPos;
                    zNode1.pos = node1ExtraPos;
                    NodeCreated1 = GetLastNodeAll();
                }
                else {
                    NodeCreated1 = Construction.CreateNode(road, true, node1ExtraPos);
                }
            }

            if (isNode2Start) {
                spline.isSpecialStartControlNode = true;
                if (spline.nodes[0].isSpecialEndNode) {
                    spline.nodes[0].transform.position = node2ExtraPos;
                    spline.nodes[0].pos = node2ExtraPos;
                    NodeCreated2 = spline.nodes[0];
                }
                else {
                    NodeCreated2 = Construction.InsertNode(spline.road, true, node2ExtraPos, false, 0, true);
                }
            }
            else {
                spline.isSpecialEndControlNode = true;
                var zNode2 = spline.GetLastNodeAll();
                if (zNode2 != null && zNode2.isSpecialEndNode) {
                    zNode2.transform.position = node2ExtraPos;
                    zNode2.pos = node2ExtraPos;
                    NodeCreated2 = spline.GetLastNodeAll();
                }
                else {
                    NodeCreated2 = Construction.CreateNode(spline.road, true, node2ExtraPos);
                }
            }

            NodeCreated1.isSpecialEndNodeIsStart = isNode1Start;
            NodeCreated2.isSpecialEndNodeIsStart = isNode2Start;
            NodeCreated1.isSpecialEndNodeIsEnd = !isNode1Start;
            NodeCreated2.isSpecialEndNodeIsEnd = !isNode2Start;
            NodeCreated1.specialNodeCounterpart = NodeCreated2;
            NodeCreated2.specialNodeCounterpart = NodeCreated1;

            var lWidth1 = _node1.spline.road.laneWidth;
            var lWidth2 = _node2.spline.road.laneWidth;
            var xWidth = Mathf.Max(lWidth1, lWidth2);

            var tDelay = 0f;
            // Handle different amount of lanes
            if (_node1.spline.road.laneAmount > _node2.spline.road.laneAmount) {
                _node2.isSpecialRoadConnPrimary = true;
                NodeCreated2.isSpecialRoadConnPrimary = true;
                if (_node2.spline.road.laneAmount == 4) xWidth *= 2f;
                tDelay = (_node1.spline.road.laneAmount - _node2.spline.road.laneAmount) * xWidth;
                if (tDelay < 10f) tDelay = 10f;
                if (isNode2Start) {
                    _node2.spline.isSpecialEndNodeIsStartDelay = true;
                    _node2.spline.specialEndNodeDelayStart = tDelay;
                    _node2.spline.specialEndNodeDelayStartResult = _node1.spline.road.RoadWidth();
                    _node2.spline.specialEndNodeStartOtherSpline = _node1.spline;
                }
                else {
                    _node2.spline.isSpecialEndNodeIsEndDelay = true;
                    _node2.spline.specialEndNodeDelayEnd = tDelay;
                    _node2.spline.specialEndNodeDelayEndResult = _node1.spline.road.RoadWidth();
                    _node2.spline.specialEndNodeEndOtherSpline = _node1.spline;
                }
            }
            else if (_node2.spline.road.laneAmount > _node1.spline.road.laneAmount) {
                _node1.isSpecialRoadConnPrimary = true;
                NodeCreated1.isSpecialRoadConnPrimary = true;
                if (_node1.spline.road.laneAmount == 4) xWidth *= 2f;
                tDelay = (_node2.spline.road.laneAmount - _node1.spline.road.laneAmount) * xWidth;
                if (tDelay < 10f) tDelay = 10f;
                if (isNode1Start) {
                    _node1.spline.isSpecialEndNodeIsStartDelay = true;
                    _node1.spline.specialEndNodeDelayStart = tDelay;
                    _node1.spline.specialEndNodeDelayStartResult = _node2.spline.road.RoadWidth();
                    _node1.spline.specialEndNodeStartOtherSpline = _node2.spline;
                }
                else {
                    _node1.spline.isSpecialEndNodeIsEndDelay = true;
                    _node1.spline.specialEndNodeDelayEnd = tDelay;
                    _node1.spline.specialEndNodeDelayEndResult = _node2.spline.road.RoadWidth();
                    _node1.spline.specialEndNodeEndOtherSpline = _node2.spline;
                }
            }
            else {
                _node1.isSpecialRoadConnPrimary = true;
                NodeCreated1.isSpecialRoadConnPrimary = true;
                tDelay = 0f;
                _node1.spline.isSpecialEndNodeIsEndDelay = false;
                _node1.spline.isSpecialEndNodeIsStartDelay = false;
                _node1.spline.specialEndNodeDelayEnd = tDelay;
                _node1.spline.specialEndNodeDelayEndResult = _node2.spline.road.RoadWidth();
                _node1.spline.specialEndNodeEndOtherSpline = _node2.spline;
                _node2.spline.isSpecialEndNodeIsEndDelay = false;
                _node2.spline.isSpecialEndNodeIsStartDelay = false;
                _node2.spline.specialEndNodeDelayEnd = tDelay;
                _node2.spline.specialEndNodeDelayEndResult = _node1.spline.road.RoadWidth();
                _node2.spline.specialEndNodeEndOtherSpline = _node1.spline;
            }

            _node1.specialNodeCounterpart = NodeCreated1;
            _node2.specialNodeCounterpart = NodeCreated2;
            NodeCreated1.specialNodeCounterpartMaster = _node1;
            NodeCreated2.specialNodeCounterpartMaster = _node2;


            NodeCreated1.ToggleHideFlags(true);
            NodeCreated2.ToggleHideFlags(true);

            var OrigNodes = new SplineN[2];
            OrigNodes[0] = _node1;
            OrigNodes[1] = _node2;
            _node1.originalConnectionNodes = OrigNodes;
            _node2.originalConnectionNodes = OrigNodes;

            //tNode1.spline.Setup_Trigger();
            //if(tNode1.spline != tNode2.spline)
            //{
            //  tNode2.spline.Setup_Trigger();
            //}


            // Schedule update
            if (_node1 != null && _node2 != null) {
                if (_node1.spline != _node2.spline) {
                    _node1.spline.road.PiggyBacks = new SplineC[1];
                    _node1.spline.road.PiggyBacks[0] = _node2.spline;
                }

                _node1.spline.road.isUpdateRequired = true;
            }

            previewSpline.isDrawingGizmos = false;
            spline.previewSpline.isDrawingGizmos = false;

#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif
        }

        #endregion

        #region "Vars"

        [FormerlySerializedAs("mNodes")] public List<SplineN> nodes = new List<SplineN>();

        [FormerlySerializedAs("mSplineRoot")] public GameObject splineRoot;

        [FormerlySerializedAs("tRoad")] public Road road;

        public float distance = -1f;

        [FormerlySerializedAs("CachedPoints")] public Vector3[] cachedPoints;

        private const float cachedPointsSeperation = 1f;

        //Editor preview splines for add and insert:
        [FormerlySerializedAs("PreviewSpline")]
        public SplineF previewSpline;

        [FormerlySerializedAs("PreviewSplineInsert")]
        public SplineI previewSplineInsert;


        #region "Nav data Vars"

        public float RoadWidth;
        public int Lanes;

        [FormerlySerializedAs("id_connected")] public List<int> connectedIDs;

        public int id;

        //Unique ID
        [FormerlySerializedAs("UID")] public string uID;

        public List<KeyValuePair<float, float>> BridgeParams;
        public List<KeyValuePair<float, float>> TunnelParams;
        public List<KeyValuePair<float, float>> HeightHistory;
        public int[] RoadDefKeysArray;
        public float[] RoadDefValuesArray;

        [FormerlySerializedAs("EditorOnly_LastNode_TimeSinceStartup")]
        public double editorOnlyLastNodeTimeSinceStartup = -1f;

        #endregion


        #region "Vars for intersections"

        private const float metersToCheckNoTurnLane = 75f;
        private const float metersToCheckNoTurnLaneSQ = 5625f;
        private const float metersToCheckTurnLane = 125f;
        private const float metersToCheckTurnLaneSQ = 15625f;
        private const float metersToCheckBothTurnLane = 125f;
        private const float metersToCheckBothTurnLaneSQ = 15625f;
        private const bool isUsingSQ = true;

        #endregion


        #region "Road connections and 3-way intersections"

        [FormerlySerializedAs("bSpecialStartControlNode")]
        public bool isSpecialStartControlNode;

        [FormerlySerializedAs("bSpecialEndControlNode")]
        public bool isSpecialEndControlNode;

        [FormerlySerializedAs("bSpecialEndNode_IsStart_Delay")]
        public bool isSpecialEndNodeIsStartDelay;

        [FormerlySerializedAs("bSpecialEndNode_IsEnd_Delay")]
        public bool isSpecialEndNodeIsEndDelay;

        [FormerlySerializedAs("SpecialEndNodeDelay_Start")]
        public float specialEndNodeDelayStart = 10f;

        [FormerlySerializedAs("SpecialEndNodeDelay_Start_Result")]
        public float specialEndNodeDelayStartResult = 10f;

        [FormerlySerializedAs("SpecialEndNodeDelay_End")]
        public float specialEndNodeDelayEnd = 10f;

        [FormerlySerializedAs("SpecialEndNodeDelay_End_Result")]
        public float specialEndNodeDelayEndResult = 10f;

        [FormerlySerializedAs("SpecialEndNode_Start_OtherSpline")]
        public SplineC specialEndNodeStartOtherSpline;

        [FormerlySerializedAs("SpecialEndNode_End_OtherSpline")]
        public SplineC specialEndNodeEndOtherSpline;

        #endregion


        public Vector2 RoadV0;
        public Vector2 RoadV1;
        public Vector2 RoadV2;
        public Vector2 RoadV3;

        #endregion


        #region "Setup"

        public void TriggerSetup() {
            if (!road)
                if (splineRoot != null)
                    road = splineRoot.transform.parent.transform.gameObject.GetComponent<Road>();
            if (road) road.UpdateRoad();
        }


        /// <summary> Setup Spline values </summary>
        public void Setup() {
            //Setup unique ID:
            RootUtils.SetupUniqueIdentifier(ref uID);

            //Set spline root:
            splineRoot = transform.gameObject;

            //Create spline nodes:
            var rawNodes = splineRoot.GetComponentsInChildren<SplineN>();
            var nodeList = new List<SplineN>();
            var rawNodesLength = rawNodes.Length;
            if (rawNodesLength == 0) return;


            // Stores nodes positions in pos and adds them to nodeList
            for (var i = 0; i < rawNodesLength; i++)
                if (rawNodes[i] != null) {
                    rawNodes[i].pos = rawNodes[i].transform.position;
                    nodeList.Add(rawNodes[i]);
                }


            nodeList.Sort(CompareListByID);
            //tList.Sort(delegate(SplineC i1, Item i2) { return i1.name.CompareTo(i2.name); });
            rawNodes = nodeList.ToArray();
            nodeList = null;
            SetupNodes(ref rawNodes);

            //Setup spline length, if more than 1 node:
            if (GetNodeCount() > 1)
                //RootUtils.StartProfiling(road, "SplineSetupLength");
                SetupSplineLength();
            //RootUtils.EndProfiling(road);
            else if (GetNodeCount() == 1) nodes[0].time = 0f;


            //Setup preview spline:
            if (previewSpline == null) {
                previewSpline = splineRoot.AddComponent<SplineF>();
                previewSpline.spline = this;
            }

            //Setup preview spline for insertion mode:
            if (previewSplineInsert == null) {
                previewSplineInsert = splineRoot.AddComponent<SplineI>();
                previewSplineInsert.spline = this;
            }


            var nodesCount = nodes.Count;
            SplineN splineNode = null;
            var nodePositions = new Vector3[nodesCount + 1];


            for (var i = 0; i < nodesCount; i++) {
                splineNode = nodes[i];
                splineNode.idOnSpline = i;
                splineNode.isEndPoint = false;
                nodePositions[i] = splineNode.pos;
            }

            nodePositions[nodePositions.Length - 1] = new Vector3(0f, 0f, 0f);


            previewSpline.Setup(ref nodePositions);

            RenameNodes();


            #region "Setup bridge params"

            if (BridgeParams != null) BridgeParams.Clear();
            BridgeParams = new List<KeyValuePair<float, float>>();
            KeyValuePair<float, float> KVP;

            #endregion


            //Setup tunnel params:
            if (TunnelParams != null) TunnelParams.Clear();
            TunnelParams = new List<KeyValuePair<float, float>>();

            if (nodesCount > 1) {
                if (isSpecialStartControlNode)
                    nodes[1].isEndPoint = true;
                else
                    nodes[0].isEndPoint = true;
                if (isSpecialEndControlNode)
                    nodes[nodesCount - 2].isEndPoint = true;
                else
                    nodes[nodesCount - 1].isEndPoint = true;
            }
            else if (nodesCount == 1) {
                nodes[0].isEndPoint = true;
                distance = 1;
            }

            var splineStart = -1f;
            var splineEnd = -1f;

            if (nodesCount > 1)
                for (var i = 0; i < nodesCount; i++) {
                    splineNode = nodes[i];

                    //Bridges:
                    splineStart = -1f;
                    splineEnd = -1f;
                    if (splineNode.isBridgeStart && !splineNode.isTunnelStart) {
                        splineStart = splineNode.time;
                        for (var j = i; j < nodesCount; j++)
                            if (nodes[j].isBridgeEnd) {
                                splineEnd = nodes[j].time;
                                break;
                            }

                        if (splineEnd > 0f || RootUtils.IsApproximately(splineEnd, 0f, 0.0001f)) {
                            KVP = new KeyValuePair<float, float>(splineStart, splineEnd);
                            BridgeParams.Add(KVP);
                        }
                    }

                    //Tunnels:
                    splineStart = -1f;
                    splineEnd = -1f;
                    if (!splineNode.isBridgeStart && splineNode.isTunnelStart) {
                        splineStart = splineNode.time;
                        for (var j = i; j < nodesCount; j++)
                            if (nodes[j].isTunnelEnd) {
                                splineEnd = nodes[j].time;
                                break;
                            }

                        if (splineEnd > 0f || RootUtils.IsApproximately(splineEnd, 0f, 0.0001f)) {
                            KVP = new KeyValuePair<float, float>(splineStart, splineEnd);
                            TunnelParams.Add(KVP);
                        }
                    }

                    splineNode.SetGradePercent(nodesCount);
                    //splineNode.isEndPoint = false;
                    splineNode.tangent = GetSplineValue(nodes[i].time, true);
                    if (i < nodesCount - 1) {
                        splineNode.nextTime = nodes[i + 1].time;
                        splineNode.nextTan = nodes[i + 1].tangent;
                    }
                }
            else if (nodesCount == 1) nodes[0].tangent = default;

            //Get bounds of road system:
            var maxEffects = new float[3];
            maxEffects[0] = road.matchHeightsDistance;
            maxEffects[1] = road.clearDetailsDistance;
            maxEffects[2] = road.clearTreesDistance;

            //Add min/max clear diff to bound checks
            var maxEffectDistance = Mathf.Max(maxEffects) * 2f;

            nodesCount = GetNodeCount();
            var minMaxX = new float[nodesCount];
            var minMaxZ = new float[nodesCount];

            for (var i = 0; i < nodesCount; i++) {
                minMaxX[i] = nodes[i].pos.x;
                minMaxZ[i] = nodes[i].pos.z;
            }

            // calculate the biggest and lowest x and z positions
            var minX = Mathf.Min(minMaxX) - maxEffectDistance;
            var maxX = Mathf.Max(minMaxX) + maxEffectDistance;
            var minZ = Mathf.Min(minMaxZ) - maxEffectDistance;
            var maxZ = Mathf.Max(minMaxZ) + maxEffectDistance;

            RoadV0 = new Vector3(minX, minZ);
            RoadV1 = new Vector3(maxX, minZ);
            RoadV2 = new Vector3(maxX, maxZ);
            RoadV3 = new Vector3(minX, maxZ);
        }


        /// <summary> Renames the Nodes to their id on the Spline </summary>
        private void RenameNodes() {
            var nodesCount = nodes.Count;
            SplineN node;
            for (var i = 0; i < nodesCount; i++) {
                node = nodes[i];
                node.name = "Node" + node.idOnSpline;
                node.transform.gameObject.name = node.name;
                node.editorDisplayString = road.transform.name + "-" + node.name;
            }
        }


        private int CompareListByID(SplineN _i1, SplineN _i2) {
            return _i1.idOnSpline.CompareTo(_i2.idOnSpline);
        }


        /// <summary> Setup all nodes of this road </summary>
        private void SetupNodes(ref SplineN[] _rawNodes) {
            //Process nodes:
            var i = 0;
            var nodes = new List<SplineN>();
            var rawNodesLength = _rawNodes.Length;
            for (i = 0; i < rawNodesLength; i++) nodes.Add(_rawNodes[i]);

            this.nodes.Clear();
            this.nodes = new List<SplineN>();
            SplineN node;
            float step;
            Quaternion rot;
            Vector3 positionChange;
            var isClosed = false;
            step = isClosed ? 1f / nodes.Count : 1f / (nodes.Count - 1);
            var nodesCount = nodes.Count;
            for (i = 0; i < nodesCount; i++) {
                node = nodes[i];


                // Calculate the rotation to the next node
                rot = Quaternion.identity;
                if (i != nodes.Count - 1) {
                    positionChange = nodes[i + 1].transform.position - node.transform.position;
                    if (positionChange == Vector3.zero)
                        rot = Quaternion.identity;
                    else
                        rot = Quaternion.LookRotation(positionChange, transform.up);
                }
                else if (isClosed) {
                    rot = Quaternion.LookRotation(nodes[0].transform.position - node.transform.position, transform.up);
                }
                else {
                    rot = Quaternion.identity;
                }

                node.Setup(node.transform.position, rot, new Vector2(0f, 1f), step * i, node.transform.gameObject.name);
                RootUtils.SetupUniqueIdentifier(ref node.uID);
                this.nodes.Add(node);
            }

            nodes = null;
            _rawNodes = null;
        }


        /// <summary> Calculates distance between node for accuracy </summary>
        private void SetupSplineLength() {
            var nodeCount = nodes.Count;

            //First lets get the general distance, node to node:
            nodes[0].time = 0f;
            nodes[nodeCount - 1].time = 1f;
            var node1 = new Vector3(0f, 0f, 0f);
            var node2 = new Vector3(0f, 0f, 0f);
            var roadLength = 0f;
            var roadLengthOriginal = 0f;

            // Calculate accumulated distance between nodes
            for (var j = 0; j < nodeCount; j++) {
                node2 = nodes[j].pos;
                if (j > 0) roadLength += Vector3.Distance(node1, node2);
                node1 = node2;
            }


            roadLengthOriginal = roadLength;
            roadLength = roadLength * 1.05f;
            var step = 0.5f / roadLength;

            //Get a slightly more accurate portrayal of the time:
            var nodeTime = 0f;
            for (var j = 0; j < nodeCount - 1; j++) {
                node2 = nodes[j].pos;
                if (j > 0) {
                    nodeTime += Vector3.Distance(node1, node2) / roadLengthOriginal;
                    nodes[j].time = nodeTime;
                }

                node1 = node2;
            }

            //Using general distance and calculated step, get an accurate distance:
            var splineDistance = 0f;
            var prevPos = nodes[0].pos;
            var currentPos = new Vector3(0f, 0f, 0f);

            prevPos = GetSplineValue(0f);
            for (var i = 0f; i < 1f; i += step) {
                currentPos = GetSplineValue(i);
                splineDistance += Vector3.Distance(currentPos, prevPos);
                prevPos = currentPos;
            }

            distance = splineDistance;


            //Now get fine distance between nodes:
            var newTotalDistance = 0f;
            step = 0.5f / distance;
            SplineN prevNode = null;
            SplineN currentNode = null;
            prevPos = GetSplineValue(0f);
            for (var j = 1; j < nodeCount - 1; j++) {
                prevNode = nodes[j - 1];
                currentNode = nodes[j];

                if (j == 1) prevPos = GetSplineValue(prevNode.time);
                splineDistance = 0.00001f;

                for (var i = prevNode.time; i < currentNode.time; i += step) {
                    currentPos = GetSplineValue(i);
                    if (!float.IsNaN(currentPos.x)) {
                        if (float.IsNaN(prevPos.x)) prevPos = currentPos;
                        splineDistance += Vector3.Distance(currentPos, prevPos);
                        prevPos = currentPos;
                    }
                }

                currentNode.tempSegmentTime = splineDistance / distance;
                newTotalDistance += splineDistance;
                currentNode.dist = newTotalDistance;
            }


            nodes[0].dist = 0f;
            prevNode = nodes[nodeCount - 2];
            currentNode = nodes[nodeCount - 1];
            splineDistance = 0.00001f;
            for (var i = prevNode.time; i < currentNode.time; i += step) {
                currentPos = GetSplineValue(i);
                if (!float.IsNaN(currentPos.x)) {
                    if (float.IsNaN(prevPos.x)) prevPos = currentPos;
                    splineDistance += Vector3.Distance(currentPos, prevPos);
                    prevPos = currentPos;
                }
            }

            currentNode.tempSegmentTime = splineDistance / distance;
            newTotalDistance += splineDistance;
            currentNode.dist = newTotalDistance;

            distance = newTotalDistance;

            // Set node data
            SplineN node;
            nodeTime = 0f;
            for (var j = 1; j < nodeCount - 1; j++) {
                node = nodes[j];
                nodeTime += node.tempSegmentTime;
                node.oldTime = node.time;
                node.time = nodeTime;
                node.tangent = GetSplineValueSkipOpt(node.time, true);
                node.transform.rotation = Quaternion.LookRotation(node.tangent);
            }

            nodes[0].tangent = GetSplineValueSkipOpt(0f, true);
            nodes[nodeCount - 1].tangent = GetSplineValueSkipOpt(1f, true);

            nodes[0].dist = 0f;

            step = distance / cachedPointsSeperation;
            var ArrayCount = (int)Mathf.Floor(step) + 2;
            cachedPoints = new Vector3[ArrayCount];
            step = cachedPointsSeperation / distance;
            for (var j = 1; j < ArrayCount - 1; j++) cachedPoints[j] = GetSplineValue(step * j);
            cachedPoints[0] = nodes[0].pos;
            cachedPoints[ArrayCount - 1] = nodes[nodeCount - 1].pos;

            RoadDefCalcs();
        }

        #endregion


        #region "Road definition cache and translation"

        private void RoadDefCalcs() {
            var tMod = Mathf.Lerp(0.05f, 0.2f, distance / 9000f);
            var step = tMod / distance;
            var currentPos = GetSplineValue(0f);
            var prevPos = currentPos;
            var tempDistanceModMax = road.roadDefinition - step;
            var tempDistanceMod = 0f;
            var tempTotalDistance = 0f;
            var tempDistanceHolder = 0f;
            if (RoadDefKeysArray != null) RoadDefKeysArray = null;
            if (RoadDefValuesArray != null) RoadDefValuesArray = null;

            var RoadDefKeys = new List<int>();
            var RoadDefValues = new List<float>();

            RoadDefKeys.Add(0);
            RoadDefValues.Add(0f);

            for (var index = 0f; index < 1f; index += step) {
                currentPos = GetSplineValue(index);
                tempDistanceHolder = Vector3.Distance(currentPos, prevPos);
                tempTotalDistance += tempDistanceHolder;
                tempDistanceMod += tempDistanceHolder;
                if (tempDistanceMod > tempDistanceModMax) {
                    tempDistanceMod = 0f;
                    RoadDefKeys.Add(TranslateParamToInt(index));
                    RoadDefValues.Add(tempTotalDistance);
                }

                prevPos = currentPos;
            }

            RoadDefKeysArray = RoadDefKeys.ToArray();
            RoadDefValuesArray = RoadDefValues.ToArray();
        }


        public int TranslateParamToInt(float _value) {
            return Mathf.Clamp((int)(_value * 10000000f), 0, 10000000);
        }


        public float TranslateInverseParamToFloat(int _value) {
            return Mathf.Clamp(_value / 10000000f, 0f, 1f);
        }


        private void GetClosestRoadDefKeys(float _x, out int _lo, out int _hi, out int _loIndex, out int _hiIndex) {
            var x = TranslateParamToInt(_x);
            _lo = -1;
            _hi = RoadDefKeysArray.Length - 1;

            var mid = -1;

            while (_hi - _lo > 1) {
                mid = Mathf.RoundToInt((_lo + _hi) / 2);
                if (RoadDefKeysArray[mid] <= x)
                    _lo = mid;
                else
                    _hi = mid;
            }

            if (RoadDefKeysArray[_lo] == x) _hi = _lo;
            //		if(hi > RoadDefKeysArray.Length-1){ hi = RoadDefKeysArray.Length-1; }

            _loIndex = _lo;
            _hiIndex = _hi;
            _lo = RoadDefKeysArray[_lo];
            _hi = RoadDefKeysArray[_hi];
        }


        public int GetClosestRoadDefIndex(float _x, bool _isRoundUp = false, bool _isRoundDown = false) {
            int lo, hi, loIndex, hiIndex;

            GetClosestRoadDefKeys(_x, out lo, out hi, out loIndex, out hiIndex);

            var x = TranslateParamToInt(_x);

            if (_isRoundUp) return hiIndex;
            if (_isRoundDown) return loIndex;

            if (x - lo > hi - x)
                return hiIndex;
            return loIndex;
        }


        private void GetClosestRoadDefValues(float _x, out float _loF, out float _hiF, out int _loIndex,
            out int _hiIndex) {
            var lo = -1;
            var hi = RoadDefValuesArray.Length - 1;
            var mid = -1;

            while (hi - lo > 1) {
                mid = Mathf.RoundToInt((lo + hi) / 2);
                if (RoadDefValuesArray[mid] < _x || RootUtils.IsApproximately(RoadDefValuesArray[mid], _x, 0.02f))
                    lo = mid;
                else
                    hi = mid;
            }

            if (RootUtils.IsApproximately(RoadDefValuesArray[lo], _x, 0.02f)) hi = lo;

            _loIndex = lo;
            _hiIndex = hi;
            _loF = RoadDefValuesArray[lo];
            _hiF = RoadDefValuesArray[hi];
        }


        public int GetClosestRoadDefValuesIndex(float _x, bool _isRoundUp = false, bool _isRoundDown = false) {
            float lo, hi;
            int loIndex, hiIndex;

            GetClosestRoadDefValues(_x, out lo, out hi, out loIndex, out hiIndex);

            if (_isRoundUp) return hiIndex;
            if (_isRoundDown) return loIndex;

            if (_x - lo > hi - _x)
                return hiIndex;
            return loIndex;
        }


        public float TranslateDistBasedToParam(float _dist) {
            var tIndex = GetClosestRoadDefValuesIndex(_dist);
            var tKey = TranslateInverseParamToFloat(RoadDefKeysArray[tIndex]);
            var tCount = RoadDefKeysArray.Length;
            var kDist = RoadDefValuesArray[tIndex];

            if (tIndex < tCount - 1)
                if (_dist > kDist) {
                    var NextValue = RoadDefValuesArray[tIndex + 1];
                    var tDiff1 = (_dist - kDist) / (NextValue - kDist);
                    tKey += tDiff1 * (TranslateInverseParamToFloat(RoadDefKeysArray[tIndex + 1]) - tKey);
                }

            if (tIndex > 0)
                if (_dist < kDist) {
                    var PrevValue = RoadDefValuesArray[tIndex - 1];
                    var tDiff1 = (_dist - PrevValue) / (kDist - PrevValue);
                    tKey -= tDiff1 * (tKey - TranslateInverseParamToFloat(RoadDefKeysArray[tIndex - 1]));
                }

            return tKey;
        }


        public float TranslateParamToDist(float _param) {
            var tIndex = GetClosestRoadDefIndex(_param);
            var tKey = TranslateInverseParamToFloat(RoadDefKeysArray[tIndex]);
            var tCount = RoadDefKeysArray.Length;
            var kDist = RoadDefValuesArray[tIndex];
            var xDiff = kDist;

            if (tIndex < tCount - 1)
                if (_param > tKey) {
                    var NextValue = TranslateInverseParamToFloat(RoadDefKeysArray[tIndex + 1]);
                    var tDiff1 = (_param - tKey) / (NextValue - tKey);
                    xDiff += tDiff1 * (RoadDefValuesArray[tIndex + 1] - kDist);
                }

            if (tIndex > 0)
                if (_param < tKey) {
                    var PrevValue = TranslateInverseParamToFloat(RoadDefKeysArray[tIndex - 1]);
                    var tDiff1 = 1f - (_param - PrevValue) / (tKey - PrevValue);
                    xDiff -= tDiff1 * (kDist - RoadDefValuesArray[tIndex - 1]);
                }

            return xDiff;
        }

        #endregion


        #region "Hermite math"

        /// <summary> Gets the spline value. </summary>
        /// <param name='_value'> The relevant param (0-1) of the spline. </param>
        /// <param name='_isTangent'> True for is tangent, false (default) for vector3 position. </param>
        public Vector3 GetSplineValue(float _value, bool _isTangent = false) {
            int index;
            var idx = -1;


            if (nodes.Count == 0) return default;
            if (nodes.Count == 1) return nodes[0].pos;

            // This Code was outcommented, but it takes care about values above and below 0f and 1f and clamping them.
            // This Fixes the Bug descripted by embeddedt/RoadArchitect/issues/4 
            if (RootUtils.IsApproximately(_value, 0f, 0.00001f)) {
                if (_isTangent)
                    return nodes[0].tangent;
                return nodes[0].pos;
            }

            if (RootUtils.IsApproximately(_value, 1f, 0.00001f) || _value > 1f) {
                if (_isTangent)
                    return nodes[nodes.Count - 1].tangent;
                return nodes[nodes.Count - 1].pos;
            }

            RootUtils.IsApproximately(_value, 1f, 0.00001f);

            for (index = 0; index < nodes.Count; index++) {
                if (index == nodes.Count - 1) {
                    idx = index - 1;
                    break;
                }

                if (nodes[index].time > _value) {
                    idx = index - 1;
                    break;
                }
            }

            if (idx < 0) idx = 0;

            var param = (_value - nodes[idx].time) / (nodes[idx + 1].time - nodes[idx].time);
            param = RootUtils.Ease(param, nodes[idx].easeIO.x, nodes[idx].easeIO.y);
            return GetHermiteInternal(idx, param, _isTangent);
        }


        /// <summary> Return position and tangent in Vector3 of spline progress </summary>
        public void GetSplineValueBoth(float _value, out Vector3 _vect1, out Vector3 _vect2) {
            int index;
            var idx = -1;
            var nodeCount = GetNodeCount();

            if (_value < 0f) _value = 0f;
            if (_value > 1f) _value = 1f;


            if (nodeCount == 0) {
                _vect1 = default;
                _vect2 = default;
                return;
            }

            if (nodeCount == 1) {
                if (nodes[0]) {
                    _vect1 = nodes[0].pos;
                    _vect2 = default;
                }
                else {
                    _vect1 = default;
                    _vect2 = default;
                }

                return;
            }


            // This Code was outcommented, but it takes care about values above and below 0f and 1f and clamping them.
            // This code needs to be reevealuated if this isn't taken care of by the function above this one. GetSplineValue() 
            // part of embeddedt/RoadArchitect/issues/4 ?
            if (RootUtils.IsApproximately(_value, 1f, 0.0001f)) {
                _vect1 = nodes[nodes.Count - 1].pos;
                _vect2 = nodes[nodes.Count - 1].tangent;
                return;
            }

            if (RootUtils.IsApproximately(_value, 0f, 0.0001f)) {
                _vect1 = nodes[0].pos;
                _vect2 = nodes[0].tangent;
                return;
            }
            // Do Note: This does prevent EdgeObjects from being placed before or after
            // 0f/1f on the Spline, but also causes the EdgeObject to be placed at the same Position at the End of the Spline.

            for (index = 1; index < nodeCount; index++) {
                if (index == nodeCount - 1) {
                    idx = index - 1;
                    break;
                }

                if (nodes[index].time > _value) {
                    idx = index - 1;
                    break;
                }
            }

            if (idx < 0) idx = 0;

            var param = (_value - nodes[idx].time) / (nodes[idx + 1].time - nodes[idx].time);
            param = RootUtils.Ease(param, nodes[idx].easeIO.x, nodes[idx].easeIO.y);

            _vect1 = GetHermiteInternal(idx, param);
            _vect2 = GetHermiteInternal(idx, param, true);
        }


        public Vector3 GetSplineValueSkipOpt(float _value, bool _isTangent = false) {
            int index;
            var idx = -1;

            if (nodes.Count == 0) return default;
            if (nodes.Count == 1) return nodes[0].pos;


            //		if(RootUtils.IsApproximately(f,0f,0.00001f)){
            //			if(_isTangent){
            //				return mNodes[0].tangent;
            //			}else{
            //				return mNodes[0].pos;	
            //			}
            //		}else
            //		if(RootUtils.IsApproximately(f,1f,0.00001f) || f > 1f){
            //			if(_isTangent){
            //				return mNodes[mNodes.Count-1].tangent;
            //			}else{
            //				return mNodes[mNodes.Count-1].pos;	
            //			}
            //		}else{
            for (index = 1; index < nodes.Count; index++) {
                if (index == nodes.Count - 1) {
                    idx = index - 1;
                    break;
                }

                if (nodes[index].time > _value) {
                    idx = index - 1;
                    break;
                }
            }

            if (idx < 0) idx = 0;
            //			if(b && RootUtils.IsApproximately(f,1f,0.00001f)){
            //				idx = mNodes.Count-2;
            //			}
            //		}

            var param = (_value - nodes[idx].time) / (nodes[idx + 1].time - nodes[idx].time);
            param = RootUtils.Ease(param, nodes[idx].easeIO.x, nodes[idx].easeIO.y);
            return GetHermiteInternal(idx, param, _isTangent);
        }


        public float GetClosestParam(Vector3 _vect, bool _is20cmPrecision = false, bool _is1MeterPrecision = false) {
            return GetClosestParamDo(ref _vect, _is20cmPrecision, _is1MeterPrecision);
        }


        private float GetClosestParamDo(ref Vector3 _vect, bool _is20cmPrecision, bool _is1MeterPrecision) {
            //5m to 1m	
            var Step1 = cachedPointsSeperation / distance;
            //20 cm	
            var Step2 = Step1 * 0.2f;
            //8 cm 
            var Step3 = Step2 * 0.4f;
            //2 cm
            var Step4 = Step3 * 0.25f;


            var tMin = 0f;
            var tMax = 1f;

            // Why is Best value set to -1f? at init
            var BestValue = -1f;
            var MaxStretch = 0.9f;
            var BestVect_p = new Vector3(0f, 0f, 0f);
            var BestVect_n = new Vector3(0f, 0f, 0f);

            if (nodes.Count == 0) return 0f;
            if (nodes.Count == 1) return 1f;

            //Step 1: 1m 
            BestValue = GetClosestPointHelper(ref _vect, Step1, BestValue, tMin, tMax, ref BestVect_p, ref BestVect_n,
                true);
            if (_is1MeterPrecision) return BestValue;

            //Step 2: 20cm 
            tMin = BestValue - Step1 * MaxStretch;
            tMax = BestValue + Step1 * MaxStretch;
            BestValue = GetClosestPointHelper(ref _vect, Step2, BestValue, tMin, tMax, ref BestVect_p, ref BestVect_n);
            if (_is20cmPrecision) return BestValue;

            //Step 3: 8cm 
            tMin = BestValue - Step2 * MaxStretch;
            tMax = BestValue + Step2 * MaxStretch;
            BestValue = GetClosestPointHelper(ref _vect, Step3, BestValue, tMin, tMax, ref BestVect_p, ref BestVect_n);

            //Step 4: 2cm
            tMin = BestValue - Step3 * MaxStretch;
            tMax = BestValue + Step3 * MaxStretch;
            BestValue = GetClosestPointHelper(ref _vect, Step4, BestValue, tMin, tMax, ref BestVect_p, ref BestVect_n);

            return BestValue;
        }


        private float GetClosestPointHelper(ref Vector3 _vect, float _step, float _bestValue, float _min, float _max,
            ref Vector3 _bestVectP, ref Vector3 _bestVectN, bool _isMeterCache = false) {
            var mDistance = 5000f;
            var tDistance = 0f;
            var cVect = new Vector3(0f, 0f, 0f);
            var pVect = new Vector3(0f, 0f, 0f);
            var isFirstLoopHappened = false;
            var isSetBestValue = false;

            //Get lean for tmin/tmax:
            if (GetClosetPointMinMaxDirection(ref _vect, ref _bestVectP, ref _bestVectN))
                _max = _bestValue;
            else
                _min = _bestValue;

            _min = Mathf.Clamp(_min, 0f, 1f);
            _max = Mathf.Clamp(_max, 0f, 1f);

            if (_isMeterCache) {
                var CachedIndex = -1;
                var Step1 = 10;

                var CachedPointsLength = cachedPoints.Length;
                for (var j = 0; j < CachedPointsLength; j += Step1) {
                    cVect = cachedPoints[j];
                    tDistance = Vector3.Distance(_vect, cVect);
                    if (tDistance < mDistance) {
                        mDistance = tDistance;
                        CachedIndex = j;
                    }
                }

                var jStart = CachedIndex - Step1;
                if (jStart < 50) jStart = 0;
                var jEnd = CachedIndex + Step1;
                if (jEnd > CachedPointsLength) jEnd = CachedPointsLength;
                for (var j = jStart; j < jEnd; j++) {
                    cVect = cachedPoints[j];
                    if (isSetBestValue) {
                        _bestVectN = cVect;
                        isSetBestValue = false;
                    }

                    tDistance = Vector3.Distance(_vect, cVect);
                    if (tDistance < mDistance) {
                        mDistance = tDistance;
                        if (!isFirstLoopHappened)
                            _bestVectP = cVect;
                        else
                            _bestVectP = pVect;
                        CachedIndex = j;
                        isSetBestValue = true;
                        isFirstLoopHappened = true;
                    }

                    pVect = cVect;
                }

                _bestValue = CachedIndex / distance;
            }
            else {
                for (var index = _min; index <= _max; index += _step) {
                    cVect = GetSplineValue(index);
                    if (isSetBestValue) {
                        _bestVectN = cVect;
                        isSetBestValue = false;
                    }

                    tDistance = Vector3.Distance(_vect, cVect);
                    if (tDistance < mDistance) {
                        mDistance = tDistance;
                        _bestValue = index;
                        if (!isFirstLoopHappened)
                            _bestVectP = cVect;
                        else
                            _bestVectP = pVect;

                        isSetBestValue = true;
                        isFirstLoopHappened = true;
                    }

                    pVect = cVect;
                }
            }

            if (isSetBestValue) _bestVectN = cVect;

            //Debug.Log ("returning: " + BestValue + " tmin:" + tMin + " tmax:" + tMax);
            return _bestValue;
        }


        //Returns true for tmin lean:
        private bool GetClosetPointMinMaxDirection(ref Vector3 _vect, ref Vector3 _bestVectP, ref Vector3 _bestVectN) {
            var Distance1 = Vector3.Distance(_vect, _bestVectP);
            var Distance2 = Vector3.Distance(_vect, _bestVectN);

            if (Distance1 < Distance2)
                //tMin lean
                return true;
            return false;
        }


        private Vector3 GetHermiteInternal(int _i, double _t, bool _isTangent = false) {
            double t2, t3;
            float BL0, BL1, BL2, BL3, tension;

            if (!_isTangent) {
                t2 = _t * _t;
                t3 = t2 * _t;
            }
            else {
                t2 = _t * _t;
                _t = _t * 2.0;
                t2 = t2 * 3.0;
                //Prevent compiler error.
                t3 = 0;
            }

            //Vectors:
            var P0 = nodes[NGI(_i, NI[0])].pos;
            var P1 = nodes[NGI(_i, NI[1])].pos;
            var P2 = nodes[NGI(_i, NI[2])].pos;
            var P3 = nodes[NGI(_i, NI[3])].pos;

            //Tension:
            tension = 0.5f;

            //Tangents:
            var xVect1 = (P1 - P2) * tension;
            var xVect2 = (P3 - P0) * tension;
            var tMaxMag = road.magnitudeThreshold;

            if (Vector3.Distance(P1, P3) > tMaxMag) {
                if (xVect1.magnitude > tMaxMag) xVect1 = Vector3.ClampMagnitude(xVect1, tMaxMag);
                if (xVect2.magnitude > tMaxMag) xVect2 = Vector3.ClampMagnitude(xVect2, tMaxMag);
            }
            else if (Vector3.Distance(P0, P2) > tMaxMag) {
                if (xVect1.magnitude > tMaxMag) xVect1 = Vector3.ClampMagnitude(xVect1, tMaxMag);
                if (xVect2.magnitude > tMaxMag) xVect2 = Vector3.ClampMagnitude(xVect2, tMaxMag);
            }


            if (!_isTangent) {
                BL0 = (float)(CM[0] * t3 + CM[1] * t2 + CM[2] * _t + CM[3]);
                BL1 = (float)(CM[4] * t3 + CM[5] * t2 + CM[6] * _t + CM[7]);
                BL2 = (float)(CM[8] * t3 + CM[9] * t2 + CM[10] * _t + CM[11]);
                BL3 = (float)(CM[12] * t3 + CM[13] * t2 + CM[14] * _t + CM[15]);
            }
            else {
                BL0 = (float)(CM[0] * t2 + CM[1] * _t + CM[2]);
                BL1 = (float)(CM[4] * t2 + CM[5] * _t + CM[6]);
                BL2 = (float)(CM[8] * t2 + CM[9] * _t + CM[10]);
                BL3 = (float)(CM[12] * t2 + CM[13] * _t + CM[14]);
            }

            var tVect = BL0 * P0 + BL1 * P1 + BL2 * xVect1 + BL3 * xVect2;

            if (!_isTangent)
                if (tVect.y < 0f)
                    tVect.y = 0f;

            return tVect;
        }


        private static readonly double[] CM =
        {
            2.0, -3.0, 0.0, 1.0,
            -2.0, 3.0, 0.0, 0.0,
            1.0, -2.0, 1.0, 0.0,
            1.0, -1.0, 0.0, 0.0
        };


        private static readonly int[] NI = { 0, 1, -1, 2 };


        private int NGI(int _i, int _o) {
            var NGITI = _i + _o;
            //		if(bClosed){
            //			return (NGITI % mNodes.Count + mNodes.Count) % mNodes.Count;
            //		}else{
            return Mathf.Clamp(NGITI, 0, nodes.Count - 1);
            //		}
        }

        #endregion


        #region "Gizmos"

        //private const bool isDrawingGizmos = true;
        private float GizmoDrawMeters = 1f;


        private void OnDrawGizmosSelected() {
            //		if(!isDrawingGizmos){ return; }
            if (nodes == null || nodes.Count < 2) return;
            if (transform == null) return;
            var DistanceFromCam = Vector3.SqrMagnitude(Camera.current.transform.position - nodes[0].transform.position);

            if (DistanceFromCam > 16777216f)
                return;
            if (DistanceFromCam > 4194304f)
                GizmoDrawMeters = 16f;
            else if (DistanceFromCam > 1048576f)
                GizmoDrawMeters = 8f;
            else if (DistanceFromCam > 262144f)
                GizmoDrawMeters = 4f;
            else if (DistanceFromCam > 65536)
                GizmoDrawMeters = 1f;
            else if (DistanceFromCam > 16384f)
                GizmoDrawMeters = 0.5f;
            else
                GizmoDrawMeters = 0.1f;

            var prevPos = nodes[0].pos;
            var tempVect = new Vector3(0f, 0f, 0f);
            var step = GizmoDrawMeters / distance;
            step = Mathf.Clamp(step, 0f, 1f);
            Gizmos.color = new Color(1f, 0f, 0f, 1f);
            var index = 0f;
            Vector3 cPos;
            var tCheck = 0f;
            var camPos = Camera.current.transform.position;
            for (index = 0f; index <= 1f; index += step) {
                tCheck += step;
                cPos = GetSplineValue(index);

                if (tCheck > 0.1f) {
                    DistanceFromCam = Vector3.SqrMagnitude(camPos - cPos);
                    if (DistanceFromCam > 16777216f)
                        return;
                    if (DistanceFromCam > 4194304f)
                        GizmoDrawMeters = 16f;
                    else if (DistanceFromCam > 1048576f)
                        GizmoDrawMeters = 10f;
                    else if (DistanceFromCam > 262144f)
                        GizmoDrawMeters = 4f;
                    else if (DistanceFromCam > 65536)
                        GizmoDrawMeters = 1f;
                    else if (DistanceFromCam > 16384f)
                        GizmoDrawMeters = 0.5f;
                    else
                        GizmoDrawMeters = 0.1f;
                    step = GizmoDrawMeters / distance;
                    step = Mathf.Clamp(step, 0f, 1f);
                    tCheck = 0f;
                }

                Gizmos.DrawLine(prevPos + tempVect, cPos + tempVect);
                prevPos = cPos;
                if (index + step > 1f) {
                    cPos = GetSplineValue(1f);
                    Gizmos.DrawLine(prevPos + tempVect, cPos + tempVect);
                }
            }
        }

        #endregion


        #region "Intersections"

        public bool IsNearIntersection(ref Vector3 _pos, ref float _result) {
            var mCount = GetNodeCount();
            SplineN tNode;
            var MetersToCheck = 75f * (road.laneWidth / 5f * (road.laneWidth / 5f));
            float tDist;
            for (var index = 0; index < mCount; index++) {
                tNode = nodes[index];
                if (tNode.isIntersection) {
                    tNode.intersection.height = tNode.pos.y;

                    if (isUsingSQ) tDist = Vector3.SqrMagnitude(_pos - tNode.pos);
                    //				else{
                    //					tDist = Vector3.Distance(tPos,tNode.pos);
                    //				}

                    if (tNode.intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane) {
                        if (isUsingSQ)
                            MetersToCheck = metersToCheckNoTurnLaneSQ * (road.laneWidth / 5f * (road.laneWidth / 5f));
                        //					else{
                        //						MetersToCheck = MetersToCheck_NoTurnLane;
                        //					}
                    }
                    else if (tNode.intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane) {
                        if (isUsingSQ) {
                            MetersToCheck = metersToCheckTurnLaneSQ * (road.laneWidth / 5f * (road.laneWidth / 5f));
                            ;
                        }
                        //					else{
                        //						MetersToCheck = MetersToCheck_TurnLane;
                        //					}
                    }
                    else if (tNode.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes) {
                        if (isUsingSQ) {
                            MetersToCheck = metersToCheckBothTurnLaneSQ * (road.laneWidth / 5f * (road.laneWidth / 5f));
                            ;
                        }
                        //					else{
                        //						MetersToCheck = MetersToCheck_BothTurnLane;
                        //					}
                    }

                    MetersToCheck *= 0.8f;
                    if (road.laneAmount == 4)
                        MetersToCheck *= 1.25f;
                    else if (road.laneAmount == 6) MetersToCheck *= 1.35f;

                    if (tDist <= MetersToCheck) {
                        _result = tNode.pos.y;
                        return true;
                    }
                }
            }

            _result = _pos.y;
            return false;
        }


        public float IntersectionStrength(ref Vector3 _pos, ref float _result, ref RoadIntersection _inter,
            ref bool _isPast, ref float _p, ref SplineN _node) {
            var nodeCount = GetNodeCount();
            float tDist;
            SplineN tNode;

            var MetersToCheck = 75f * (road.laneWidth / 5f * (road.laneWidth / 5f));

            for (var index = 0; index < nodeCount; index++) {
                tNode = nodes[index];
                if (tNode.isIntersection) {
                    tNode.intersection.height = tNode.pos.y;
                    SplineN xNode;
                    if (isUsingSQ) tDist = Vector3.SqrMagnitude(_pos - tNode.pos);
                    //				else{
                    //					tDist = Vector3.Distance(tPos,tNode.pos);
                    //				}

                    if (tNode.intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane) {
                        if (isUsingSQ)
                            MetersToCheck = metersToCheckNoTurnLaneSQ * (road.laneWidth / 5f * (road.laneWidth / 5f));
                        //					else{
                        //						MetersToCheck = MetersToCheck_NoTurnLane;
                        //					}
                    }
                    else if (tNode.intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane) {
                        if (isUsingSQ)
                            MetersToCheck = metersToCheckTurnLaneSQ * (road.laneWidth / 5f * (road.laneWidth / 5f));
                        //					else{
                        //						MetersToCheck = MetersToCheck_TurnLane;
                        //					}
                    }
                    else if (tNode.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes) {
                        if (isUsingSQ)
                            MetersToCheck = metersToCheckBothTurnLaneSQ * (road.laneWidth / 5f * (road.laneWidth / 5f));
                        //					else{
                        //						MetersToCheck = MetersToCheck_BothTurnLane;
                        //					}
                    }

                    if (road.laneAmount == 4)
                        MetersToCheck *= 1.25f;
                    else if (road.laneAmount == 6) MetersToCheck *= 1.35f;

                    if (tDist <= MetersToCheck) {
                        if (tNode.intersection.isSameSpline) {
                            if (tNode.intersection.node1.uID != tNode.uID)
                                xNode = tNode.intersection.node1;
                            else
                                xNode = tNode.intersection.node2;

                            var P1 = tNode.time - _p;
                            if (P1 < 0f) P1 *= -1f;
                            var P2 = xNode.time - _p;
                            if (P2 < 0f) P2 *= -1f;

                            if (P1 > P2) {
                                if (_p > xNode.time)
                                    _isPast = true;
                                else
                                    _isPast = false;
                                _node = xNode;
                            }
                            else {
                                if (_p > tNode.time)
                                    _isPast = true;
                                else
                                    _isPast = false;
                                _node = tNode;
                            }
                        }
                        else {
                            if (_p > tNode.time)
                                _isPast = true;
                            else
                                _isPast = false;
                            _node = tNode;
                        }


                        if (isUsingSQ) {
                            tDist = Mathf.Sqrt(tDist);
                            MetersToCheck = Mathf.Sqrt(MetersToCheck);
                        }

                        _inter = tNode.intersection;
                        _result = tNode.pos.y + 0.1f;
                        tDist = 1f - tDist / MetersToCheck;
                        tDist = Mathf.Pow(tDist, 3f) * 5f;
                        if (tDist > 1f)
                            tDist = 1f;
                        if (tDist < 0f)
                            tDist = 0f;
                        return tDist;
                    }
                }
            }

            _result = _pos.y;
            return 0f;
        }


        public float IntersectionStrengthNext(Vector3 _pos) {
            var result = 0f;
            RoadIntersection intersection = null;
            var isPast = false;
            var p = 0f;
            SplineN node = null;
            return IntersectionStrength(ref _pos, ref result, ref intersection, ref isPast, ref p, ref node);
        }


        public bool IntersectionIsPast(ref float _p, ref SplineN _node) {
            //int mCount = GetNodeCount();
            //bool bIsPast;
            //SplineN tNode = null;
            //for(int i=0;i<mCount;i++)
            //{
            //	tNode = mNodes[i];
            //	if(tNode.bIsIntersection)
            //{
            //		float P1 = tNode.roadIntersection.Node1.tTime - p;
            //      if(P1 < 0f)
            //      {
            //          P1 *= -1f;
            //      }
            //	    float P2 = tNode.roadIntersection.Node2.tTime - p;
            //      if(P2 < 0f)
            //      {
            //          P2 *= -1f;
            //      }
            //				
            //		if(P1 > P2)
            //      {
            //			if(p > tNode.roadIntersection.Node2.tTime)
            //          {
            //				bIsPast = true;	
            //			}
            //          else
            //          {
            //				bIsPast = false;	
            //			}
            //		}
            //      else
            //      {
            //			if(p > tNode.roadIntersection.Node1.tTime)
            //          {
            //				bIsPast = true;	
            //			}
            //          else
            //          {
            //				bIsPast = false;	
            //			}
            //		}
            //		return bIsPast;
            //	}
            //}
            //return false;


            if (_p < _node.time)
                return false;
            return true;
        }


        private void DestroyIntersection(SplineN _node) {
            if (_node == null) return;

            if (_node.isEndPoint) {
                if (_node.idOnSpline == 1 && _node.spline.nodes[0].isSpecialEndNodeIsStart) {
                    DestroyImmediate(_node.spline.nodes[0].transform.gameObject);
                    _node.spline.isSpecialStartControlNode = false;
                }
                else if (_node.idOnSpline == _node.spline.GetNodeCount() - 2 &&
                         _node.spline.nodes[_node.spline.GetNodeCount() - 1].isSpecialEndNodeIsEnd) {
                    DestroyImmediate(_node.spline.nodes[_node.spline.GetNodeCount() - 1].transform.gameObject);
                    _node.spline.isSpecialEndControlNode = false;
                }
            }

            _node.isIntersection = false;
            _node.isSpecialIntersection = false;


            if (_node.intersectionOtherNode != null) {
                if (_node.intersectionOtherNode.isEndPoint) {
                    if (_node.intersectionOtherNode.idOnSpline == 1 &&
                        _node.intersectionOtherNode.spline.nodes[0].isSpecialEndNodeIsStart) {
                        DestroyImmediate(_node.intersectionOtherNode.spline.nodes[0].transform.gameObject);
                        _node.intersectionOtherNode.spline.isSpecialStartControlNode = false;
                    }
                    else if (_node.intersectionOtherNode.idOnSpline ==
                        _node.intersectionOtherNode.spline.GetNodeCount() - 2 && _node.intersectionOtherNode.spline
                            .nodes[_node.intersectionOtherNode.spline.GetNodeCount() - 1].isSpecialEndNodeIsEnd) {
                        DestroyImmediate(_node.intersectionOtherNode.spline
                            .nodes[_node.intersectionOtherNode.spline.GetNodeCount() - 1].transform.gameObject);
                        _node.intersectionOtherNode.spline.isSpecialEndControlNode = false;
                    }
                }

                _node.intersectionOtherNode.isIntersection = false;
                _node.intersectionOtherNode.isSpecialIntersection = false;


                _node.spline.road.isUpdatingSpline = true;
                if (_node.spline != _node.intersectionOtherNode.spline)
                    _node.intersectionOtherNode.spline.road.isUpdatingSpline = true;
            }
        }

        #endregion


        #region "Bridges"

        public bool IsInBridge(float _p) {
            KeyValuePair<float, float> KVP;
            if (BridgeParams == null) return false;
            var cCount = BridgeParams.Count;
            if (cCount < 1) return false;
            for (var index = 0; index < cCount; index++) {
                KVP = BridgeParams[index];
                if (RootUtils.IsApproximately(KVP.Key, _p, 0.0001f) ||
                    RootUtils.IsApproximately(KVP.Value, _p, 0.0001f)) return true;
                if (_p > KVP.Key && _p < KVP.Value) return true;
            }

            return false;
        }


        public float BridgeUpComing(float _p) {
            var tDist = 20f / distance;
            var OrigP = _p;
            _p += tDist;
            KeyValuePair<float, float> KVP;
            if (BridgeParams == null) return 1f;
            var cCount = BridgeParams.Count;
            if (cCount < 1) return 1f;
            for (var index = 0; index < cCount; index++) {
                KVP = BridgeParams[index];

                if (RootUtils.IsApproximately(KVP.Key, _p, 0.0001f) ||
                    RootUtils.IsApproximately(KVP.Value, _p, 0.0001f)) return (KVP.Key - OrigP) / tDist;
                if (_p > KVP.Key && _p < KVP.Value) return (KVP.Key - OrigP) / tDist;
            }

            return 1f;
        }


        public bool IsInBridgeTerrain(float _p) {
            KeyValuePair<float, float> KVP;
            if (BridgeParams == null) return false;
            var cCount = BridgeParams.Count;
            if (cCount < 1) return false;
            for (var index = 0; index < cCount; index++) {
                KVP = BridgeParams[index];
                if (RootUtils.IsApproximately(KVP.Key + 10f / distance, _p, 0.0001f) ||
                    RootUtils.IsApproximately(KVP.Value - 10f / distance, _p, 0.0001f)) return true;
                if (_p > KVP.Key + 10f / distance && _p < KVP.Value - 10f / distance) return true;
            }

            return false;
        }


        public float GetBridgeEnd(float _p) {
            KeyValuePair<float, float> KVP;
            if (BridgeParams == null) return -1f;
            var cCount = BridgeParams.Count;
            if (cCount < 1) return -1f;
            for (var index = 0; index < cCount; index++) {
                KVP = BridgeParams[index];
                if (_p >= KVP.Key && _p <= KVP.Value) return KVP.Value;
            }

            return -1f;
        }

        #endregion


        #region "Tunnels"

        public bool IsInTunnel(float _p) {
            KeyValuePair<float, float> KVP;
            if (TunnelParams == null) return false;
            var cCount = TunnelParams.Count;
            if (cCount < 1) return false;
            for (var index = 0; index < cCount; index++) {
                KVP = TunnelParams[index];
                if (RootUtils.IsApproximately(KVP.Key, _p, 0.0001f) ||
                    RootUtils.IsApproximately(KVP.Value, _p, 0.0001f)) return true;
                if (_p > KVP.Key && _p < KVP.Value) return true;
            }

            return false;
        }


        public float TunnelUpComing(float _p) {
            var tDist = 20f / distance;
            var OrigP = _p;
            _p += tDist;
            KeyValuePair<float, float> KVP;
            if (TunnelParams == null) return 1f;
            var cCount = TunnelParams.Count;
            if (cCount < 1) return 1f;
            for (var index = 0; index < cCount; index++) {
                KVP = TunnelParams[index];

                if (RootUtils.IsApproximately(KVP.Key, _p, 0.0001f) ||
                    RootUtils.IsApproximately(KVP.Value, _p, 0.0001f)) return (KVP.Key - OrigP) / tDist;
                if (_p > KVP.Key && _p < KVP.Value) return (KVP.Key - OrigP) / tDist;
            }

            return 1f;
        }


        public bool IsInTunnelTerrain(float _p) {
            KeyValuePair<float, float> KVP;
            if (TunnelParams == null) return false;
            var cCount = TunnelParams.Count;
            if (cCount < 1) return false;
            for (var index = 0; index < cCount; index++) {
                KVP = TunnelParams[index];
                if (RootUtils.IsApproximately(KVP.Key + 10f / distance, _p, 0.0001f) ||
                    RootUtils.IsApproximately(KVP.Value - 10f / distance, _p, 0.0001f)) return true;
                if (_p > KVP.Key + 10f / distance && _p < KVP.Value - 10f / distance) return true;
            }

            return false;
        }


        public float GetTunnelEnd(float _p) {
            KeyValuePair<float, float> KVP;
            if (TunnelParams == null) return -1f;
            var cCount = TunnelParams.Count;
            if (cCount < 1) return -1f;
            for (var index = 0; index < cCount; index++) {
                KVP = TunnelParams[index];
                if (_p >= KVP.Key && _p <= KVP.Value) return KVP.Value;
            }

            return -1f;
        }

        #endregion


        #region "General Util"

        public int GetNodeCount() {
            return nodes.Count;
        }


        public int GetNodeCountNonNull() {
            var nodeCount = GetNodeCount();
            var validCount = 0;

            for (var index = 0; index < nodeCount; index++)
                if (nodes[index] != null) {
                    validCount += 1;
                    if (nodes[index].isIntersection && nodes[index].intersection == null)
                        DestroyIntersection(nodes[index]);
                }

            return validCount;
        }


        /// <summary> Checks if the Nodes are null </summary>
        public bool CheckInvalidNodeCount() {
            var nodeCount = GetNodeCount();
            var validCount = 0;


            for (var index = 0; index < nodeCount; index++)
                if (nodes[index] != null) {
                    validCount += 1;
                    if (nodes[index].isIntersection && nodes[index].intersection == null)
                        DestroyIntersection(nodes[index]);
                }


            if (validCount != nodeCount)
                return true;
            return false;
        }


        /// <summary> Get node from spline progress </summary>
        public SplineN GetCurrentNode(float _p) {
            var nodeCount = GetNodeCount();
            SplineN node = null;

            for (var index = 0; index < nodeCount; index++) {
                node = nodes[index];
                if (node.time > _p) {
                    node = nodes[index - 1];
                    return node;
                }
            }

            return node;
        }


        public SplineN GetLastLegitimateNode() {
            var nodeCount = GetNodeCount();
            SplineN node = null;
            for (var index = nodeCount - 1; index >= 0; index--) {
                node = nodes[index];
                if (node.IsLegitimate()) return node;
            }

            return null;
        }


        public SplineN GetLastNodeAll() {
            var startIndex = GetNodeCount() - 1;
            SplineN node = null;

            var i = startIndex;
            while (i >= 0) {
                if (i <= nodes.Count - 1) {
                    node = nodes[i];
                    if (node != null) return node;
                }

                i -= 1;
            }

            return null;
        }


        public SplineN GetPrevLegitimateNode(int _index) {
            try {
                SplineN node = null;
                for (var index = _index - 1; index >= 0; index--) {
                    node = nodes[index];
                    if (node.IsLegitimateGrade()) return node;
                }

                return null;
            }
            catch {
                return null;
            }
        }


        public SplineN GetNextLegitimateNode(int _index) {
            SplineN node = null;
            var nodeCount = GetNodeCount();
            for (var index = _index + 1; index < nodeCount; index++) {
                node = nodes[index];
                if (node.IsLegitimateGrade()) return node;
            }

            return null;
        }


        /// <summary> Removes materials on all nodes </summary>
        public void ClearAllRoadCuts() {
            var nodeCount = GetNodeCount();
            for (var index = 0; index < nodeCount; index++) nodes[index].ClearCuts();
        }


        public void ResetNavigationData() {
            connectedIDs = null;
            connectedIDs = new List<int>();
        }

        #endregion

    }
}