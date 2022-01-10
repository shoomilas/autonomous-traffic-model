#region "Imports"

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RoadArchitect.EdgeObjects;
using RoadArchitect.Splination;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion


namespace RoadArchitect {
    //Generic http://www.fhwa.dot.gov/bridge/bridgerail/br053504.cfm
    public enum RailingTypeEnum {
        None,
        Generic1,
        Generic2,
        K_Rail,
        WBeam
    }

    public enum RailingSubTypeEnum {
        Both,
        Left,
        Right
    }

    public enum SignPlacementSubTypeEnum {
        Center,
        Left,
        Right
    }

    public enum CenterDividerTypeEnum {
        None,
        K_Rail,
        KRail_Blinds,
        Wire,
        Markers
    }

    public enum EndCapTypeEnum {
        None,
        WBeam,
        Barrels3Static,
        Barrels3Rigid,
        Barrels7Static,
        Barrels7Rigid
    }

    public enum RoadUpdateTypeEnum {
        Full,
        Intersection,
        Railing,
        CenterDivider,
        Bridges
    }

    public enum AxisTypeEnum {
        X,
        Y,
        Z
    }


    public static class RoadUtility {
        public const string FileSepString = "\n#### RoadArchitect ####\n";
        public const string FileSepStringCRLF = "\r\n#### RoadArchitect ####\r\n";


        /// <summary> Returns closest terrain to _vect </summary>
        public static Terrain GetTerrain(Vector3 _vect) {
            return GetTerrainDo(ref _vect);
        }


        /// <summary> Returns closest terrain to _vect </summary>
        private static Terrain GetTerrainDo(ref Vector3 _vect) {
            //Sphere cast 5m first. Then raycast down 1000m, then up 1000m.
            var colliders = Physics.OverlapSphere(_vect, 10f);
            if (colliders != null) {
                var collidersLength = colliders.Length;
                for (var index = 0; index < collidersLength; index++) {
                    var tTerrain = colliders[index].transform.GetComponent<Terrain>();
                    if (tTerrain) {
                        colliders = null;
                        return tTerrain;
                    }
                }

                colliders = null;
            }

            RaycastHit[] hits;
            hits = Physics.RaycastAll(_vect, Vector3.down, 1000f);
            var hitsLength = 0;
            if (hits != null) {
                hitsLength = hits.Length;
                for (var index = 0; index < hitsLength; index++) {
                    var tTerrain = hits[index].collider.transform.GetComponent<Terrain>();
                    if (tTerrain) {
                        hits = null;
                        return tTerrain;
                    }
                }

                hits = null;
            }

            hits = Physics.RaycastAll(_vect, Vector3.up, 1000f);
            if (hits != null) {
                hitsLength = hits.Length;
                for (var i = 0; i < hitsLength; i++) {
                    var tTerrain = hits[i].collider.transform.GetComponent<Terrain>();
                    if (tTerrain) {
                        hits = null;
                        return tTerrain;
                    }
                }

                hits = null;
            }

            return null;
        }


        public static void SaveNodeObjects(ref SplinatedMeshMaker[] _splinatedObjects,
            ref EdgeObjectMaker[] _edgeObjects, ref WizardObject _wizardObj) {
            var sCount = _splinatedObjects.Length;
            var eCount = _edgeObjects.Length;
            //Splinated objects first:
            SplinatedMeshMaker SMM = null;
            RootUtils.CheckCreateSpecialLibraryDirs();
            var libraryPath = RootUtils.GetDirLibrary();
            var tPath = Path.Combine(Path.Combine(libraryPath, "Groups"), _wizardObj.fileName + ".rao");
            if (_wizardObj.isDefault) {
                tPath = Path.Combine(Path.Combine(libraryPath, "Groups"), "Default");
                tPath = Path.Combine(tPath, _wizardObj.fileName + ".rao");
            }

            var builder = new StringBuilder(32768);

            //Wizard object:
            builder.Append(_wizardObj.ConvertToString());
            builder.Append(FileSepString);

            for (var index = 0; index < sCount; index++) {
                SMM = _splinatedObjects[index];
                builder.Append(SMM.ConvertToString());
                builder.Append(FileSepString);
            }

            EdgeObjectMaker EOM = null;
            for (var index = 0; index < eCount; index++) {
                EOM = _edgeObjects[index];
                builder.Append(EOM.ConvertToString());
                builder.Append(FileSepString);
            }

            File.WriteAllText(tPath, builder.ToString());
        }


