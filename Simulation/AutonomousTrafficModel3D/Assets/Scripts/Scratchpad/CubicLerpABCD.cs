using UnityEngine;

public class CubicLerpABCD : MonoBehaviour {
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform pointC;
    [SerializeField] private Transform pointD;
    [SerializeField] private Transform pointABCD;
    private float interpolateAmount;

    private void Update() {
        interpolateAmount = (interpolateAmount + Time.deltaTime) % 1f;
        pointABCD.position = CubicLerp(pointA.position, pointB.position, pointC.position, pointD.position,
            interpolateAmount);
    }

    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t) {
        var ab = Vector3.Lerp(a, b, t);
        var bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, interpolateAmount);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) {
        var ab_bc = QuadraticLerp(a, b, c, t);
        var bc_cd = QuadraticLerp(b, c, d, t);
        return Vector3.Lerp(ab_bc, bc_cd, interpolateAmount);
    }
}