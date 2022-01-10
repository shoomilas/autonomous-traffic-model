#region "Imports"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#endregion


namespace RoadArchitect {
    public class RoadIntersection : MonoBehaviour {


        public enum IntersectionTypeEnum {
            ThreeWay,
            FourWay
        }

        public enum iStopTypeEnum {
            StopSign_AllWay,
            TrafficLight1,
            None,
            TrafficLight2
        }

        public enum LightTypeEnum {
            Timed,
            Sensors
        }

        public enum RoadTypeEnum {
            NoTurnLane,
            TurnLane,
            BothTurnLanes
        }


        [FormerlySerializedAs("BoundsRect")] private Construction2DRect boundsRect;


        #region "Gizmos"

        private void OnDrawGizmos() {
            if (!isDrawingGizmo) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position + new Vector3(0f, 5f, 0f), new Vector3(2f, 11f, 2f));
        }

        #endregion


        // A struct may be better and faster
        public class CornerPositionMaker {
            [FormerlySerializedAs("DirectionFromCenter")]
            public Vector3 directionFromCenter;

            public Vector3 position;
            public Quaternion rotation;
        }

        #region "Vars"

        [FormerlySerializedAs("Node1")] public SplineN node1;

        [FormerlySerializedAs("Node2")] public SplineN node2;

        [FormerlySerializedAs("Node1UID")] public string node1uID;

        [FormerlySerializedAs("Node2UID")] public string node2uID;

        //Unique ID
        [FormerlySerializedAs("UID")] protected string uID;

        [FormerlySerializedAs("tName")] public string intersectionName = "";

        [FormerlySerializedAs("bSameSpline")] public bool isSameSpline;

        [FormerlySerializedAs("bDrawGizmo")] public bool isDrawingGizmo = true;

        [FormerlySerializedAs("bFlipped")] public bool isFlipped;

        [FormerlySerializedAs("bUseDefaultMaterials")]
        public bool isUsingDefaultMaterials = true;

        [FormerlySerializedAs("opt_AutoUpdateIntersections")]
        public bool isAutoUpdatingIntersection = true;

        [FormerlySerializedAs("bNode2B_LeftTurnLane")]
        public bool isNode2BLeftTurnLane = true;

        [FormerlySerializedAs("bNode2B_RightTurnLane")]
        public bool isNode2BRightTurnLane = true;

        [FormerlySerializedAs("bNode2F_LeftTurnLane")]
        public bool isNode2FLeftTurnLane = true;

        [FormerlySerializedAs("bNode2F_RightTurnLane")]
        public bool isNode2FRightTurnLane = true;

        [FormerlySerializedAs("bFirstSpecial_First")]
        public bool isFirstSpecialFirst;

        [FormerlySerializedAs("bFirstSpecial_Last")]
        public bool isFirstSpecialLast;

        [FormerlySerializedAs("bSecondSpecial_First")]
        public bool isSecondSpecialFirst;

        [FormerlySerializedAs("bSecondSpecial_Last")]
        public bool isSecondSpecialLast;

        [FormerlySerializedAs("CornerRR1")] public bool isCornerRR1Enabled;

        [FormerlySerializedAs("CornerRR2")] public bool isCornerRR2Enabled;

        [FormerlySerializedAs("CornerRL1")] public bool isCornerRL1Enabled;

        [FormerlySerializedAs("CornerRL2")] public bool isCornerRL2Enabled;

        [FormerlySerializedAs("CornerLR1")] public bool isCornerLR1Enabled;

        [FormerlySerializedAs("CornerLR2")] public bool isCornerLR2Enabled;

        [FormerlySerializedAs("CornerLL1")] public bool isCornerLL1Enabled;

        [FormerlySerializedAs("CornerLL2")] public bool isCornerLL2Enabled;

        #region "Marker materials"

        [FormerlySerializedAs("MarkerCenter1")]
        public Material markerCenter1;

        [FormerlySerializedAs("MarkerCenter2")]
        public Material markerCenter2;

        [FormerlySerializedAs("MarkerCenter3")]
        public Material markerCenter3;

        [FormerlySerializedAs("MarkerExt_Stretch1")]
        public Material markerExtStretch1;

        [FormerlySerializedAs("MarkerExt_Stretch2")]
        public Material markerExtStretch2;

        [FormerlySerializedAs("MarkerExt_Stretch3")]
        public Material markerExtStretch3;

        [FormerlySerializedAs("MarkerExt_Tiled1")]
        public Material markerExtTiled1;

        [FormerlySerializedAs("MarkerExt_Tiled2")]
        public Material markerExtTiled2;

