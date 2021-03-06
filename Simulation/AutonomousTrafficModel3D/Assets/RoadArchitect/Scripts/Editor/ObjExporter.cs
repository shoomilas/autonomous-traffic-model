/*
Based on ObjExporter.cs, this "wrapper" lets you export to .OBJ directly from the editor menu.
 
Use by selecting the objects you want to export, and select the appropriate menu item from "Custom->Export".
Exported models are put in a folder called "ExportedObj" in the root of your project.
Textures should also be copied and placed in the same folder.
*/

#if UNITY_EDITOR

#region "Imports"

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion


namespace RoadArchitect {
    public class ObjExporter : ScriptableObject {
        private static int vertexOffset;
        private static int normalOffset;
        private static int uvOffset;


        //User should probably be able to change this. It is currently left as an excercise for
        //the reader.
        private static readonly string targetFolder = "ExportedObj";


        private static string MeshToString(MeshFilter _meshFilter, Dictionary<string, ObjMaterial> _materialList) {
            var mesh = _meshFilter.sharedMesh;
            var renderer = _meshFilter.GetComponent<Renderer>();
            //Material[] mats = mf.renderer.sharedMaterials;
            var materials = renderer.sharedMaterials;

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("g ").Append(_meshFilter.name).Append("\n");
            foreach (var lv in mesh.vertices) {
                var wv = _meshFilter.transform.TransformPoint(lv);

                //This is sort of ugly - inverting x-component since we're in
                //a different coordinate system than "everyone" is "used to".
                stringBuilder.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }

            stringBuilder.Append("\n");

            foreach (var lv in mesh.normals) {
                var wv = _meshFilter.transform.TransformDirection(lv);

                stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }

            stringBuilder.Append("\n");

            foreach (Vector3 v in mesh.uv) stringBuilder.Append(string.Format("vt {0} {1}\n", v.x, v.y));

            for (var material = 0; material < mesh.subMeshCount; material++) {
                stringBuilder.Append("\n");
                stringBuilder.Append("usemtl ").Append(materials[material].name).Append("\n");
                stringBuilder.Append("usemap ").Append(materials[material].name).Append("\n");

                //See if this material is already in the materiallist.
                try {
                    var objMaterial = new ObjMaterial();

                    objMaterial.name = materials[material].name;

                    if (materials[material].mainTexture)
                        objMaterial.textureName = AssetDatabase.GetAssetPath(materials[material].mainTexture);
                    else
                        objMaterial.textureName = null;

                    _materialList.Add(objMaterial.name, objMaterial);
                }
                catch (ArgumentException) {
                    //Already in the dictionary
                }


                var triangles = mesh.GetTriangles(material);
                for (var index = 0; index < triangles.Length; index += 3)
                    //Because we inverted the x-component, we also needed to alter the triangle winding.
                    stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                        triangles[index] + 1 + vertexOffset, triangles[index + 1] + 1 + normalOffset,
                        triangles[index + 2] + 1 + uvOffset));
            }

            vertexOffset += mesh.vertices.Length;
            normalOffset += mesh.normals.Length;
            uvOffset += mesh.uv.Length;

