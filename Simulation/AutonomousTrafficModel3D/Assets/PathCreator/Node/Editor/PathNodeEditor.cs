using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using PathCreation;
using PathCreation.Examples;
using PathCreator.Aggregator;
using UnityEditor;
using UnityEngine;
using Object = System.Object;
using PathCreator = PathCreation.PathCreator;

[CanEditMultipleObjects]
[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor {

    const string TextRemovePathNode = "Remove PathNode";
    const string TextAddPathNode = "Add Spline To PathNode";
    const string TextAddPathNodeAndSelect = "Add Spline To PathNode (and Select)";
    const string TextConnectNodesButton = "Connect chosen PathNode";
    const string TextUpdateClosestPathNodesButton = "Update Closest Path Nodes";
    private const string TextUpdateAllPathNodesButton = "[!] Update All Path Nodes";
    private const string TextConnectNodesLabel = "Node to connect (as next node):";
    const string TextConnectNodesFirstAsSrcLabel = "Connect with first as SRC";
    const string TextConnectNodesFirstAsDstLabel = "Connect with first as DST";
    private const string TextCreateNewNode = "Create new node";
    
    public static float newNodeOffset = 1f;
    public static float forwardOffset = 1f;
    public static float rightOffset = 1f;
    public static bool createAsDst = false;
    
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

    private PathNode typedTarget;
    
    public override void OnInspectorGUI() {
        typedTarget = (PathNode)target;
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
        
        if (targets.Length == 2) {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove ALL Connections")) {
                foreach (var target in targets) {
                    ((PathNode)target).RemoveAllConnectionsForPathNode();
                }
            }
            if (GUILayout.Button("Remove Connection Between Selected 2")) {
                ((PathNode)targets[1]).RemoveConnectionBetweenTwoNodesCaller((PathNode)targets[0]);
            }
            GUILayout.EndHorizontal();
        }
        else {
            if (GUILayout.Button("Remove ALL Connections")) {
                typedTarget.RemoveAllConnectionsForPathNode();
            }
        }

        GUILayout.BeginVertical();
        GUILayout.Space(spacerSize);
        
       
            
        GUILayout.BeginVertical();
        createAsDst = GUILayout.Toggle(createAsDst, "(create new node as SRC)");
        GUILayout.EndHorizontal();

        
        GUILayout.BeginHorizontal();
        newNodeOffset = EditorGUILayout.FloatField("(A) Create new node: ", newNodeOffset);
        PathNode newNode = null;
        if (GUILayout.Button("←")) {
            UndoNodeAction(TextCreateNewNode, typedTarget);
            newNode =  typedTarget.CreateNewNodeInSpecifiedDirectionWithDefinedOffset(newNodeOffset, Vector3.left);
        }

        if (GUILayout.Button("↑")) {
            UndoNodeAction(TextCreateNewNode, typedTarget);
            newNode =  typedTarget.CreateNewNodeInSpecifiedDirectionWithDefinedOffset(newNodeOffset, Vector3.forward);
        }

        if (GUILayout.Button("↓")) {
            UndoNodeAction(TextCreateNewNode, typedTarget);
            newNode =  typedTarget.CreateNewNodeInSpecifiedDirectionWithDefinedOffset(newNodeOffset, Vector3.back);
        }

        if (GUILayout.Button("→")) {
            UndoNodeAction(TextCreateNewNode, typedTarget);
            newNode = typedTarget.CreateNewNodeInSpecifiedDirectionWithDefinedOffset(newNodeOffset, Vector3.right);
        }
        if (newNode != null) {
            PathNodeHelper.SelectObject(newNode.gameObject);
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("(B) Create new node: ");
        GUILayout.Label("↑");
        forwardOffset = EditorGUILayout.FloatField(forwardOffset);
        GUILayout.Label("→");
        rightOffset = EditorGUILayout.FloatField(rightOffset);
        if (GUILayout.Button("Create")) {
            newNode = typedTarget.CreateNewNodeWithForwardRightOffset(forwardOffset, rightOffset);
            if (newNode != null) { PathNodeHelper.SelectObject(newNode.gameObject); }
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(spacerSize);
        
        if (targets.Length > 1) {
            var label = $"First: {targets[0].name} | Second:";
            PathNodeToConnectTo = (PathNode)EditorGUILayout
                .ObjectField(label, targets[1], typeof(PathNode), true);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(TextConnectNodesFirstAsSrcLabel)) {
                ConnectNodes((PathNode)targets[0], (PathNode)PathNodeToConnectTo);
                PathNodeHelper.SelectObject(PathNodeToConnectTo.gameObject);
            }
            if (GUILayout.Button(TextConnectNodesFirstAsDstLabel)) {
                ConnectNodes((PathNode)PathNodeToConnectTo, (PathNode)targets[0]);
                PathNodeHelper.SelectObject(PathNodeToConnectTo.gameObject);
            }            
            GUILayout.EndHorizontal();
        }
        else {
            var label = "First: - | Second:";
            if (target != null) {
                label = $"First: {target.name} | Second:";   
            }
            PathNodeToConnectTo = (PathNode)EditorGUILayout
                .ObjectField(label, PathNodeToConnectTo, typeof(PathNode), true);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(TextConnectNodesFirstAsSrcLabel)) {
                ConnectNodes(typedTarget, PathNodeToConnectTo);
                PathNodeHelper.SelectObject(PathNodeToConnectTo.gameObject);
            }
            if (GUILayout.Button(TextConnectNodesFirstAsDstLabel)) {
                ConnectNodes(PathNodeToConnectTo, typedTarget);
                PathNodeHelper.SelectObject(PathNodeToConnectTo.gameObject);
            }            
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    public void ConnectNodes(PathNode srcNode, PathNode dstNode) {
        srcNode.ConnectNodes(dstNode, true);
    }

    public void AddSplineToPathNodeAndSelect(PathNode node) {
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
        var toClear = false;
        node.nextPathNodes?.ForEach(_ => {
                if (_ == null) {
                    toClear = true; 
                } 
                else
                {
                    Undo.RegisterCompleteObjectUndo(_, undoString);
                }
            }
        );
        if(toClear) { node.nextPathNodes.Clear(); }
        node.previousPathNodes?.ForEach(_ => {
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