        [FormerlySerializedAs("MarkerExt_Tiled3")]
        public Material markerExtTiled3;

        #endregion

        #region "Lane materials"

        [FormerlySerializedAs("Lane0Mat1")] public Material lane0Mat1;

        [FormerlySerializedAs("Lane0Mat2")] public Material lane0Mat2;

        [FormerlySerializedAs("Lane1Mat1")] public Material lane1Mat1;

        [FormerlySerializedAs("Lane1Mat2")] public Material lane1Mat2;

        [FormerlySerializedAs("Lane2Mat1")] public Material lane2Mat1;

        [FormerlySerializedAs("Lane2Mat2")] public Material lane2Mat2;

        [FormerlySerializedAs("Lane3Mat1")] public Material lane3Mat1;

        [FormerlySerializedAs("Lane3Mat2")] public Material lane3Mat2;

        [FormerlySerializedAs("Lane1Mat1_Disabled")]
        public Material lane1Mat1Disabled;

        [FormerlySerializedAs("Lane1Mat2_Disabled")]
        public Material lane1Mat2Disabled;

        [FormerlySerializedAs("Lane1Mat1_DisabledActive")]
        public Material lane1Mat1DisabledActive;

        [FormerlySerializedAs("Lane1Mat2_DisabledActive")]
        public Material lane1Mat2DisabledActive;

        [FormerlySerializedAs("Lane2Mat1_Disabled")]
        public Material lane2Mat1Disabled;

        [FormerlySerializedAs("Lane2Mat2_Disabled")]
        public Material lane2Mat2Disabled;

        [FormerlySerializedAs("Lane2Mat1_DisabledActive")]
        public Material lane2Mat1DisabledActive;

        [FormerlySerializedAs("Lane2Mat2_DisabledActive")]
        public Material lane2Mat2DisabledActive;

        [FormerlySerializedAs("Lane2Mat1_DisabledActiveR")]
        public Material lane2Mat1DisabledActiveR;

        [FormerlySerializedAs("Lane2Mat2_DisabledActiveR")]
        public Material lane2Mat2DisabledActiveR;

        [FormerlySerializedAs("Lane3Mat1_Disabled")]
        public Material lane3Mat1Disabled;

        [FormerlySerializedAs("Lane3Mat2_Disabled")]
        public Material lane3Mat2Disabled;

        #endregion

        //Width of the largest of road connected
        /// <summary> 10 * 1.25f = intersectionWidth; Never written, only read </summary>
        [FormerlySerializedAs("IntersectionWidth")]
        public int intersectionWidth = 10;

        /// <summary> Amount of lanes from road of node1 </summary>
        [FormerlySerializedAs("Lanes")] public int lanesAmount;

        [FormerlySerializedAs("IgnoreSide")] public int ignoreSide = -1;

        [FormerlySerializedAs("IgnoreCorner")] public int ignoreCorner = -1;

        [FormerlySerializedAs("iType")] public IntersectionTypeEnum intersectionType = IntersectionTypeEnum.FourWay;

        [FormerlySerializedAs("iStopType")] public iStopTypeEnum intersectionStopType = iStopTypeEnum.StopSign_AllWay;

        [FormerlySerializedAs("rType")] public RoadTypeEnum roadType = RoadTypeEnum.NoTurnLane;

        [FormerlySerializedAs("lType")] public LightTypeEnum lightType = LightTypeEnum.Timed;

        #region "CalculationData"

        [FormerlySerializedAs("CornerPoints")] public CornerPositionMaker[] cornerPoints;

        [FormerlySerializedAs("CornerRR")] public Vector3 cornerRR;

        [FormerlySerializedAs("CornerRR_Outer")]
        public Vector3 cornerRROuter;

        [FormerlySerializedAs("CornerRR_RampOuter")]
        public Vector3 cornerRRRampOuter;

        [FormerlySerializedAs("CornerRL")] public Vector3 cornerRL;

        [FormerlySerializedAs("CornerRL_Outer")]
        public Vector3 cornerRLOuter;

        [FormerlySerializedAs("CornerRL_RampOuter")]
        public Vector3 cornerRLRampOuter;

        [FormerlySerializedAs("CornerLR")] public Vector3 cornerLR;

        [FormerlySerializedAs("CornerLR_Outer")]
        public Vector3 cornerLROuter;

        [FormerlySerializedAs("CornerLR_RampOuter")]
        public Vector3 cornerLRRampOuter;

        [FormerlySerializedAs("CornerLL")] public Vector3 cornerLL;

