using UnityEngine;
using UnityEngine.Serialization;

namespace RoadArchitect {
    public class SplinePreviewNode {


        public void Setup(Vector3 _p, Quaternion _q, Vector2 _io, float _time, string _name) {
            pos = _p;
            rot = _q;
            easeIO = _io;
            time = _time;
            name = _name;
        }

        #region "Vars"

        public Vector3 pos;
        public Quaternion rot;
        public Vector3 tangent;

        [FormerlySerializedAs("EaseIO")] public Vector2 easeIO;

        [FormerlySerializedAs("tTime")] public float time;

        [FormerlySerializedAs("OldTime")] public float oldTime = 0f;

        public string name = "Node-1";

        [FormerlySerializedAs("tempTime")] public bool isTempTime = false;

        public float tempSegmentTime = 0f;
        public float tempMinDistance = 5000f;
        public float tempMinTime = 0f;

        public int idOnSpline;

        [FormerlySerializedAs("GSDSpline")] public SplineC spline;

        [FormerlySerializedAs("bDestroyed")] public bool isDestroyed = false;

        [FormerlySerializedAs("bPreviewNode")] public bool isPreviewNode = false;

        #endregion

    }
}