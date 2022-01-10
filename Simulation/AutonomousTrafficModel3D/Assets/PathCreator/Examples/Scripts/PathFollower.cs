﻿using UnityEngine;

namespace PathCreation.Examples {
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour {
        // xz plane: (90,0,90)
        [SerializeField] [Range(-360, 360)] public float additionalXRotation = 90;
        [SerializeField] [Range(-360, 360)] public float additionalYRotation = 90;
        [SerializeField] [Range(-360, 360)] public float additionalZRotation;
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        private float distanceTravelled;

        private void Start() {
            if (pathCreator != null)
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
        }

        private void Update() {
            if (pathCreator != null) {
                distanceTravelled += speed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                // transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                var initial = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                initial *= Quaternion.Euler(additionalXRotation, additionalYRotation, additionalZRotation);
                transform.rotation = initial;
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        private void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}