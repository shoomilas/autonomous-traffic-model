using UnityEditor;
using UnityEngine;

namespace RoadArchitect {
    public static class Construction {
        /// <summary> Creates a node and performs validation checks </summary>
        public static SplineN CreateNode(Road _road, bool _isSpecialEndNode = false,
            Vector3 _vectorSpecialLoc = default, bool _isInterNode = false) {
            Object[] worldNodeCount = Object.FindObjectsOfType<SplineN>();
            var nodeObj = new GameObject("Node" + worldNodeCount.Length);

#if UNITY_EDITOR
            if (!_isInterNode) Undo.RegisterCreatedObjectUndo(nodeObj, "Created node");
#endif


            var node = nodeObj.AddComponent<SplineN>();

            if (_isSpecialEndNode) {
                node.isSpecialEndNode = true;
                nodeObj.transform.position = _vectorSpecialLoc;
            }
            else {
                nodeObj.transform.position = _road.editorMousePos;
                //This helps prevent double clicks:
                var nodeCount = _road.spline.GetNodeCount();
                for (var index = 0; index < nodeCount; index++)
                    if (Vector3.Distance(_road.editorMousePos, _road.spline.nodes[index].pos) < 5f) {
                        Object.DestroyImmediate(nodeObj);
                        return null;
                    }
                //End double click prevention
            }


            var xVect = nodeObj.transform.position;
            if (xVect.y < 0.03f) xVect.y = 0.03f;
            nodeObj.transform.position = xVect;

            nodeObj.transform.parent = _road.splineObject.transform;
            node.idOnSpline = _road.spline.GetNodeCount() + 1;
            node.spline = _road.spline;

            //Enforce max road grade:
            if (_road.isMaxGradeEnabled && !_isSpecialEndNode) node.EnsureGradeValidity(-1, true);

            if (!_isInterNode && !_isSpecialEndNode) _road.UpdateRoad();
            return node;
        }


        /// <summary>
        ///     Insert
        ///     Detect closest node (if end node, auto select other node)
        ///     Determine which node is closest (up or down) on spline
        ///     Place node, adjust all id on splines
        ///     Setup spline
        /// </summary>
        public static SplineN InsertNode(Road _road, bool _isForcedLoc = false, Vector3 _forcedLoc = default,
            bool _isPreNode = false, int _insertIndex = -1, bool _isSpecialEndNode = false, bool _isInterNode = false) {
            GameObject nodeObj;
            Object[] worldNodeCount = Object.FindObjectsOfType<SplineN>();
            if (!_isForcedLoc)
                nodeObj = new GameObject("Node" + worldNodeCount.Length);
            else if (_isForcedLoc && !_isSpecialEndNode)
                nodeObj = new GameObject("Node" + worldNodeCount.Length + "Ignore");
            else
                nodeObj = new GameObject("Node" + worldNodeCount.Length);


#if UNITY_EDITOR
            if (!_isInterNode) Undo.RegisterCreatedObjectUndo(nodeObj, "Inserted node");
#endif


            if (!_isForcedLoc) {
                nodeObj.transform.position = _road.editorMousePos;

                //This helps prevent double clicks:
                var nodeCount = _road.spline.GetNodeCount();
                for (var index = 0; index < nodeCount; index++)
                    if (Vector3.Distance(_road.editorMousePos, _road.spline.nodes[index].pos) < 15f) {
                        Object.DestroyImmediate(nodeObj);
                        return null;
                    }
                //End double click prevention
            }
            else {
                nodeObj.transform.position = _forcedLoc;
            }

            var xVect = nodeObj.transform.position;
            if (xVect.y < 0.03f) xVect.y = 0.03f;
            nodeObj.transform.position = xVect;
            nodeObj.transform.parent = _road.splineObject.transform;

            var childCount = _road.spline.nodes.Count;
            //float mDistance = 50000f;
            //float tDistance = 0f;

            float param;
            if (!_isForcedLoc)
                param = _road.spline.GetClosestParam(_road.editorMousePos, false, true);
            else
                param = _road.spline.GetClosestParam(_forcedLoc, false, true);
            var isEndInsert = false;
            var isZeroInsert = false;
            var iStart = 0;
            if (RootUtils.IsApproximately(param, 0f, 0.0001f)) {
                isZeroInsert = true;
                iStart = 0;
            }
            else if (RootUtils.IsApproximately(param, 1f, 0.0001f)) {
                isEndInsert = true;
            }

            if (_isForcedLoc)
                iStart = _insertIndex;
            else
                for (var index = 0; index < childCount; index++) {
                    var xNode = _road.spline.nodes[index];
                    if (!isZeroInsert && !isEndInsert)
                        if (param > xNode.time)
                            iStart = xNode.idOnSpline + 1;
                }

            if (isEndInsert)
                iStart = _road.spline.nodes.Count;
            else
                for (var i = iStart; i < childCount; i++)
                    _road.spline.nodes[i].idOnSpline += 1;

            // Create new node
            var node = nodeObj.AddComponent<SplineN>();
            if (_isForcedLoc && !_isSpecialEndNode) {
                node.isBridge = true;
                node.isIgnore = true;
                //tNode.bIsBridge_PreNode = bIsPreNode;
                //tNode.bIsBridge_PostNode = !bIsPreNode;	
            }

            node.spline = _road.spline;
            node.idOnSpline = iStart;
            node.isSpecialEndNode = _isSpecialEndNode;
            if (!_isForcedLoc)
                node.pos = _road.editorMousePos;
            else
                node.pos = _forcedLoc;

            _road.spline.nodes.Insert(iStart, node);

            //Enforce maximum road grade:
            if (!_isForcedLoc && !_isSpecialEndNode && _road.isMaxGradeEnabled) node.EnsureGradeValidity(iStart);

            if (!_isInterNode && !_isSpecialEndNode)
                if (!_isForcedLoc)
                    _road.UpdateRoad();

            return node;
        }
    }
}