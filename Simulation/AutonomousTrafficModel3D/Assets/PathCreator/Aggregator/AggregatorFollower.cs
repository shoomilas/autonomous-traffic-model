﻿using PathCreation;
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

        void Start() {
            if (Aggregator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                // Aggregator.pathUpdated += OnPathChanged;
            }
        }

        public int splineIndex = 0;
        void Update()
        {
            var pathCreator = Aggregator.Paths[splineIndex]; // currentPathCreator
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                ObjectToMove.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                
                var initial = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                initial *= Quaternion.Euler(additionalXRotation, additionalYRotation, additionalZRotation);
                ObjectToMove.transform.rotation = initial;
            }
            // check if end.
            // var vertexCount = pathCreator.path.NumPoints;
            // var lastPosition = pathCreator.path.GetPoint(vertexCount-1);
            // Debug.Log(lastPosition);

            var count = pathCreator.bezierPath.NumPoints;
            var lastPosition = pathCreator.bezierPath.GetPoint(count-1);
            Debug.Log(pathCreator.path.GetClosestPointOnPath(ObjectToMove.transform.position) );

            if (pathCreator.path.GetClosestPointOnPath(ObjectToMove.transform.position) == lastPosition)
            {
                Debug.Log("NOW");
            }
            
            var distance = Vector3.Distance (lastPosition, ObjectToMove.transform.position);
            // Debug.Log(distance);
            if (distance < .2f)
            {
                Debug.Log("NOW");
            }

            // Debug.Log($"BOO: {ObjectToMove.transform.position}");
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        // void OnPathChanged() {
        //     distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        // }
    }
}