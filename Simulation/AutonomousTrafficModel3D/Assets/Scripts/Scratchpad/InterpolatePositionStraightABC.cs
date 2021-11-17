using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolatePositionStraightABC : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform pointC;
    [SerializeField] private Transform pointAB;
    [SerializeField] private Transform pointBC;
    private float interpolateAmount;
    
    void Update() {
        interpolateAmount = (interpolateAmount + Time.deltaTime) % 1f;
        pointAB.position = Vector3.Lerp(pointA.position, pointB.position, interpolateAmount);
        pointBC.position = Vector3.Lerp(pointB.position, pointC.position, interpolateAmount);
    }
}
