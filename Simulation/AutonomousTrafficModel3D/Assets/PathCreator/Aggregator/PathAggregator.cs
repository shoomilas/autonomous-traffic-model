using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathCreator.Aggregator
{
    public class PathAggregator : MonoBehaviour
    {
        public List<PathCreation.PathCreator> Paths
        {
            get
            {
                var childrenCreators = GetComponentsInChildren<PathCreation.PathCreator>().ToList();
                // childrenCreators.ForEach( x => Debug.Log(x.name)); 
                return childrenCreators.ToList();
            }
        }

        private void Start()
        {
            Debug.Log("Started");
            var childrenCreators = GetComponentsInChildren<PathCreation.PathCreator>().ToList();
            // childrenCreators.ForEach( x => Debug.Log(x.name));
        }
    }
}