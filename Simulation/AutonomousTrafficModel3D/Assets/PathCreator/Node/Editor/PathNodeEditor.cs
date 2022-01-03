using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using PathCreation.Examples;
using PathCreator.Aggregator;
using UnityEditor;
using UnityEngine;
using Object = System.Object;
using PathCreator = PathCreation.PathCreator;

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor {

    const string TextRemovePathNode = "Remove PathNode";
    const string TextAddPathNode = "Add Spline To PathNode";
    const string TextAddPathNodeAndSelect = "Add Spline To PathNode (and Select)";
    const string TextConnectNodesButton = "Connect chosen PathNode";
    const string TextUpdateClosestPathNodesButton = "Update Closest Path Nodes";
    private const string TextUpdateAllPathNodesButton = "[!] Update All Path Nodes";
    private const string TextConnectNodesLabel = "Node to connect (as next node):";
    
    private void OnEnable() {
        SceneView.duringSceneGui += CustomOnSceneGUI;
    }

    private void OnDisable() {
        SceneView.duringSceneGui -= CustomOnSceneGUI;
    }

    private void CustomOnSceneGUI(SceneView view) {
        // var typedTarget = (PathNode)target;
        // float size = 1f;
        // Transform transform = typedTarget.transform;
        // Handles.color = Color.red;
        // Handles.ConeHandleCap(
        //     0,
        //     transform.position + new Vector3(0f, size / 2, 0f),
        //     transform.rotation * Quaternion.LookRotation(Vector3.up),
        //     size,
        //     EventType.Repaint
        // );
    }

    // private void OnSceneGUI() {
    // var foundPathNodeObjects = FindObjectsOfType<PathNode>();
    // foreach (var foundPathNodeObject in foundPathNodeObjects) {
    //     
    // }
    //
    // var typedTarget = (PathNode)target;
    //
    // float size = 1f;
    // Transform transform = typedTarget.transform;
    // Handles.color = Color.red;
    // Handles.ConeHandleCap(
    //     0,
    //     transform.position + new Vector3(0f, size/2, 0f),
    //     transform.rotation * Quaternion.LookRotation(Vector3.up),
    //     size,
    //     EventType.Repaint
    // );
    // }

    [SerializeField]
    public PathNode PathNodeToConnectTo = null;
    
    public override void OnInspectorGUI() {
        var typedTarget = (PathNode)target;
        base.OnInspectorGUI();
        var spacerSize = 40;
        GUILayout.Space(spacerSize);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(TextAddPathNode)) {;
            AddSplineToPathNode(typedTarget);
        }
        if (GUILayout.Button(TextAddPathNodeAndSelect)) {
            UndoNodeAction(TextAddPathNodeAndSelect, typedTarget);
            AddSplineToPathNodeAndSelect(typedTarget);
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button(TextRemovePathNode)) {;
            UndoNodeAction(TextRemovePathNode, typedTarget);
            PathNode.DeletePathNode(typedTarget);
        }

        if (GUILayout.Button(TextUpdateClosestPathNodesButton)) {
            typedTarget.UpdateClosestSplineConnections();
        }
        if (GUILayout.Button(TextUpdateAllPathNodesButton)) {
            PathNode.UpdateAllNodesSplineConnections();
        }

        GUILayout.BeginVertical();
        GUILayout.Space(spacerSize);
        PathNodeToConnectTo = (PathNode)EditorGUILayout
            .ObjectField(TextConnectNodesLabel, PathNodeToConnectTo, typeof(PathNode), true);
        
        if (GUILayout.Button(TextConnectNodesButton)) {
            ConnectNodes(typedTarget, PathNodeToConnectTo);
            PathNodeHelper.SelectObject(PathNodeToConnectTo.gameObject);
        }
        
        GUILayout.EndVertical();
    }

    private void ConnectNodes(PathNode srcNode, PathNode dstNode) {
        bool isRequestNotValid =
               srcNode.nextPathNodes.Contains(dstNode)
                || srcNode.previousPathNodes.Contains(dstNode)
                || srcNode == null
                || dstNode == null;
            
        if (isRequestNotValid) {
            Debug.Log("Request not valid");
            return;
        }
        
        AddSplineBetweenPathNodes(srcNode, dstNode);
        srcNode.nextPathNodes.Add(dstNode);
        dstNode.previousPathNodes.Add(srcNode);
    }

    private void AddSplineToPathNodeAndSelect(PathNode node) {
        var newNode = AddSplineToPathNode(node);
        if (newNode != null) {
            PathNodeHelper.SelectObject(newNode.gameObject);
        }
    }

    private PathNode AddSplineToPathNode(PathNode node) {
        Undo.RecordObject(node.transform.parent, "Add new Path Node");
        var newNode = AddPathNode(node);
        var newSpline = AddSplineBetweenPathNodes(node, newNode);
        // var splineData = new SplineOutData(newSpline, Direction.Unknown, newNode);
        // node.SplinesOut.Add(splineData);
        PathNodeHelper.SelectObject(node.gameObject);
        return newNode;
    }

    private void UndoNodeAction(string undoString, PathNode node) {
        node.nextPathNodes.ForEach(_=>Undo.RegisterCompleteObjectUndo(_, undoString));
        node.previousPathNodes.ForEach(_ => {
            Undo.RegisterCompleteObjectUndo(_, undoString);
        });
        Undo.RegisterCompleteObjectUndo(node, undoString);
    }
    
    public PathCreation.PathCreator AddSplineBetweenPathNodes(PathNode node1, PathNode node2) {
        var pos1 = node1.transform.position;
        var pos2 = node2.transform.position;
        var pos3 = (pos1 + pos2) / 2;
        var listOfPositions = new List<Vector3> { node1.transform.position, pos3, node2.transform.position};
        
        // Create a new bezier path from the waypoints.
        var bezier = new BezierPath(listOfPositions);
        
        // Create new GameObject
        GameObject go = new GameObject("Spline");
        var foo = go.AddComponent<PathCreation.PathCreator>();
        foo.bezierPath = bezier;
        var name1 = node1.transform.name;
        var name2 = node2.transform.name;
        foo.transform.name = $"Spline: {name1}-{name2}";foo.transform.name = $"Spline {NodeCounter}";
        foo.transform.parent = node1.transform; // as child
        // foo.transform.parent = node1.transform.parent; // as sibling
        
        // add splinesOutData to node1
        var splineOutData = new SplineOutData(foo, Direction.Unknown, node2);
        node1.SplinesOut.Add(splineOutData);
        return foo;
    }

    public static int NodeCounter = 0;
    public static int SplineCounter = 0;

    private PathNode AddPathNode(PathNode node) {
        // TODO: adding existing pathnodes - do not add as children then
        var tr = node.transform;
        var goo = PathNode.MakePathNode(node);
        var transform = goo.transform;
        transform.parent = tr.transform.parent;
        transform.position = tr.position + Vector3.right + Vector3.forward;
        transform.name = $"Path Node {NodeCounter}";
        NodeCounter += 1;
        node.nextPathNodes.Add(goo);
        return goo;
    }

    ////////////////// ADDITIONAL ////////////////
    private void SelectAllPathNodes() {
        var objects = FindObjectsOfType<PathNode>();
        var uObjects = new UnityEngine.Object[objects.Length];
        for (int i = 0; i < objects.Length; i++) {
            uObjects[i] = (UnityEngine.Object)objects[i];
        }

        Selection.objects = uObjects;
    }

}