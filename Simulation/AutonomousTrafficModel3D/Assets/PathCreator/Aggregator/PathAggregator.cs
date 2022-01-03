using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathCreator.Aggregator
{
    public class PathAggregator : MonoBehaviour, IPathAggregator {
        public List<PathCreation.PathCreator> Paths => GetComponentsInChildren<PathCreation.PathCreator>().ToList();

        private void Start()
        {
            Debug.Log("Started");
            var childrenCreators = GetComponentsInChildren<PathCreation.PathCreator>().ToList();
        }
    }
}