        [FormerlySerializedAs("CornerLL_Outer")]
        public Vector3 cornerLLOuter;

        [FormerlySerializedAs("CornerLL_RampOuter")]
        public Vector3 cornerLLRampOuter;

        [FormerlySerializedAs("CornerRR_2D")] public Vector2 cornerRR2D;

        [FormerlySerializedAs("CornerRL_2D")] public Vector2 cornerRL2D;

        [FormerlySerializedAs("CornerLR_2D")] public Vector2 cornerLR2D;

        [FormerlySerializedAs("CornerLL_2D")] public Vector2 cornerLL2D;

        [FormerlySerializedAs("fCornerLR_CornerRR")]
        public Vector3[] cornerLRCornerRR;

        [FormerlySerializedAs("fCornerLL_CornerRL")]
        public Vector3[] cornerLLCornerRL;

        [FormerlySerializedAs("fCornerLL_CornerLR")]
        public Vector3[] cornerLLCornerLR;

        [FormerlySerializedAs("fCornerRL_CornerRR")]
        public Vector3[] cornerRLCornerRR;

        #endregion

        [FormerlySerializedAs("GradeMod")] public float gradeMod = 0.375f;

        [FormerlySerializedAs("GradeModNegative")]
        public float gradeModNegative = 0.75f;

        [FormerlySerializedAs("ScalingSense")] public float scalingSense = 3f;

        [FormerlySerializedAs("OddAngle")] public float oddAngle;

        [FormerlySerializedAs("EvenAngle")] public float evenAngle;

        [FormerlySerializedAs("MaxInterDistance")]
        public float maxInterDistance;

        [FormerlySerializedAs("MaxInterDistanceSQ")]
        public float maxInterDistanceSQ;

        [FormerlySerializedAs("Height")] public float height = 50000f;

        [FormerlySerializedAs("SignHeight")] public float signHeight = -2000f;

        #region "Traffic Light Vars"

        [FormerlySerializedAs("bLightsEnabled")]
        public bool isLightsEnabled = true;

        [FormerlySerializedAs("bRegularPoleAlignment")]
        public bool isRegularPoleAlignment = true;

        [FormerlySerializedAs("bTrafficPoleStreetLight")]
        public bool isTrafficPoleStreetLight = true;

        [FormerlySerializedAs("bTrafficLightGray")]
        public bool isTrafficLightGray;

        [FormerlySerializedAs("bLeftTurnYieldOnGreen")]
        public bool isLeftTurnYieldOnGreen = true;

        [FormerlySerializedAs("StreetLight_Range")]
        public float streetLightRange = 30f;

        [FormerlySerializedAs("StreetLight_Intensity")]
        public float streetLightIntensity = 1f;

        [FormerlySerializedAs("StreetLight_Color")]
        public Color streetLightColor = new Color(1f, 0.7451f, 0.27451f, 1f);

        [FormerlySerializedAs("FixedTimeIndex")]
        private int fixedTimeIndex;

        [FormerlySerializedAs("LightsRR")] public TrafficLightController lightsRR;

        [FormerlySerializedAs("LightsRL")] public TrafficLightController lightsRL;

        [FormerlySerializedAs("LightsLL")] public TrafficLightController lightsLL;

        [FormerlySerializedAs("LightsLR")] public TrafficLightController lightsLR;

        [FormerlySerializedAs("opt_FixedTime_RegularLightLength")]
        public float fixedTimeRegularLightLength = 10f;

        [FormerlySerializedAs("opt_FixedTime_LeftTurnLightLength")]
        public float fixedTimeLeftTurnLightLength = 5f;

        [FormerlySerializedAs("opt_FixedTime_AllRedLightLength")]
        public float fixedTimeAllRedLightLength = 1f;

        [FormerlySerializedAs("opt_FixedTime_YellowLightLength")]
        public float fixedTimeYellowLightLength = 2f;

        [FormerlySerializedAs("FixedTimeSequenceList")]
        public List<TrafficLightSequence> fixedTimeSequenceList;

        #endregion

        #endregion


        #region "Setup"

        /// <summary> Links nodes and intersection </summary>
        public void Setup(SplineN _node1, SplineN _node2) {
            if (_node1.spline == _node2.spline) isSameSpline = true;

            if (isSameSpline) {
                if (_node1.idOnSpline < _node2.idOnSpline) {
                    node1 = _node1;
                    node2 = _node2;
                }
                else {
                    node1 = _node2;
                    node2 = _node1;
                }
            }
            else {
                node1 = _node1;
                node2 = _node2;
            }

            node1.intersectionOtherNode = node2;
            node2.intersectionOtherNode = node1;

            node1.ToggleHideFlags(true);
            node2.ToggleHideFlags(true);

            node1uID = node1.uID;
            node2uID = node2.uID;
            node1.isIntersection = true;
            node2.isIntersection = true;
            node1.intersection = this;
            node2.intersection = this;
        }


