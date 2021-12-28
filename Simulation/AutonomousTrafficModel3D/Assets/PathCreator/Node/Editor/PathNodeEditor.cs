using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using PathCreator.Aggregator;
using UnityEditor;
using UnityEngine;
using PathCreator = PathCreation.PathCreator;

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor {
         
    private void OnEnable()
    {
        SceneView.duringSceneGui += CustomOnSceneGUI;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= CustomOnSceneGUI;
    }

    private void CustomOnSceneGUI(SceneView view)
    {
        var typedTarget = (PathNode)target;
        
        float size = 1f;
        Transform transform = typedTarget.transform;
        Handles.color = Color.red;
        Handles.ConeHandleCap(
            0,
            transform.position + new Vector3(0f, size/2, 0f),
            transform.rotation * Quaternion.LookRotation(Vector3.up),
            size,
            EventType.Repaint
        );
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
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var typedTarget = (PathNode)target;
        if (GUILayout.Button("Add Spline To Current PathNode")) {
            AddSplineToPathNode(typedTarget); 
        }
        if (GUILayout.Button("Remove Current PathNode")) {
            PathNode.DeletePathNode(typedTarget);
        }
        if (GUILayout.Button("(TODO) Safe Remove Current PathNode")) {
            Debug.Log("Not implemented.");
            // TODO: PathNode.DeletePathNodeSafe(typedTarget); // also should remove splines going to and from surrounding nodes
        }
        if (GUILayout.Button("Select All")) {
           
        }
    }

    private void AddSplineToPathNode(PathNode node) { // move to PathNode class
        var newNode = AddPathNode(node);
        SelectObject(node.gameObject);
        // NewSpline
    }

    private PathNode AddPathNode(PathNode node) {
        // TODO: adding existing pathnodes - do not add as children then
        var tr = node.transform;
        var goo = PathNode.MakePathNode(node);
        var transform = goo.transform;
        transform.parent = tr.transform.parent;
        transform.position = tr.position + Vector3.right + Vector3.forward;
        transform.name = "Path Node";
        node.nextPathNodes.Add(goo);
        return goo;
    }

    public static void SelectObject(GameObject obj) {
        Selection.objects = new UnityEngine.Object[] { obj.gameObject };
    }
    
    ////////////////// ADDITIONAL ////////////////
    private void SelectAllPathNodes() {
        var objects = FindObjectsOfType<PathNode>();
        var uObjects = new UnityEngine.Object[objects.Length];
        for (int i = 0; i<objects.Length; i++) {
            uObjects[i] = (UnityEngine.Object) objects[i];
        }
        Selection.objects = uObjects;
    }

}