        /// <summary> Loads splinated objects for this _node </summary>
        public static void LoadNodeObjects(string _fileName, SplineN _node, bool _isDefault = false,
            bool _isBridge = false) {
            var filePath = "";
            RootUtils.CheckCreateSpecialLibraryDirs();
            var libraryPath = RootUtils.GetDirLibrary();
            if (_isDefault) {
                filePath = Path.Combine(Path.Combine(libraryPath, "Groups"), "Default");
                filePath = Path.Combine(filePath, _fileName + ".rao");
            }
            else {
                filePath = Path.Combine(Path.Combine(libraryPath, "Groups"), _fileName + ".rao");
            }

            var fileData = File.ReadAllText(filePath);
            var tSep = new string[2];
            tSep[0] = FileSepString;
            tSep[1] = FileSepStringCRLF;
            var tSplit = fileData.Split(tSep, StringSplitOptions.RemoveEmptyEntries);

            SplinatedMeshMaker SMM = null;
            SplinatedMeshMaker.SplinatedMeshLibraryMaker SLM = null;
            EdgeObjectMaker EOM = null;
            EdgeObjectMaker.EdgeObjectLibraryMaker ELM = null;
            var tSplitCount = tSplit.Length;

            for (var index = 0; index < tSplitCount; index++) {
                SLM = null;
                SLM = SplinatedMeshMaker.SLMFromData(tSplit[index]);
                if (SLM != null) {
                    SMM = _node.AddSplinatedObject();
                    SMM.LoadFromLibraryBulk(ref SLM);
                    SMM.isToggled = false;
                    if (_isBridge && _node.isBridgeStart && _node.isBridgeMatched &&
                        _node.bridgeCounterpartNode != null) {
                        SMM.StartTime = _node.time;
                        SMM.EndTime = _node.bridgeCounterpartNode.time;
                        SMM.StartPos = _node.spline.GetSplineValue(SMM.StartTime);
                        SMM.EndPos = _node.spline.GetSplineValue(SMM.EndTime);
                    }

                    continue;
                }

                ELM = null;
                ELM = EdgeObjectMaker.ELMFromData(tSplit[index]);
                if (ELM != null) {
                    EOM = _node.AddEdgeObject();
                    EOM.LoadFromLibraryBulk(ref ELM);
                    EOM.isToggled = false;
                    if (!EOM.isSingle && _isBridge && _node.isBridgeStart && _node.isBridgeMatched &&
                        _node.bridgeCounterpartNode != null) {
                        EOM.startTime = _node.time;
                        EOM.endTime = _node.bridgeCounterpartNode.time;
                        EOM.startPos = _node.spline.GetSplineValue(EOM.startTime);
                        EOM.endPos = _node.spline.GetSplineValue(EOM.endTime);
                    }
                    else if (EOM.isSingle && _isBridge && _node.bridgeCounterpartNode != null && _node.isBridgeStart) {
                        var tDist = EOM.singleOnlyBridgePercent * (_node.bridgeCounterpartNode.dist - _node.dist) +
                                    _node.dist;
                        EOM.singlePosition = _node.spline.TranslateDistBasedToParam(tDist);
                        EOM.startPos = _node.spline.GetSplineValue(EOM.singlePosition);
                        EOM.endPos = _node.spline.GetSplineValue(EOM.singlePosition);
                    }
                }
            }

            _node.SetupSplinatedMeshes();
            _node.SetupEdgeObjects();
        }


        #region "Terrain history"

