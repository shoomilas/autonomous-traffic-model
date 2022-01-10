#region "Imports"

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion


namespace RoadArchitect {
    public static class TerrainHistoryUtility {


        /// <summary> Saves the Terrain History to disk </summary>
        public static void SaveTerrainHistory(List<TerrainHistoryMaker> _obj, Road _road) {
            var path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (string.IsNullOrEmpty(path) || path.Length < 2) return;
            Stream stream = File.Open(path, FileMode.Create);
            var bformatter = new BinaryFormatter();
            bformatter.Binder = new VersionDeserializationBinder();
            bformatter.Serialize(stream, _obj);
            _road.TerrainHistoryByteSize = (stream.Length * 0.001f).ToString("n0") + " kb";
            stream.Close();
        }


        /// <summary> Deletes the Terrain History from disk </summary>
        public static void DeleteTerrainHistory(Road _road) {
            var path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (File.Exists(path)) File.Delete(path);
        }


        /// <summary> Loads the Terrain History from disk </summary>
        public static List<TerrainHistoryMaker> LoadTerrainHistory(Road _road) {
            var path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (string.IsNullOrEmpty(path) || path.Length < 2) return null;
            if (!File.Exists(path)) return null;
            List<TerrainHistoryMaker> result;
            Stream stream = File.Open(path, FileMode.Open);
            var bFormatter = new BinaryFormatter();
            bFormatter.Binder = new VersionDeserializationBinder();

            result = (List<TerrainHistoryMaker>)bFormatter.Deserialize(stream);

            stream.Close();
            return result;
        }


        /// <summary> Generates the Terrain History file name </summary>
        private static string GetRoadTHFilename(ref Road _road) {
            string sceneName;

            sceneName = SceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");
            var roadName = _road.roadSystem.transform.name.Replace("RoadArchitectSystem", "RAS") + "-" +
                           _road.transform.name;
            return sceneName + "-" + roadName + ".th";
        }


        /// <summary> Returns the path to the RoadArchitect folder where Terrain History is saved </summary>
        public static string GetDirBase() {
            return Application.dataPath.Replace("/Assets", "/RoadArchitect/");
        }


        /// <summary> Returns the path where Terrain History is saved </summary>
        public static string GetTHDir() {
            var path = GetDirBase() + "TerrainHistory/";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }


        /// <summary> Checks if RoadArchitect folder exists </summary>
        public static string CheckRoadArchitectDirectory() {
            var path = GetDirBase();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (Directory.Exists(path))
                return path + "/";
            return "";
        }


        /// <summary> Returns RoadArchitect/TerrainHistory path or empty </summary>
        public static string CheckNonAssetDirTH() {
            CheckRoadArchitectDirectory();

            var path = GetTHDir();
            if (Directory.Exists(path))
                return path;
            return "";
        }
        //http://forum.unity3d.com/threads/32647-C-Sharp-Binary-Serialization
        //http://answers.unity3d.com/questions/363477/c-how-to-setup-a-binary-serialization.html

        // === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
        // Do not change this
        public sealed class VersionDeserializationBinder : SerializationBinder {
            public override Type BindToType(string assemblyName, string typeName) {
                if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName)) {
                    Type typeToDeserialize = null;
                    assemblyName = Assembly.GetExecutingAssembly().FullName;
                    // The following line of code returns the type.
                    typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
                    return typeToDeserialize;
                }

                return null;
            }
        }
    }
}