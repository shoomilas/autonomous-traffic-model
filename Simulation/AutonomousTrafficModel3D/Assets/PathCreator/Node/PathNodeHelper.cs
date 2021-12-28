using UnityEditor;
using UnityEngine;

public class PathNodeHelper {
    public static void SelectObject(GameObject obj) {
        Selection.objects = new UnityEngine.Object[] { obj.gameObject };
    }
}