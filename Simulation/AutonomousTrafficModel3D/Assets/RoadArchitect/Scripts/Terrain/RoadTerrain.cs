#region "Imports"

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#endregion


namespace RoadArchitect {
    [ExecuteInEditMode]
    public class RoadTerrain : MonoBehaviour {


        private void Start() {
            CheckID();
            if (!terrain) terrain = transform.gameObject.GetComponent<Terrain>();
        }


        private void OnEnable() {
            CheckID();
            if (!terrain) terrain = transform.gameObject.GetComponent<Terrain>();
        }


        /// <summary> Check for unique id and assign terrain </summary>
        public void CheckID() {
            if (uID < 0) uID = GetNewID();
            if (!terrain) terrain = transform.gameObject.GetComponent<Terrain>();
        }


        /// <summary> Return new id preventing terrain id duplication </summary>
        private int GetNewID() {
            Object[] allTerrainObjs = FindObjectsOfType<RoadTerrain>();
            var allIDS = new List<int>(allTerrainObjs.Length);
            foreach (RoadTerrain Terrain in allTerrainObjs)
                if (Terrain.UID > 0)
                    allIDS.Add(Terrain.UID);

            var isNotDone = true;
            var spamChecker = 0;
            var spamCheckerMax = allIDS.Count + 64;
            int random;
            while (isNotDone) {
                if (spamChecker > spamCheckerMax) {
                    Debug.LogError("Failed to generate terrainID");
                    break;
                }

                random = Random.Range(1, 2000000000);
                if (!allIDS.Contains(random)) {
                    isNotDone = false;
                    return random;
                }

                spamChecker += 1;
            }

            return -1;
        }

        #region "Vars"

        [SerializeField] [HideInInspector] [FormerlySerializedAs("mGSDID")]
        private int uID = -1;

        public int UID => uID;

        [HideInInspector] [FormerlySerializedAs("tTerrain")]
        public Terrain terrain;

        //Splat map:
        [FormerlySerializedAs("SplatResoWidth")]
        public int splatResoWidth = 1024;

        [FormerlySerializedAs("SplatResoHeight")]
        public int splatResoHeight = 1024;

        [FormerlySerializedAs("SplatBackground")]
        public Color splatBackground = new Color(0f, 0f, 0f, 1f);

        [FormerlySerializedAs("SplatForeground")]
        public Color splatForeground = new Color(1f, 1f, 1f, 1f);

        [FormerlySerializedAs("SplatWidth")] public float splatWidth = 30f;

        [FormerlySerializedAs("SplatSkipBridges")]
        public bool isSplatSkipBridges;

        [FormerlySerializedAs("SplatSkipTunnels")]
        public bool isSplatSkipTunnels;

        [FormerlySerializedAs("SplatSingleRoad")]
        public bool isSplatSingleRoad;

        [FormerlySerializedAs("SplatSingleChoiceIndex")]
        public int splatSingleChoiceIndex;

        [FormerlySerializedAs("RoadSingleChoiceUID")]
        public string roadSingleChoiceUID = "";

        #endregion

    }
}