        /// <summary> Deletes Meshes based on road name and the centermarker if _node is intersections node1 </summary>
        public void DeleteRelevantChildren(SplineN _node, string _string) {
            Transform transformChild;
            var childCount = transform.childCount;
            for (var index = childCount - 1; index >= 0; index--) {
                transformChild = transform.GetChild(index);
                if (transformChild.name.ToLower().Contains(_string.ToLower()))
                    DestroyImmediate(transformChild.gameObject);
                else if (_node == node1)
                    if (transformChild.name.ToLower().Contains("centermarkers"))
                        DestroyImmediate(transformChild.gameObject);
            }
        }

        #endregion


        #region "Utility"

        /// <summary> Attach other spline to PiggyBacks if not same spline and setup </summary>
        public void UpdateRoads() {
            if (!isSameSpline) {
                var piggys = new SplineC[1];
                piggys[0] = node2.spline;
                node1.spline.road.PiggyBacks = piggys;
                node1.spline.TriggerSetup();
            }
            else {
                node1.spline.TriggerSetup();
            }
        }


        public void ConstructBoundsRect() {
            boundsRect = null;
            boundsRect = new Construction2DRect(new Vector2(cornerRR.x, cornerRR.z),
                new Vector2(cornerRL.x, cornerRL.z), new Vector2(cornerLR.x, cornerLR.z),
                new Vector2(cornerLL.x, cornerLL.z));
        }


        /// <summary> Creates boundsRect if null and returns true if _vector is inside the rect </summary>
        public bool Contains(ref Vector3 _vector) {
            var vector2D = new Vector2(_vector.x, _vector.z);
            if (boundsRect == null) ConstructBoundsRect();
            return boundsRect.Contains(ref vector2D);
        }


        private bool ContainsLineOld(Vector3 _vector1, Vector3 _vector2, int _lineDef = 30) {
            var MaxDef = _lineDef;
            float MaxDefF = MaxDef;

            var tVects = new Vector3[MaxDef];

            tVects[0] = _vector1;
            var mMod = 0f;
            var fcounter = 1f;
            for (var index = 1; index < MaxDef - 1; index++) {
                mMod = fcounter / MaxDefF;
                tVects[index] = (_vector2 - _vector1) * mMod + _vector1;
                fcounter += 1f;
            }

            tVects[MaxDef - 1] = _vector2;

            var xVect = default(Vector2);
            for (var index = 0; index < MaxDef; index++) {
                xVect = new Vector2(tVects[index].x, tVects[index].z);
                if (boundsRect.Contains(ref xVect)) return true;
            }

            return false;
        }


