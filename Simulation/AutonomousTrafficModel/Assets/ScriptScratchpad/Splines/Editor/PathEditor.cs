using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {
    // public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    // {
    //     return source.Select((item, index) => (item, index));
    // } foreach (var (item, index) in collection.WithIndex()
    
    public PathCreator creator;   // public?
    public Path path;             // public?

    void OnSceneGUI() {
        Draw();
    }

    void Draw() {
        for (int i = 0; i < path.NumSegments; i++) {
            Vector2[] points = path.GetPointsInSegment(i);
            
            // Handles.color = Color.black;
            // Handles.DrawLine(points[1], points[0]); // draws for control points
            // Handles.DrawLine(points[2], points[3]); // draws for control points
            // Handles.DrawBezier(points[0],points[3], points[1],
            //     points[2], Color.green, null, 2);
            
            Handles.color = Color.black;
            Handles.DrawLine(points[i+1], points[i+0]); // draws for control points
            Handles.DrawLine(points[i+2], points[i+3]); // draws for control points
            Handles.DrawBezier(points[i+0],points[i+3], points[i+1],
                points[i+2], Color.green, null, 2);
        }
        
        Handles.color = Color.red;
        for (int i = 0; i < path.NumPoints; i++) {
            
            var offset = (Vector3.down + Vector3.left) * .1f; 
            var position3D = new Vector3(path[i].x, path[i].y, 0) + offset;
            Handles.Label(position3D, $"{i}");
            
            Vector2 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, .1f, Vector2.zero, Handles.CylinderHandleCap);
            if (path[i] != newPos) {
                Undo.RecordObject(creator, "Move point");
                path.MovePoint(i, newPos);
            }
        }
    }
    
    void OnEnable() {
        creator = (PathCreator)target;
        if (creator.path == null) {
            creator.CreatePath();
        }
        path = creator.path;
    }
}
