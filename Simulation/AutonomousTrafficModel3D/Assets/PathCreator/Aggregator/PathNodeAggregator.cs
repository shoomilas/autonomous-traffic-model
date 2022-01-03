using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathCreator.Aggregator {
    public class PathNodeAggregator : MonoBehaviour, IPathAggregator {
        public List<PathCreation.PathCreator> Paths => new List<PathCreation.PathCreator>();
        private void Start()
        {
            Debug.Log("Started");
            // var childrenCreators = GetComponentsInChildren<PathCreation.PathCreator>().ToList();
        }
    }
}