#region "Imports"

using System.Collections.Generic;
using UnityEngine;

#endregion


namespace RoadArchitect.Roads {
    /* Proper automation flow:
     * 1. Make sure isAllowingRoadUpdates in the scene's RoadSystem is set to FALSE.
     * 2. Create your roads programmatically via CreateRoadProgrammatically (pass it the road, and then the points in a list)
     *      a. Optionally you can do it via CreateNodeProgrammatically and InsertNodeProgrammatically
     * 3. Call CreateIntersectionsProgrammaticallyForRoad for each road to create intersections automatically at intersection points.
     * 4. Set isAllowingRoadUpdates in the scene's RoadSystem is set to TRUE.
     * 5. Call RoadSystem.UpdateAllRoads();
     * 6. Call RoadSystem.UpdateAllRoads(); after step #5 completes.
     * 
     * See "UnitTests.cs" for an example on automation (ignore unit test #3).
     */


    public static class RoadAutomation {
        /// <summary>
        ///     Use this to create nodes via coding while in editor mode. Make sure isAllowingRoadUpdates is set to false in
        ///     RS.roadSystem.isAllowingRoadUpdates.
        /// </summary>
        /// <param name="RS">The road system to create nodes on.</param>
        /// <param name="NodeLocation">The location of the newly created node.</param>
        public static Road CreateRoadProgrammatically(RoadSystem _RoadSys, ref List<Vector3> _positions) {
            var roadObject = _RoadSys.AddRoad();
            var road = roadObject.GetComponent<Road>();

            var count = _positions.Count;
            for (var index = 0; index < count; index++) CreateNodeProgrammatically(road, _positions[index]);

            return road;
        }


        /// <summary>
        ///     Use this to create nodes via coding while in editor mode. Make sure isAllowingRoadUpdates is set to false in
        ///     RS.roadSystem.isAllowingRoadUpdates.
        /// </summary>
        /// <param name="RS">The road system to create nodes on.</param>
        /// <param name="_nodePosition">The location of the newly created node.</param>
        public static SplineN CreateNodeProgrammatically(Road _road, Vector3 _nodePosition) {
            var splineChildCount = _road.spline.transform.childCount;
            //Add the node
            var nodeObj = new GameObject("Node" + (splineChildCount + 1));
            var node = nodeObj.AddComponent<SplineN>();

            //Set node location:
            //Make sure it doesn't try to create a node below 0 height
            if (_nodePosition.y < 0.03f) _nodePosition.y = 0.03f;
            nodeObj.transform.position = _nodePosition;

            //Set the node's parent:
            nodeObj.transform.parent = _road.splineObject.transform;

            //Set the idOnSpline:
            node.idOnSpline = splineChildCount + 1;
            node.spline = _road.spline;

            //Make sure isAllowingRoadUpdates is set to false in RS.roadSystem.isAllowingRoadUpdates
            _road.UpdateRoad();

            return node;
        }


        /// <summary>
        ///     Use this to insert nodes via coding while in editor mode. Make sure isAllowingRoadUpdates is set to false in
        ///     RS.roadSystem.isAllowingRoadUpdates.
        /// </summary>
        /// <param name="_road">The road system to insert nodes in.</param>
        /// <param name="_nodePosition">The location of the newly inserted node.</param>
        public static SplineN InsertNodeProgrammatically(Road _road, Vector3 _nodePosition) {
            GameObject nodeObj;
            Object[] worldNodeCount = Object.FindObjectsOfType<SplineN>();
            nodeObj = new GameObject("Node" + worldNodeCount.Length);

            //Set node location:
            //Make sure it doesn't try to create a node below 0 height.
            if (_nodePosition.y < 0.03f) _nodePosition.y = 0.03f;
            nodeObj.transform.position = _nodePosition;

            //Set the node's parent:
            nodeObj.transform.parent = _road.splineObject.transform;

            var nodesCount = _road.spline.nodes.Count;

            //Get the closet param on spline:
            var param = _road.spline.GetClosestParam(_nodePosition, false, true);

            var isInsertEnded = false;
            var isInsertZero = false;
            var start = 0;
            if (RootUtils.IsApproximately(param, 0f, 0.0001f)) {
                isInsertZero = true;
                start = 0;
            }
            else if (RootUtils.IsApproximately(param, 1f, 0.0001f)) {
                //Inserted at end, switch to create node instead:
                Object.DestroyImmediate(nodeObj);
                return CreateNodeProgrammatically(_road, _nodePosition);
            }

            //Figure out where to insert the node:
            for (var index = 0; index < nodesCount; index++) {
                var node = _road.spline.nodes[index];
                if (!isInsertZero && !isInsertEnded)
                    if (param > node.time)
                        start = node.idOnSpline + 1;
            }

            for (var index = start; index < nodesCount; index++) _road.spline.nodes[index].idOnSpline += 1;

            var newNode = nodeObj.AddComponent<SplineN>();
            newNode.spline = _road.spline;
            newNode.idOnSpline = start;
            newNode.pos = _nodePosition;
            _road.spline.nodes.Insert(start, newNode);

            //Make sure isAllowingRoadUpdates is set to false in RS.roadSystem.isAllowingRoadUpdates
            _road.UpdateRoad();

            return newNode;
        }


