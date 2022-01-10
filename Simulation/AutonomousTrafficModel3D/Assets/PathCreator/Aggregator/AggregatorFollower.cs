using PathCreation;
using UnityEngine;

namespace PathCreator.Aggregator {
    public class AggregatorFollower : MonoBehaviour {
        // xz plane: (90,0,90)
        [SerializeField] [Range(-360, 360)] public float additionalXRotation = 90;
        [SerializeField] [Range(-360, 360)] public float additionalYRotation = 90;
        [SerializeField] [Range(-360, 360)] public float additionalZRotation;

        public PathAggregator aggregator;
        public Transform followerObject;
        public float speed = 0.3f;
        public EndOfPathInstruction endOfPathInstruction;
        public bool updateLocked = true;
        public float updateLockUpperLimit = .998f;

        public int splineIndex;
        private float _distanceTravelled;

        private void Start() {
            if (aggregator != null) {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                // Aggregator.pathUpdated += OnPathChanged;
            }
        }

        private void Update() {
            if (splineIndex < aggregator.Paths.Count) {
                ;
                var pathCreator = aggregator.Paths[splineIndex];
                moveObject(pathCreator);
                actionOnEnd(pathCreator);
            }
        }

        private void moveObject(PathCreation.PathCreator pathCreator) {
            if (pathCreator != null) {
                _distanceTravelled += speed * Time.deltaTime;
                followerObject.transform.position = pathCreator.path.GetPointAtDistance(_distanceTravelled,
                    endOfPathInstruction);
                var initial = pathCreator.path.GetRotationAtDistance(_distanceTravelled, endOfPathInstruction);
                initial *= Quaternion.Euler(additionalXRotation, additionalYRotation, additionalZRotation);
                followerObject.transform.rotation = initial;
            }
        }


        private void actionOnEnd(PathCreation.PathCreator pathCreator) {
            // TODO: Safe "increment once method"
            var pathPercentage = pathCreator.path.CalculatedPercentageOfTotalPath;
            if (pathPercentage < 0.1f) updateLocked = false;
            if (pathPercentage > updateLockUpperLimit && !updateLocked) {
                // Debug.Log("NOW");
                updateLocked = true;
                splineIndex += 1;
            }
        }
    }
}