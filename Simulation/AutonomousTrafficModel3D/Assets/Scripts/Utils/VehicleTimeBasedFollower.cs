using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LinqExtension {
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<int, T> action) {
        // argument null checking omitted
        var i = 0;
        foreach (var item in sequence) {
            action(i, item);
            i++;
        }
    }
}


public class VehicleTimeBasedFollower : MonoBehaviour {
    // Start is called before the first frame update
    // public Vector3 positionToMoveTo;
    public Transform thingToMoveTo;
    public List<Transform> PointsToFollow; // TODO Change to nodes?
    public float duration = 1f;
    public VehiclePointsFollower.DriveStatus CurrentDriveStatus;

    private void Start() {
        StartCoroutine(LerpToPoints(PointsToFollow, duration));
    }

    private void Update() {
        if (CurrentDriveStatus == VehiclePointsFollower.DriveStatus.Finished) return;
        if (PointsToFollow != null)
            if (PointsToFollow.Count > 0)
                Start();
    }


    private IEnumerator LerpToPoints(List<Transform> transformsToMoveTo, float totalTime) {
        var pointsCount = transformsToMoveTo.Count;

        var currentPosition = transform.position;
        var allPositions = new List<Vector3> { currentPosition }
            .Concat(transformsToMoveTo.Select(_ => _.position)).ToList();
        var positions = transformsToMoveTo.Select((t, i) => (vector: t.position, index: i)).ToList();
        var distances = positions
            .Select(point => Vector3.Distance(allPositions[point.index], allPositions[point.index + 1])).ToList();

        // distances.ToList().ForEach(d => print($"{d}"));
        var totalDistance = distances.Sum(c => c);

        // if there was a "total number" first cound perStepTime (totalSteptTime/positionCount) and then go.

        // yield break;

        var i = 0;
        foreach (var tr in transformsToMoveTo) {
            var stepDuration = totalTime / (totalDistance / distances[i]);
            Debug.Log($"{totalDistance} / {distances[i]} = {totalDistance / distances[i]} | {stepDuration}");
            i += 1;
            yield return LerpPosition(tr, stepDuration);
        }
    }

    private IEnumerator LerpPosition(Transform thingToMoveTo, float duration) {
        float time = 0;
        var targetPosition = thingToMoveTo.position;
        var startPosition = transform.position;

        while (time < duration) {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}