        /// <summary> Creates intersections where this road intersects with other roads. </summary>
        /// <param name="_road">The primary road to create intersections for.</param>
        /// <param name="_iStopType">Stop signs, traffic lights #1 (US) or traffic lights #2 (Euro). Defaults to none.</param>
        /// <param name="_roadType">Intersection type: No turn lane, left turn lane or both turn lanes. Defaults to no turn lane.</param>
        public static void CreateIntersectionsProgrammaticallyForRoad(Road _road,
            RoadIntersection.iStopTypeEnum _iStopType = RoadIntersection.iStopTypeEnum.None,
            RoadIntersection.RoadTypeEnum _roadType = RoadIntersection.RoadTypeEnum.NoTurnLane) {
            /*
            General logic:
             20m increments to gather collection of which roads intersect
                2m increments to find actual intersection point
                each 2m, primary road checks all intersecting array for an intersection.
             find intersection point
                if any intersections already within 75m or 100m, dont create intersection here
                check if nodes within 50m, if more than one just grab closest, and move  it to intersecting point
                if no node within 50m, add
             create intersection with above two nodes
            */

            Object[] roadObjects = Object.FindObjectsOfType<Road>();

            //20m increments to gather collection of which roads intersect
            var roads = new List<Road>();
            foreach (Road road in roadObjects)
                if (_road != road) {
                    var earlyDistanceCheckMeters = 10f;
                    var earlyDistanceCheckThreshold = 50f;
                    var isEarlyDistanceFound = false;
                    var tempRoadMod = earlyDistanceCheckMeters / _road.spline.distance;
                    var roadMod = earlyDistanceCheckMeters / road.spline.distance;
                    var vector1 = default(Vector3);
                    var vector2 = default(Vector3);
                    for (var index = 0f; index < 1.0000001f; index += tempRoadMod) {
                        vector1 = _road.spline.GetSplineValue(index);
                        for (var x = 0f; x < 1.000001f; x += roadMod) {
                            vector2 = road.spline.GetSplineValue(x);
                            if (Vector3.Distance(vector1, vector2) < earlyDistanceCheckThreshold) {
                                if (!roads.Contains(road)) roads.Add(road);
                                isEarlyDistanceFound = true;
                                break;
                            }
                        }

                        if (isEarlyDistanceFound) break;
                    }
                }

            //See if any end point nodes are on top of each other already since T might not intersect all the time.:
            var keyValuePairs = new List<KeyValuePair<SplineN, SplineN>>();
            foreach (var road in roads)
            foreach (var intersectionNode1 in _road.spline.nodes) {
                if (intersectionNode1.isIntersection || !intersectionNode1.IsLegitimate()) continue;
                foreach (var intersectionNode2 in road.spline.nodes) {
                    if (intersectionNode2.isIntersection || !intersectionNode2.IsLegitimate()) continue;
                    if (intersectionNode1.transform.position == intersectionNode2.transform.position)
                        //Only do T intersections and let the next algorithm handle the +, since T might not intersect all the time.
                        if (intersectionNode1.isEndPoint || intersectionNode2.isEndPoint)
                            keyValuePairs.Add(new KeyValuePair<SplineN, SplineN>(intersectionNode1, intersectionNode2));
                }
            }

            foreach (var KVP in keyValuePairs) {
                // Creates fresh intersection
                //Now create the fucking intersection:
                var tInter = Intersections.CreateIntersection(KVP.Key, KVP.Value);
                var roadIntersection = tInter.GetComponent<RoadIntersection>();
                roadIntersection.intersectionStopType = _iStopType;
                roadIntersection.roadType = _roadType;
            }

            //Main algorithm: 2m increments to find actual intersection point:
            foreach (var road in roads)
                if (_road != road) {
                    //Debug.Log("Checking road: " + xRoad.transform.name);
                    var distanceCheckMeters = 2f;
                    var isEarlyDistanceFound = false;
                    var tempRoadMod = distanceCheckMeters / _road.spline.distance;
                    var roadMod = distanceCheckMeters / road.spline.distance;
                    var vector = default(Vector3);
                    var vector1 = default(Vector2);
                    var vector2 = default(Vector2);
                    var xVector1 = default(Vector2);
                    var xVector2 = default(Vector2);
                    var intersectPoint2D = default(Vector2);
                    var i2 = 0f;
                    for (var index = 0f; index < 1.0000001f; index += tempRoadMod) {
                        i2 = index + tempRoadMod;
                        if (i2 > 1f) i2 = 1f;
                        vector = _road.spline.GetSplineValue(index);
                        vector1 = new Vector2(vector.x, vector.z);
                        vector = _road.spline.GetSplineValue(i2);
                        vector2 = new Vector2(vector.x, vector.z);

                        var x2 = 0f;
                        for (var x = 0f; x < 1.000001f; x += roadMod) {
                            x2 = x + roadMod;
                            if (x2 > 1f) x2 = 1f;
                            vector = road.spline.GetSplineValue(x);
                            xVector1 = new Vector2(vector.x, vector.z);
                            vector = road.spline.GetSplineValue(x2);
                            xVector2 = new Vector2(vector.x, vector.z);

                            //Now see if these two lines intersect:
                            if (RootUtils.Intersects2D(ref vector1, ref vector2, ref xVector1, ref xVector2,
                                out intersectPoint2D)) {
                                //Get height of intersection on primary road:
                                var height = 0f;
                                var param = _road.spline.GetClosestParam(new Vector3(intersectPoint2D.x, 0f,
                                    intersectPoint2D.y));
                                var paramVector = _road.spline.GetSplineValue(param);
                                height = paramVector.y;

                                //if any intersections already within 75m or 100m, dont create intersection here
                                Object[] allInterectionObjects = Object.FindObjectsOfType<RoadIntersection>();
                                foreach (RoadIntersection roadIntersection in allInterectionObjects)
                                    if (Vector2.Distance(
                                        new Vector2(roadIntersection.transform.position.x,
                                            roadIntersection.transform.position.z), intersectPoint2D) < 100f)
                                        goto NoIntersectionCreation;

                                SplineN IntersectionNode1 = null;
                                SplineN IntersectionNode2 = null;
                                var IntersectionPoint3D = new Vector3(intersectPoint2D.x, height, intersectPoint2D.y);
                                //Debug.Log("Instersect found road: " + xRoad.transform.name + " at point: " + IntersectionPoint3D.ToString());

                                //Check primary road if any nodes are nearby and usable for intersection
                                foreach (var node in _road.spline.nodes)
                                    if (node.IsLegitimate())
                                        if (Vector2.Distance(
                                            new Vector2(node.transform.position.x, node.transform.position.z),
                                            intersectPoint2D) < 30f) {
                                            IntersectionNode1 = node;
                                            IntersectionNode1.transform.position = IntersectionPoint3D;
                                            IntersectionNode1.pos = IntersectionPoint3D;
                                            break;
                                        }

                                //Check secondary road if any nodes are nearby and usable for intersection
                                foreach (var node in road.spline.nodes)
                                    if (node.IsLegitimate())
                                        if (Vector2.Distance(
                                            new Vector2(node.transform.position.x, node.transform.position.z),
                                            intersectPoint2D) < 30f) {
                                            IntersectionNode2 = node;
                                            IntersectionNode2.transform.position = IntersectionPoint3D;
                                            IntersectionNode2.pos = IntersectionPoint3D;
                                            break;
                                        }

                                //Check if any of the nodes are null. If so, need to insert node. And maybe update it.
                                if (IntersectionNode1 == null)
                                    IntersectionNode1 = InsertNodeProgrammatically(_road, IntersectionPoint3D);
                                if (IntersectionNode2 == null)
                                    IntersectionNode2 = InsertNodeProgrammatically(road, IntersectionPoint3D);

                                //Now create the fucking intersection:
                                var intersection =
                                    Intersections.CreateIntersection(IntersectionNode1, IntersectionNode2);
                                var newRoadIntersection = intersection.GetComponent<RoadIntersection>();
                                newRoadIntersection.intersectionStopType = _iStopType;
                                newRoadIntersection.roadType = _roadType;
                            }

                            NoIntersectionCreation:
                            //Gibberish to get rid of warnings:
                            var xxx = 1;
                            if (xxx == 1) xxx = 2;
                        }

                        if (isEarlyDistanceFound) break;
                    }
                }
        }
    }
}