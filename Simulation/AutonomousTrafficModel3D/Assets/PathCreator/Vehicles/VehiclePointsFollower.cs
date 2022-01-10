using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehiclePointsFollower : MonoBehaviour {

    [Serializable]
    public enum DriveStatus {
        Start,
        Driving,
        WayPointReached,
        Finished
    }

    [SerializeField] public Transform targetPositionTranform;
    public Vector3 targetPosition;

    public float forwardAmount;
    public float turnAmount;
    public float reachedTargetDistance = 3.5f; // Should be around half of length of the prefab
    public float stoppingDistance = 30f;
    public float stoppingSpeed = 40f;
    public float onReachedTargetBreakEngageSpeed = 15f;

    public List<Transform> PointsToFollow;
    public Transform CurrentPoint;
    public DriveStatus CurrentDriveStatus;

    private VehicleController carDriver;

    private void Awake() {
        carDriver = GetComponent<VehicleController>();
        CurrentDriveStatus = DriveStatus.Start;
    }

    private void Update() {
        if (PointsToFollow != null)
            if (PointsToFollow.Count > 0)
                Drive();
    }

    private void SetCurrentPointToFollow() {
        Transform GetFirstWayPoint() {
            return PointsToFollow.First();
        }

        Transform GetNextWayPoint() {
            return PointsToFollow.SkipWhile(x => x != CurrentPoint).Skip(1).FirstOrDefault();
        }

        switch (CurrentDriveStatus) {
            case DriveStatus.Start:
                CurrentPoint = GetFirstWayPoint();
                break;
            case DriveStatus.WayPointReached:
                CurrentPoint = GetNextWayPoint();
                break;
            case DriveStatus.Finished:
                CurrentPoint = null;
                return;
        }

        if (CurrentPoint is null) {
            CurrentDriveStatus = DriveStatus.Finished;
            carDriver.StopCompletely();
            return;
        }

        CurrentDriveStatus = DriveStatus.Driving;
        SetTargetPosition(CurrentPoint.position);
    }

    private void Drive() {
        IterationPrep();
        SetCurrentPointToFollow();
        if (CurrentDriveStatus == DriveStatus.Finished) return;
        var distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget > reachedTargetDistance)
            OnTargetNotReachedYet();
        else
            OnTargetReached();
        carDriver.SetInputs(forwardAmount, turnAmount);
    }

    private void IterationPrep() {
        forwardAmount = 0f;
        turnAmount = 0f;
    }

    private void OnTargetReached() {
        CurrentDriveStatus = DriveStatus.WayPointReached;
        // if (carDriver.GetSpeed() > onReachedTargetBreakEngageSpeed) {
        //     forwardAmount = -1f;
        // }
        // else {
        //     forwardAmount = 0f;
        //     turnAmount = 0f;   
        // }
    }

    private void OnTargetNotReachedYet() {
        var dirToMovePosition = (targetPosition - transform.position).normalized;
        var dot = Vector3.Dot(transform.forward,
            dirToMovePosition); // dot: if >0, target's behind, if < 0: target's behind.

        if (dot > 0) forwardAmount = 1f;
        // forwardAmount = 0.9f + dot/10;
        else forwardAmount = -1f;
        // forwardAmount = -0.9f - dot/10;

        // Gets angle to the target
        var angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);
        if (angleToDir > 0) // turnAmount = 1f;
            turnAmount = angleToDir / 180;
        else
            turnAmount = angleToDir / 180;
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
    }
}