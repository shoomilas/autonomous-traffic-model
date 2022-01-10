using System;
using UnityEngine;

namespace PathCreation.Examples {
    [ExecuteInEditMode]
    public abstract class PathSceneTool : MonoBehaviour {
        public PathCreator pathCreator;
        public bool autoUpdate = true;

        protected VertexPath path => pathCreator.path;


        protected virtual void OnDestroy() {
            if (onDestroyed != null) onDestroyed();
        }

        public event Action onDestroyed;

        public void TriggerUpdate() {
            PathUpdated();
        }

        protected abstract void PathUpdated();
    }
}