        /// <summary> Returns true when the Vectors or the line between them are inside the intersection </summary>
        public bool ContainsLine(Vector3 _vector1, Vector3 _vector2) {
            var tVectStart = new Vector2(_vector1.x, _vector1.z);
            var tVectEnd = new Vector2(_vector2.x, _vector2.z);
            var bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerRR2D, ref cornerRL2D);
            if (bIntersects) return true;
            bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerRL2D, ref cornerLL2D);
            if (bIntersects) return true;
            bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerLL2D, ref cornerLR2D);
            if (bIntersects) return true;
            bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerLR2D, ref cornerRR2D);
            return bIntersects;
        }


        // Returns true if the lines intersect, otherwise false. If the lines
        // intersect, intersectionPoint holds the intersection point.
        private static bool Intersects2D(ref Vector2 _line1S, ref Vector2 _line1E, ref Vector2 _line2S,
            ref Vector2 _line2E) {
            float firstLineSlopeX, firstLineSlopeY, secondLineSlopeX, secondLineSlopeY;

            firstLineSlopeX = _line1E.x - _line1S.x;
            firstLineSlopeY = _line1E.y - _line1S.y;

            secondLineSlopeX = _line2E.x - _line2S.x;
            secondLineSlopeY = _line2E.y - _line2S.y;

            float s, t;
            s = (-firstLineSlopeY * (_line1S.x - _line2S.x) + firstLineSlopeX * (_line1S.y - _line2S.y)) /
                (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
            t = (secondLineSlopeX * (_line1S.y - _line2S.y) - secondLineSlopeY * (_line1S.x - _line2S.x)) /
                (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1) return true;
            // No collision
            return false;
        }

        #endregion


        #region "Traffic light controlling"

        private void Start() {
            lightsRR.Setup(isLeftTurnYieldOnGreen);
            lightsRL.Setup(isLeftTurnYieldOnGreen);
            lightsLL.Setup(isLeftTurnYieldOnGreen);
            lightsLR.Setup(isLeftTurnYieldOnGreen);
            if (lightType == LightTypeEnum.Timed) {
                CreateFixedSequence();
                FixedTimeIncrement();
            }
        }


        private void CreateFixedSequence() {
            TrafficLightSequence SequenceMaker = null;
            fixedTimeSequenceList = new List<TrafficLightSequence>();
            if (roadType != RoadTypeEnum.NoTurnLane) {
                SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.LeftTurn,
                    TrafficLightController.iLightSubStatusEnum.Green, fixedTimeLeftTurnLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }

            if (roadType != RoadTypeEnum.NoTurnLane) {
                SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.LeftTurn,
                    TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }

            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Red,
                TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Regular,
                TrafficLightController.iLightSubStatusEnum.Green, fixedTimeRegularLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Regular,
                TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Red,
                TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);

            if (roadType != RoadTypeEnum.NoTurnLane) {
                SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.LeftTurn,
                    TrafficLightController.iLightSubStatusEnum.Green, fixedTimeLeftTurnLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }

            if (roadType != RoadTypeEnum.NoTurnLane) {
                SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.LeftTurn,
                    TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }

            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Red,
                TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.Regular,
                TrafficLightController.iLightSubStatusEnum.Green, fixedTimeRegularLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.Regular,
                TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.Red,
                TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
        }


        private IEnumerator TrafficLightFixedUpdate(float _time) {
            yield return new WaitForSeconds(_time);
            FixedTimeIncrement();
        }


        /// <summary> Executes the next traffic light sequence and shedules a new increment </summary>
        private void FixedTimeIncrement() {
            var SequenceMaker = fixedTimeSequenceList[fixedTimeIndex];
            fixedTimeIndex += 1;
            if (fixedTimeIndex > fixedTimeSequenceList.Count - 1) fixedTimeIndex = 0;

            TrafficLightController lights1 = null;
            TrafficLightController lights2 = null;

            TrafficLightController lightsOuter1 = null;
            TrafficLightController lightsOuter2 = null;

            if (SequenceMaker.isLightMasterPath1) {
                lights1 = lightsRL;
                lights2 = lightsLR;

                if (isFlipped) {
                    lightsOuter1 = lightsRR;
                    lightsOuter2 = lightsLL;
                }
                else {
                    lightsOuter1 = lightsRR;
                    lightsOuter2 = lightsLL;
                }
            }
            else {
                if (isFlipped) {
                    lights1 = lightsRR;
                    lights2 = lightsLL;
                }
                else {
                    lights1 = lightsRR;
                    lights2 = lightsLL;
                }

                lightsOuter1 = lightsRL;
                lightsOuter2 = lightsLR;
            }

            var LCE = SequenceMaker.lightController;
            var LCESub = SequenceMaker.lightSubcontroller;

            if (LCE == TrafficLightController.iLightControllerEnum.Regular) {
                lights1.UpdateLights(TrafficLightController.iLightStatusEnum.Regular, LCESub, isLightsEnabled);
                lights2.UpdateLights(TrafficLightController.iLightStatusEnum.Regular, LCESub, isLightsEnabled);
                lightsOuter1.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lightsOuter2.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
            }
            else if (LCE == TrafficLightController.iLightControllerEnum.LeftTurn) {
                lights1.UpdateLights(TrafficLightController.iLightStatusEnum.LeftTurn, LCESub, isLightsEnabled);
                lights2.UpdateLights(TrafficLightController.iLightStatusEnum.LeftTurn, LCESub, isLightsEnabled);
                lightsOuter1.UpdateLights(TrafficLightController.iLightStatusEnum.RightTurn, LCESub, isLightsEnabled);
                lightsOuter2.UpdateLights(TrafficLightController.iLightStatusEnum.RightTurn, LCESub, isLightsEnabled);
            }
            else if (LCE == TrafficLightController.iLightControllerEnum.Red) {
                lights1.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lights2.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lightsOuter1.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lightsOuter2.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
            }

            //Debug.Log ("Starting: " + SMaker.ToString());
            StartCoroutine(TrafficLightFixedUpdate(SequenceMaker.time));
        }

        #endregion


        #region "Materials"

        public void ResetMaterialsAll() {
            ResetCenterMaterials(false);
            ResetExtStrechtedMaterials(false);
            ResetExtTiledMaterials(false);
            ResetLanesMaterials(false);
            UpdateMaterials();
        }


        public void ResetCenterMaterials(bool _isUpdate = true) {
            var lanesNumber = "-2L";
            lanesAmount = node1.spline.road.laneAmount;
            if (lanesAmount == 4)
                lanesNumber = "-4L";
            else if (lanesAmount == 6) lanesNumber = "-6L";
            if (intersectionType == IntersectionTypeEnum.ThreeWay) {
                lanesNumber += "-3";
                if (node1.idOnSpline < 2 || node2.idOnSpline < 2)
                    //if(isFirstSpecialFirst || isFirstSpecialLast)
                    //{
                    //Reverse if from node 0
                    //stands for "Center Reversed"
                    lanesNumber += "-crev";
                //}
            }

            var basePath = RoadEditorUtility.GetBasePath();

            if (roadType == RoadTypeEnum.BothTurnLanes) {
                markerCenter1 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterCenter-Both" + lanesNumber +
                                                   ".mat");
                markerCenter2 = null;
                markerCenter3 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane) {
                markerCenter1 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterCenter-Left" + lanesNumber +
                                                   ".mat");
                markerCenter2 = null;
                markerCenter3 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane) {
                markerCenter1 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterCenter-None" + lanesNumber +
                                                   ".mat");
                markerCenter2 = null;
                markerCenter3 = null;
            }

            if (_isUpdate) UpdateMaterials();
        }


        public void ResetExtStrechtedMaterials(bool _isUpdate = true) {
            var lanesNumber = "-2L";
            lanesAmount = node1.spline.road.laneAmount;
            if (lanesAmount == 4)
                lanesNumber = "-4L";
            else if (lanesAmount == 6) lanesNumber = "-6L";

            var basePath = RoadEditorUtility.GetBasePath();

            if (roadType == RoadTypeEnum.BothTurnLanes) {
                markerExtStretch1 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterStretch-Both" + lanesNumber +
                                                   ".mat");
                markerExtStretch2 = null;
                markerExtStretch3 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane) {
                markerExtStretch1 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterStretch-Left" + lanesNumber +
                                                   ".mat");
                markerExtStretch2 = null;
                markerExtStretch3 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane) {
                markerExtStretch1 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterStretch-None" + lanesNumber +
                                                   ".mat");
                markerExtStretch2 = null;
                markerExtStretch3 = null;
            }

            if (_isUpdate) UpdateMaterials();
        }


        public void ResetExtTiledMaterials(bool _isUpdate = true) {
            var basePath = RoadEditorUtility.GetBasePath();

            if (roadType == RoadTypeEnum.BothTurnLanes) {
                markerExtTiled1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                markerExtTiled2 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
                markerExtTiled3 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane) {
                markerExtTiled1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                markerExtTiled2 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
                markerExtTiled3 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane) {
                markerExtTiled1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                markerExtTiled2 =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
                markerExtTiled3 = null;
            }

            if (_isUpdate) UpdateMaterials();
        }


        public void ResetLanesMaterials(bool _isUpdate = true) {
            var lanesNumber = "";
            lanesAmount = node1.spline.road.laneAmount;
            if (lanesAmount == 4)
                lanesNumber = "-4L";
            else if (lanesAmount == 6) lanesNumber = "-6L";

            var basePath = RoadEditorUtility.GetBasePath();

            if (intersectionType == IntersectionTypeEnum.ThreeWay) {
                lane1Mat1Disabled =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabled.mat");
                lane1Mat2Disabled = null;
                if (roadType == RoadTypeEnum.BothTurnLanes) {
                    lane1Mat1DisabledActive =
                        RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledOuterRR.mat");
                    lane1Mat2DisabledActive = null;
                    lane2Mat1Disabled =
                        RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledR.mat");
                    lane2Mat2Disabled = null;
                }
                else {
                    lane2Mat1Disabled = null;
                    lane2Mat2Disabled = null;
                    lane2Mat1DisabledActive = null;
                    lane2Mat2DisabledActive = null;
                }

                lane2Mat1DisabledActive =
                    RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledOuter" +
                                                   lanesNumber + ".mat");
                lane2Mat2DisabledActive = null;
                if (roadType == RoadTypeEnum.BothTurnLanes) {
                    lane2Mat1DisabledActiveR =
                        RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledOuterR.mat");
                    lane2Mat2DisabledActiveR = null;
                    lane3Mat1Disabled =
                        RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledR.mat");
                    lane3Mat2Disabled = null;
                }
                else {
                    lane2Mat1DisabledActiveR = null;
                    lane2Mat2DisabledActiveR = null;
                    lane3Mat1Disabled = null;
                    lane3Mat2Disabled = null;
                }
            }
            else {
                lane1Mat1Disabled = null;
                lane1Mat2Disabled = null;
                lane2Mat1Disabled = null;
                lane2Mat2Disabled = null;
                lane2Mat1DisabledActive = null;
                lane2Mat2DisabledActive = null;
                lane2Mat1DisabledActiveR = null;
                lane2Mat2DisabledActiveR = null;
                lane3Mat1Disabled = null;
                lane3Mat2Disabled = null;
            }

            if (roadType == RoadTypeEnum.BothTurnLanes) {
                lane0Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteLYellowR" +
                                                           lanesNumber + ".mat");
                lane0Mat2 = null;
                lane1Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterYellowLWhiteR.mat");
                lane1Mat2 = null;
                lane2Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteR" + lanesNumber +
                                                           ".mat");
                lane2Mat2 = null;
                lane3Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteR.mat");
                lane3Mat2 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane) {
                lane0Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteLYellowR" +
                                                           lanesNumber + ".mat");
                lane0Mat2 = null;
                lane1Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterYellowLWhiteR.mat");
                lane1Mat2 = null;
                lane2Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteR" + lanesNumber +
                                                           ".mat");
                lane2Mat2 = null;
                lane3Mat1 = null;
                lane3Mat2 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane) {
                lane0Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteLYellowR" +
                                                           lanesNumber + ".mat");
                lane0Mat2 = null;
                lane1Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterYellowLWhiteR" +
                                                           lanesNumber + ".mat");
                lane1Mat2 = null;
                lane2Mat1 = null;
                lane2Mat2 = null;
                lane3Mat1 = null;
                lane3Mat2 = null;
            }

            if (_isUpdate) UpdateMaterials();
        }


        private void ApplyMaterials(List<MeshRenderer> _meshRenderers, Material _material1 = null,
            Material _material2 = null, Material _material3 = null) {
            if (_meshRenderers != null && _meshRenderers.Count > 0) {
                var MarkerExtStretchMats = new List<Material>();

                if (_material1 != null) {
                    MarkerExtStretchMats.Add(_material1);
                    if (_material2 != null) {
                        MarkerExtStretchMats.Add(_material2);
                        if (_material3 != null) MarkerExtStretchMats.Add(_material3);
                    }
                }

                var meshMaterials = MarkerExtStretchMats.ToArray();
                for (var i = 0; i < _meshRenderers.Count; i++) _meshRenderers[i].materials = meshMaterials;
            }
        }


        public void UpdateMaterials() {
            var childCount = transform.childCount;
            var extStretchMR = new List<MeshRenderer>();
            var extTiledMR = new List<MeshRenderer>();
            MeshRenderer centerMR = null;
            var lane0MR = new List<MeshRenderer>();
            var lane1MR = new List<MeshRenderer>();
            var lane2MR = new List<MeshRenderer>();
            var lane3MR = new List<MeshRenderer>();
            var laneD1MR = new List<MeshRenderer>();
            var laneD3MR = new List<MeshRenderer>();
            var laneDA2MR = new List<MeshRenderer>();
            var laneDAR2MR = new List<MeshRenderer>();
            var laneD2MR = new List<MeshRenderer>();
            var laneDA1MR = new List<MeshRenderer>();

            MeshRenderer childMesh;
            var transformName = "";
            for (var i = 0; i < childCount; i++) {
                childMesh = transform.GetChild(i).GetComponent<MeshRenderer>();
                if (childMesh == null) continue;

                transformName = childMesh.transform.name.ToLower();
                if (transformName.Contains("-stretchext")) {
                    extStretchMR.Add(childMesh);
                    continue;
                }

                if (transformName.Contains("-tiledext")) {
                    extTiledMR.Add(childMesh);
                    continue;
                }

                if (transformName.Contains("centermarkers")) {
                    centerMR = childMesh;
                    continue;
                }

                if (transformName.Contains("lane0")) {
                    lane0MR.Add(childMesh);
                    continue;
                }

                if (transformName.Contains("lane1")) {
                    lane1MR.Add(childMesh);
                    continue;
                }

                if (transformName.Contains("lane2")) {
                    lane2MR.Add(childMesh);
                    continue;
                }

                if (transformName.Contains("lane3")) {
                    lane3MR.Add(childMesh);
                    continue;
                }

                if (intersectionType == IntersectionTypeEnum.ThreeWay) {
                    if (transformName.Contains("laned1")) {
                        laneD1MR.Add(childMesh);
                        continue;
                    }

                    if (transformName.Contains("laned3")) {
                        laneD3MR.Add(childMesh);
                        continue;
                    }

                    if (transformName.Contains("laneda2")) {
                        laneDA2MR.Add(childMesh);
                        continue;
                    }

                    if (transformName.Contains("lanedar2")) {
                        laneDAR2MR.Add(childMesh);
                        continue;
                    }

                    if (transformName.Contains("laned2")) {
                        laneD2MR.Add(childMesh);
                        continue;
                    }

                    if (transformName.Contains("laneda1")) laneDA1MR.Add(childMesh);
                }
            }

            ApplyMaterials(extStretchMR, markerExtStretch1, markerExtStretch2, markerExtStretch3);
            ApplyMaterials(extTiledMR, markerExtTiled1, markerExtTiled2, markerExtTiled3);

            // Center only uses 1 mesh renderer
            if (centerMR != null) {
                var centerMats = new List<Material>();

                if (markerCenter1 != null) {
                    centerMats.Add(markerCenter1);
                    if (markerCenter2 != null) {
                        centerMats.Add(markerCenter2);
                        if (markerCenter3 != null) centerMats.Add(markerCenter3);
                    }
                }

                centerMR.materials = centerMats.ToArray();
            }


            ApplyMaterials(lane0MR, lane0Mat1, lane0Mat2);
            ApplyMaterials(lane1MR, lane1Mat1, lane1Mat2);
            ApplyMaterials(lane2MR, lane2Mat1, lane2Mat2);
            ApplyMaterials(lane3MR, lane3Mat1, lane3Mat2);
            ApplyMaterials(laneD1MR, lane1Mat1Disabled, lane1Mat2Disabled);
            ApplyMaterials(laneD3MR, lane3Mat1Disabled, lane3Mat2Disabled);
            ApplyMaterials(laneDA2MR, lane2Mat1DisabledActive, lane2Mat2DisabledActive);
            ApplyMaterials(laneDAR2MR, lane2Mat1DisabledActiveR, lane2Mat2DisabledActiveR);
            ApplyMaterials(laneD2MR, lane2Mat1Disabled, lane2Mat2Disabled);
            ApplyMaterials(laneDA1MR, lane1Mat1DisabledActive, lane1Mat2DisabledActive);
        }

        #endregion


        #region "TrafficLights"

        public void ToggleTrafficLightPoleColor() {
            var basePath = RoadEditorUtility.GetBasePath();

            Material trafficLightMaterial = null;
            if (isTrafficLightGray)
                trafficLightMaterial = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Signs/InterTLB2.mat");
            else
                trafficLightMaterial = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Signs/InterTLB1.mat");
            var childCount = transform.childCount;
            var transformName = "";
            MeshRenderer MR = null;
            var materials = new Material[1];
            materials[0] = trafficLightMaterial;
            for (var index = 0; index < childCount; index++) {
                transformName = transform.GetChild(index).name.ToLower();
                if (transformName.Contains("trafficlight")) {
                    MR = transform.GetChild(index).GetComponent<MeshRenderer>();
                    MR.materials = materials;
                }
            }
        }


        public void TogglePointLights(bool _isLightsEnabled) {
            isLightsEnabled = _isLightsEnabled;
            var cCount = transform.childCount;
            Light[] fLights = null;
            Transform tTrans = null;
            for (var index = 0; index < cCount; index++)
                if (transform.GetChild(index).name.ToLower().Contains("trafficlight")) {
                    tTrans = transform.GetChild(index);
                    var childCount = tTrans.childCount;
                    for (var k = 0; k < childCount; k++)
                        if (tTrans.GetChild(k).name.ToLower().Contains("streetlight")) {
                            fLights = tTrans.GetChild(k).GetComponentsInChildren<Light>();
                            if (fLights != null)
                                for (var j = 0; j < fLights.Length; j++) {
                                    fLights[j].enabled = isLightsEnabled;
                                    fLights[j].range = streetLightRange;
                                    fLights[j].intensity = streetLightIntensity;
                                    fLights[j].color = streetLightColor;
                                }

                            break;
                        }
                }
        }


        public void ResetStreetLightSettings() {
            streetLightRange = 30f;
            streetLightIntensity = 1f;
            streetLightColor = new Color(1f, 0.7451f, 0.27451f, 1f);
            TogglePointLights(isLightsEnabled);
        }

        #endregion

    }
}