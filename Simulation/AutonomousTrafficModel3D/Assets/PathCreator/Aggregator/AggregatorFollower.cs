using PathCreation;
using UnityEngine;

namespace PathCreator.Aggregator
{
    public class AggregatorFollower : MonoBehaviour
    {
        // xz plane: (90,0,90)
        [SerializeField, Range(-360,360)] public float additionalXRotation = 90;
        [SerializeField, Range(-360,360)] public float additionalYRotation = 90;
        [SerializeField, Range(-360,360)] public float additionalZRotation = 0;

        public PathAggregator Aggregator;
        public Transform ObjectToMove; // Or Transform?
        public float speed = 0.3f;
        
        public EndOfPathInstruction endOfPathInstruction;
        float distanceTravelled;
        public bool updateLocked = true;
        public float upperLimit = .99999f;
        // public PathCreation.PathCreator CurrentPathCreator; 
        
        void Start() {
            if (Aggregator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                // Aggregator.pathUpdated += OnPathChanged;
            }
        }

        public int splineIndex = 0;
        
        private void moveObject(PathCreation.PathCreator pathCreator) {
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                ObjectToMove.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                var initial = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                initial *= Quaternion.Euler(additionalXRotation, additionalYRotation, additionalZRotation);
                ObjectToMove.transform.rotation = initial;
            }
        }


        private void actionOnEnd(PathCreation.PathCreator pathCreator) {
            // TODO: Safe "increment once method"
            var pathPercentage = pathCreator.path.CalculatedPercentageOfTotalPath;
            if (pathPercentage < 0.1f) {
                updateLocked = false;
            }
            if ( pathPercentage > upperLimit && (!updateLocked)) {
                Debug.Log("NOW");
                updateLocked = true;
                splineIndex += 1;
            }
        }
        
        void Update()
        {
            // if(splineIndex == 0) {}
            if (splineIndex < (Aggregator.Paths.Count)) {
                Debug.Log($"Aggregator.Paths.Count: {Aggregator.Paths.Count} | SplineIndex: {splineIndex}");
                var pathCreator = Aggregator.Paths[splineIndex]; // currentPathCreator
                moveObject(pathCreator);
                actionOnEnd(pathCreator);
            }
        }
    }
}





// If the path changes during the game, update the distance travelled so that the follower's position on the new path
// is as close as possible to its position on the old path
// void OnPathChanged() {
//     distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
// }