            return stringBuilder.ToString();
        }


        private static void Clear() {
            vertexOffset = 0;
            normalOffset = 0;
            uvOffset = 0;
        }


        private static Dictionary<string, ObjMaterial> PrepareFileWrite() {
            Clear();

            return new Dictionary<string, ObjMaterial>();
        }


        private static void MaterialsToFile(Dictionary<string, ObjMaterial> _materialList, string _folder,
            string _fileName) {
            using (var streamWriter = new StreamWriter(Path.Combine(_folder, _fileName) + ".mtl")) {
                foreach (var kvp in _materialList) {
                    streamWriter.Write("\n");
                    streamWriter.Write("newmtl {0}\n", kvp.Key);
                    streamWriter.Write("Ka  0.6 0.6 0.6\n");
                    streamWriter.Write("Kd  0.6 0.6 0.6\n");
                    streamWriter.Write("Ks  0.9 0.9 0.9\n");
                    streamWriter.Write("d  1.0\n");
                    streamWriter.Write("Ns  0.0\n");
                    streamWriter.Write("illum 2\n");

                    if (kvp.Value.textureName != null) {
                        var destinationFile = kvp.Value.textureName;

                        var stripIndex = destinationFile.LastIndexOf(Path.PathSeparator);

                        if (stripIndex >= 0) destinationFile = destinationFile.Substring(stripIndex + 1).Trim();


                        var relativeFile = destinationFile;

                        destinationFile = Path.Combine(_folder, destinationFile);

                        //Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);

                        try {
                            //Copy the source file
                            File.Copy(kvp.Value.textureName, destinationFile);
                        }
                        catch { }


                        streamWriter.Write("map_Kd {0}", relativeFile);
                    }

                    streamWriter.Write("\n\n\n");
                }
            }
        }


        private static void MeshToFile(MeshFilter _meshFilter, string _folder, string _fileName) {
            var materialList = PrepareFileWrite();

            using (var streamWriter = new StreamWriter(Path.Combine(_folder, _fileName) + ".obj")) {
                streamWriter.Write("mtllib ./" + _fileName + ".mtl\n");

                streamWriter.Write(MeshToString(_meshFilter, materialList));
            }

            MaterialsToFile(materialList, _folder, _fileName);
        }


        private static void MeshesToFile(MeshFilter[] _meshFilter, string _folder, string _fileName) {
            var materialList = PrepareFileWrite();

            using (var streamWriter = new StreamWriter(Path.Combine(_folder, _fileName) + ".obj")) {
                streamWriter.Write("mtllib ./" + _fileName + ".mtl\n");

                for (var index = 0; index < _meshFilter.Length; index++)
                    streamWriter.Write(MeshToString(_meshFilter[index], materialList));
            }

            MaterialsToFile(materialList, _folder, _fileName);
        }


        private static bool CreateTargetFolder() {
            try {
                Directory.CreateDirectory(targetFolder);
            }
            catch {
                EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
                return false;
            }

            return true;
        }


        [MenuItem("Window/Road Architect/Export/Export all MeshFilters in selection to separate OBJs")]
        private static void ExportSelectionToSeparate() {
            if (!CreateTargetFolder()) return;

            var selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

            if (selection.Length == 0) {
                EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects",
                    "");
                return;
            }

            var exportedObjects = 0;

            for (var index = 0; index < selection.Length; index++) {
                Component[] meshfilter = selection[index].GetComponentsInChildren<MeshFilter>();

                for (var m = 0; m < meshfilter.Length; m++) {
                    exportedObjects++;
                    MeshToFile((MeshFilter)meshfilter[m], targetFolder, selection[index].name + "_" + index + "_" + m);
                }
            }

            if (exportedObjects > 0)
                EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
            else
                EditorUtility.DisplayDialog("Objects not exported",
                    "Make sure at least some of your selected objects have mesh filters!", "");
        }


        [MenuItem("Window/Road Architect/Export/Export whole selection to single OBJ")]
        private static void ExportWholeSelectionToSingle() {
            if (!CreateTargetFolder()) return;


            var selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

            if (selection.Length == 0) {
                EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects",
                    "");
                return;
            }

            var exportedObjects = 0;

            var mfList = new ArrayList();

            for (var index = 0; index < selection.Length; index++) {
                Component[] meshfilter = selection[index].GetComponentsInChildren<MeshFilter>();

                for (var m = 0; m < meshfilter.Length; m++) {
                    exportedObjects++;
                    mfList.Add(meshfilter[m]);
                }
            }

            if (exportedObjects > 0) {
                var meshFilters = new MeshFilter[mfList.Count];

                for (var index = 0; index < mfList.Count; index++) meshFilters[index] = (MeshFilter)mfList[index];


                var sceneName = SceneManager.GetActiveScene().name;
                //string filename = EditorApplication.currentScene + "_" + exportedObjects;
                var filename = sceneName + "_" + exportedObjects;

                var stripIndex = filename.LastIndexOf(Path.PathSeparator);

                if (stripIndex >= 0) filename = filename.Substring(stripIndex + 1).Trim();

                MeshesToFile(meshFilters, targetFolder, filename);


                EditorUtility.DisplayDialog("Objects exported",
                    "Exported " + exportedObjects + " objects to " + filename, "");
            }
            else {
                EditorUtility.DisplayDialog("Objects not exported",
                    "Make sure at least some of your selected objects have mesh filters!", "");
            }
        }


        [MenuItem("Window/Road Architect/Export/Export each selected to single OBJ")]
        private static void ExportEachSelectionToSingle() {
            if (!CreateTargetFolder()) return;

            var selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

            if (selection.Length == 0) {
                EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects",
                    "");
                return;
            }

            var exportedObjects = 0;


            for (var index = 0; index < selection.Length; index++) {
                Component[] meshfilter = selection[index].GetComponentsInChildren<MeshFilter>();

                var mf = new MeshFilter[meshfilter.Length];

                for (var m = 0; m < meshfilter.Length; m++) {
                    exportedObjects++;
                    mf[m] = (MeshFilter)meshfilter[m];
                }

                MeshesToFile(mf, targetFolder, selection[index].name + "_" + index);
            }

            if (exportedObjects > 0)
                EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
            else
                EditorUtility.DisplayDialog("Objects not exported",
                    "Make sure at least some of your selected objects have mesh filters!", "");
        }


        [MenuItem("Window/Road Architect/Export/Exporters by Hrafnkell Freyr Hlooversson from Unity3D wiki")]
        private static void OpenLink() {
            Application.OpenURL("http://wiki.unity3d.com/index.php?title=ObjExporter");
        }


        private struct ObjMaterial {
            public string name;
            public string textureName;
        }
    }
}
#endif