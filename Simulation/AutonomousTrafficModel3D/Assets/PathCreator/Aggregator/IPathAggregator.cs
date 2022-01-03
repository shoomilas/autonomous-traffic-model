using System.Collections.Generic;
using UnityEngine;

namespace PathCreator.Aggregator {
    public interface IPathAggregator {
        List<PathCreation.PathCreator> Paths { get; }
    }
}