using System.Collections.Generic;
using UnityEngine;

namespace RoadArchitect {
    public class RoadTerrainInfo {
        public Rect bounds;
        public float[,] heights;
        public int hmHeight;
        public int hmWidth;
        public Vector3 pos;
        public Vector3 size;
        public int uID;


        public static RoadTerrainInfo[] GetRoadTerrainInfos() {
            Object[] tTerrainsObj = Object.FindObjectsOfType<Terrain>();
            RoadTerrainInfo tInfo;
            var tInfos = new List<RoadTerrainInfo>();
            foreach (Terrain tTerrain in tTerrainsObj) {
                tInfo = new RoadTerrainInfo();
                tInfo.uID = tTerrain.transform.gameObject.GetComponent<RoadTerrain>().UID;
                tInfo.bounds = new Rect(tTerrain.transform.position.x, tTerrain.transform.position.z,
                    tTerrain.terrainData.size.x, tTerrain.terrainData.size.z);
                tInfo.hmWidth = tTerrain.terrainData.heightmapResolution;
                tInfo.hmHeight = tTerrain.terrainData.heightmapResolution;
                tInfo.pos = tTerrain.transform.position;
                tInfo.size = tTerrain.terrainData.size;
                tInfo.heights = tTerrain.terrainData.GetHeights(0, 0, tInfo.hmWidth, tInfo.hmHeight);
                tInfos.Add(tInfo);
            }

            var fInfos = new RoadTerrainInfo[tInfos.Count];
            var fInfosLength = fInfos.Length;
            for (var index = 0; index < fInfosLength; index++) fInfos[index] = tInfos[index];
            tInfos = null;
            return fInfos;
        }
    }
}