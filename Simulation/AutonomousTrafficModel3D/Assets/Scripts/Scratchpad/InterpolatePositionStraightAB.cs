using UnityEngine;

public class InterpolatePositionStraightAB : MonoBehaviour {
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform pointAB;
    private float interpolateAmount;

    private void Update() {
        interpolateAmount = (interpolateAmount + Time.deltaTime) % 1f;
        pointAB.position = Vector3.Lerp(pointA.position, pointB.position, interpolateAmount);
    }
}