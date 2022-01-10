#if UNITY_EDITOR

#region "Imports"

using UnityEditor;
using UnityEngine;

#endregion


namespace RoadArchitect {
    [CustomEditor(typeof(SplineC))]
    public class SplineCEditor : Editor {
        private int browseNode;
        private SplineC spline;


        private void OnEnable() {
            spline = (SplineC)target;
        }


        public override void OnInspectorGUI() {
            #region NodeBrowser

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Browse to node:", EditorStyles.boldLabel);
            browseNode = EditorGUILayout.IntField(browseNode);
            if (GUILayout.Button("Browse"))
                if (browseNode < spline.nodes.Count)
                    Selection.objects = new Object[1] { spline.nodes[browseNode] };
            EditorGUILayout.EndHorizontal();

            #endregion
        }
    }
}
#endif