        public static void ConstructRoadStoreTerrainHistory(ref Road _road) {
            Object[] TIDs = Object.FindObjectsOfType<RoadTerrain>();

            var tTIDS = new HashSet<int>();
            foreach (RoadTerrain TID in TIDs) tTIDS.Add(TID.UID);

            if (_road.TerrainHistory != null && _road.TerrainHistory.Count > 0)
                //Delete unnecessary terrain histories:
                foreach (var THMaker in _road.TerrainHistory)
                    if (!tTIDS.Contains(THMaker.terrainID)) {
                        THMaker.Nullify();
                        _road.TerrainHistory.Remove(THMaker);
                    }

            if (_road.TerrainHistory == null) _road.TerrainHistory = new List<TerrainHistoryMaker>();
            foreach (var TTD in _road.EditorTTDList) {
                TerrainHistoryMaker TH = null;
                RoadTerrain TID = null;
                //Get terrainID:
                foreach (RoadTerrain _TID in TIDs)
                    if (_TID.UID == TTD.uID)
                        TID = _TID;

                if (_road.TerrainHistory == null) _road.TerrainHistory = new List<TerrainHistoryMaker>();
                if (TID == null) continue;

                var THCount = _road.TerrainHistory.Count;
                var isContainingTID = false;
                for (var index = 0; index < THCount; index++)
                    if (_road.TerrainHistory[index].terrainID == TID.UID) {
                        isContainingTID = true;
                        break;
                    }

                if (!isContainingTID) {
                    var THMaker = new TerrainHistoryMaker();
                    THMaker.terrainID = TID.UID;
                    _road.TerrainHistory.Add(THMaker);
                }

                TH = null;
                for (var index = 0; index < THCount; index++)
                    if (_road.TerrainHistory[index].terrainID == TID.UID) {
                        TH = _road.TerrainHistory[index];
                        break;
                    }

                if (TH == null) continue;

                //Heights:
                if (_road.isHeightModificationEnabled) {
                    if (TTD.cX != null && TTD.cY != null) {
                        TH.x1 = new int[TTD.Count];
                        Array.Copy(TTD.cX, 0, TH.x1, 0, TTD.Count);
                        TH.y1 = new int[TTD.Count];
                        Array.Copy(TTD.cY, 0, TH.y1, 0, TTD.Count);
                        TH.height = new float[TTD.Count];
                        Array.Copy(TTD.oldH, 0, TH.height, 0, TTD.Count);
                        TH.Count = TTD.Count;
                        TH.heightmapResolution = TTD.TerrainMaxIndex;
                    }
                }
                else {
                    TH.x1 = null;
                    TH.y1 = null;
                    TH.height = null;
                    TH.Count = 0;
                }

                //Details:
                if (_road.isDetailModificationEnabled) {
                    var TotalSize = 0;
                    for (var i = 0; i < TTD.DetailLayersCount; i++) TotalSize += TTD.detailsCount[i];

                    TH.detailsX = new int[TotalSize];
                    TH.detailsY = new int[TotalSize];
                    TH.detailsOldValue = new int[TotalSize];

                    var RunningIndex = 0;
                    var cLength = 0;
                    for (var index = 0; index < TTD.DetailLayersCount; index++) {
                        cLength = TTD.detailsCount[index];
                        if (cLength < 1) continue;
                        Array.Copy(TTD.DetailsX[index].ToArray(), 0, TH.detailsX, RunningIndex, cLength);
                        Array.Copy(TTD.DetailsY[index].ToArray(), 0, TH.detailsY, RunningIndex, cLength);
                        Array.Copy(TTD.OldDetailsValue[index].ToArray(), 0, TH.detailsOldValue, RunningIndex, cLength);
                        RunningIndex += TTD.detailsCount[index];
                    }

                    //TH.detailsX = TTD.detailsX;
                    //TH.detailsY = TTD.detailsY;
                    //TH.detailsOldValue = TTD.OldDetailsValue;
                    TH.detailsCount = TTD.detailsCount;
                    TH.detailLayersCount = TTD.DetailLayersCount;
                }
                else {
                    TH.detailsX = null;
                    TH.detailsY = null;
                    TH.detailsOldValue = null;
                    TH.detailsCount = null;
                    TH.detailLayersCount = 0;
                }

                //Trees:
                if (_road.isTreeModificationEnabled) {
                    if (TTD.TreesOld != null) {
                        TH.MakeRATrees(ref TTD.TreesOld);
                        TTD.TreesOld.Clear();
                        TTD.TreesOld = null;
                        TH.treesCount = TTD.treesCount;
                    }
                }
                else {
                    TH.oldTrees = null;
                    TH.treesCount = 0;
                }
            }
        }


        /// <summary> Clears the terrain history of _road </summary>
        public static void ResetTerrainHistory(ref Road _road) {
            if (_road.TerrainHistory != null) {
                _road.TerrainHistory.Clear();
                _road.TerrainHistory = null;
            }
        }

        #endregion


        #region "Splat maps"

