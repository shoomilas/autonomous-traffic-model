using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class VehiclePointsListFollower : MonoBehaviour
{
    [SerializeField] public Transform targetPositionTranform;

    private VehicleController carDriver;
    public  Vector3 targetPosition;
    
    public float forwardAmount = 0f;
    public float turnAmount = 0f;
    public float reachedTargetDistance = 3.5f; // Should be around half of length of the prefab
    public float stoppingDistance = 30f; 
    public float stoppingSpeed = 40f;
    public float onReachedTargetBreakEngageSpeed = 15f;

    public List<Transform> PointsToFollow;
    public Transform CurrentPoint;
    public DriveStatus CurrentDriveStatus;

    [Serializable]
    public enum DriveStatus {
        Start,
        Driving,
        WayPointReached,
        Finished
    }
    
    void Awake()
    {
        carDriver = GetComponent<VehicleController>();
        CurrentDriveStatus = DriveStatus.Start;
    }

    void SetCurrentPointToFollow() {
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

    void Drive() {
        IterationPrep();
        SetCurrentPointToFollow();
        if (CurrentDriveStatus == DriveStatus.Finished) return;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget > reachedTargetDistance) {
            OnTargetNotReachedYet();
        }
        else {
            OnTargetReached();
        }
        carDriver.SetInputs(forwardAmount, turnAmount);
    }

    void IterationPrep() {
        forwardAmount = 0f;
        turnAmount = 0f;
    }
    
    void OnTargetReached() {
        CurrentDriveStatus = DriveStatus.WayPointReached;
        // if (carDriver.GetSpeed() > onReachedTargetBreakEngageSpeed) {
        //     forwardAmount = -1f;
        // }
        // else {
        //     forwardAmount = 0f;
        //     turnAmount = 0f;   
        // }
    }

    void OnTargetNotReachedYet() {
        var dirToMovePosition = (targetPosition - transform.position).normalized;
        var dot = Vector3.Dot(transform.forward, dirToMovePosition); // dot: if >0, target's behind, if < 0: target's behind.
        
        if (dot > 0) {
            forwardAmount = 1f;
        }
        else {
            forwardAmount = -1f;
        }

        // Gets angle to the target
        float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);
        if (angleToDir > 0) {
            // turnAmount = 1f;
            turnAmount = angleToDir / 180;
        }
        else {
            turnAmount = angleToDir /180;
        }
        if(transform.name == "TestCar") { Debug.Log($"{turnAmount}->{angleToDir}"); }
    }
    void Update() {
        if (PointsToFollow != null) {
            if(PointsToFollow.Count > 0) {
                Drive();
            }
        }
    }
    public void SetTargetPosition(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
    }
}