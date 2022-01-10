using UnityEngine;

namespace PathCreation.Examples {

    [ExecuteInEditMode]
    public class PathPlacer : PathSceneTool {

        private const float minSpacing = .1f;

        public GameObject prefab;
        public GameObject holder;
        public float spacing = 3;

        private void Generate() {
            if (pathCreator != null && prefab != null && holder != null) {
                DestroyObjects();

                var path = pathCreator.path;

                spacing = Mathf.Max(minSpacing, spacing);
                float dst = 0;

                while (dst < path.length) {
                    var point = path.GetPointAtDistance(dst);
                    var rot = path.GetRotationAtDistance(dst);
                    Instantiate(prefab, point, rot, holder.transform);
                    dst += spacing;
                }
            }
        }

        private void DestroyObjects() {
            var numChildren = holder.transform.childCount;
            for (var i = numChildren - 1; i >= 0; i--) DestroyImmediate(holder.transform.GetChild(i).gameObject, false);
        }

        protected override void PathUpdated() {
            if (pathCreator != null) Generate();
        }
    }
}