        /// <summary> Returns a splat map texture encoded as png </summary>
        public static byte[] MakeSplatMap(Terrain _terrain, Color _BG, Color _FG, int _width, int _height,
            float _splatWidth, bool _isSkippingBridge, bool _isSkippingTunnel, string _roadUID = "") {
            var tTexture = new Texture2D(_width, _height, TextureFormat.RGB24, false);

            //Set background color:
            var tColorsBG = new Color[_width * _height];
            var tBGCount = tColorsBG.Length;
            for (var i = 0; i < tBGCount; i++) tColorsBG[i] = _BG;
            tTexture.SetPixels(0, 0, _width, _height, tColorsBG);
            tColorsBG = null;

            Object[] tRoads = null;
            if (_roadUID != "") {
                tRoads = new Object[1];
                Object[] roads = Object.FindObjectsOfType<Road>();
                foreach (Road road in roads)
                    if (string.CompareOrdinal(road.UID, _roadUID) == 0) {
                        tRoads[0] = road;
                        break;
                    }
            }
            else {
                tRoads = Object.FindObjectsOfType<Road>();
            }

            var tPos = _terrain.transform.position;
            var tSize = _terrain.terrainData.size;
            foreach (Road tRoad in tRoads) {
                var tSpline = tRoad.spline;
                var tCount = tSpline.RoadDefKeysArray.Length;

                var POS1 = default(Vector3);
                var POS2 = default(Vector3);

                var tVect = default(Vector3);
                var tVect2 = default(Vector3);
                var lVect1 = default(Vector3);
                var lVect2 = default(Vector3);
                var rVect1 = default(Vector3);
                var rVect2 = default(Vector3);

                int x1, y1;
                var tX = new int[4];
                var tY = new int[4];
                var MinX = -1;
                var MaxX = -1;
                var MinY = -1;
                var MaxY = -1;
                var xDiff = -1;
                var yDiff = -1;
                var p1 = 0f;
                var p2 = 0f;
                var bXBad = false;
                var bYBad = false;
                for (var i = 0; i < tCount - 1; i++) {
                    bXBad = false;
                    bYBad = false;
                    p1 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[i]);
                    p2 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[i + 1]);

                    //Skip bridges:
                    if (_isSkippingBridge)
                        if (tSpline.IsInBridgeTerrain(p1))
                            continue;

                    //Skip tunnels:
                    if (_isSkippingTunnel)
                        if (tSpline.IsInTunnelTerrain(p1))
                            continue;

                    tSpline.GetSplineValueBoth(p1, out tVect, out POS1);
                    tSpline.GetSplineValueBoth(p2, out tVect2, out POS2);
                    lVect1 = tVect + new Vector3(_splatWidth * -POS1.normalized.z, 0, _splatWidth * POS1.normalized.x);
                    rVect1 = tVect + new Vector3(_splatWidth * POS1.normalized.z, 0, _splatWidth * -POS1.normalized.x);
                    lVect2 = tVect2 + new Vector3(_splatWidth * -POS2.normalized.z, 0, _splatWidth * POS2.normalized.x);
                    rVect2 = tVect2 + new Vector3(_splatWidth * POS2.normalized.z, 0, _splatWidth * -POS2.normalized.x);

                    TranslateWorldVectToCustom(_width, _height, lVect1, ref tPos, ref tSize, out x1, out y1);
                    tX[0] = x1;
                    tY[0] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect1, ref tPos, ref tSize, out x1, out y1);
                    tX[1] = x1;
                    tY[1] = y1;
                    TranslateWorldVectToCustom(_width, _height, lVect2, ref tPos, ref tSize, out x1, out y1);
                    tX[2] = x1;
                    tY[2] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect2, ref tPos, ref tSize, out x1, out y1);
                    tX[3] = x1;
                    tY[3] = y1;

                    MinX = Mathf.Min(tX);
                    MaxX = Mathf.Max(tX);
                    MinY = Mathf.Min(tY);
                    MaxY = Mathf.Max(tY);


                    if (MinX < 0) {
                        MinX = 0;
                        bXBad = true;
                    }

                    if (MaxX < 0) {
                        MaxX = 0;
                        bXBad = true;
                    }

                    if (MinY < 0) {
                        MinY = 0;
                        bYBad = true;
                    }

                    if (MaxY < 0) {
                        MaxY = 0;
                        bYBad = true;
                    }

                    if (MinX > _width - 1) {
                        MinX = _width - 1;
                        bXBad = true;
                    }

                    if (MaxX > _width - 1) {
                        MaxX = _width - 1;
                        bXBad = true;
                    }

                    if (MinY > _height - 1) {
                        MinY = _height - 1;
                        bYBad = true;
                    }

                    if (MaxY > _height - 1) {
                        MaxY = _height - 1;
                        bYBad = true;
                    }

                    if (bXBad && bYBad) continue;

                    xDiff = MaxX - MinX;
                    yDiff = MaxY - MinY;

                    var tColors = new Color[xDiff * yDiff];
                    var cCount = tColors.Length;
                    for (var j = 0; j < cCount; j++) tColors[j] = _FG;

                    if (xDiff > 0 && yDiff > 0) tTexture.SetPixels(MinX, MinY, xDiff, yDiff, tColors);
                }
            }

            tTexture.Apply();
            var tBytes = tTexture.EncodeToPNG();
            Object.DestroyImmediate(tTexture);
            return tBytes;
        }


        /// <summary> Writes _vect location into _x1 and _y1 relative to the terrain on a 2D map </summary>
        private static void TranslateWorldVectToCustom(int _width, int _height, Vector3 _vect, ref Vector3 _pos,
            ref Vector3 _size, out int _x1, out int _y1) {
            //Get the normalized position of this game object relative to the terrain:
            _vect -= _pos;

            _vect.x = _vect.x / _size.x;
            _vect.z = _vect.z / _size.z;

            //Get the position of the terrain heightmap where this game object is:
            _x1 = (int)(_vect.x * _width);
            _y1 = (int)(_vect.z * _height);
        }

        #endregion

    }
}