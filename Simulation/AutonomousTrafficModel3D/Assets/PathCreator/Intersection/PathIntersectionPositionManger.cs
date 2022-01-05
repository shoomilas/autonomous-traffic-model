using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using PathCreator.Aggregator;
using PathCreator.Intersection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public class IntersectionSides {
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public Vector3 D;
}

[Serializable]
public class IntersectionCorners {
    public Vector3 AB;
    public Vector3 BC;
    public Vector3 CD;
    public Vector3 DA;
}

[Serializable]
public class IntersectionInsOuts {
    public List<Vector3> InsA;
    public List<Vector3> InsB;
    public List<Vector3> InsC;
    public List<Vector3> InsD;
    public List<Vector3> OutsA;
    public List<Vector3> OutsB;
    public List<Vector3> OutsC;
    public List<Vector3> OutsD;
}

public class PathIntersectionPositionManger : MonoBehaviour
{
    // gets intersection component, computes stuff based on that
    public PathIntersection Intersection;
    public Vector3 Center;
    public float Size;
    public IntersectionSides Sides;
    public IntersectionCorners Corners;
    public IntersectionInsOuts InsOuts;

    private void Start() {
        PrepData();
    }

    public void PrepData() {
        Center = Intersection.transform.position;
        Size = Intersection.size;
        Sides = ComputeSides();
        Corners = ComputeCorners();
        InsOuts = ComputeInsOuts();
    }
    
    public IntersectionSides ComputeSides() {
        var (a, b, c, d) = (
            Center + Vector3.left * Size,
            Center + Vector3.back * Size,
            Center + Vector3.right * Size,
            Center + Vector3.forward * Size
        );
        return new IntersectionSides()
        {
            A = a,
            B = b,
            C = c,
            D = d
        };
    }
    
    public IntersectionInsOuts ComputeInsOuts() {
        // TODO: Current assumption is that every intersection has the same number of lanes on each entry lane
        // In case of ever extending this to different types of intersections:
        // 1. adjust size of the intersection 2. Accomodate the difference in Sides and Corners computation
        // 3. Change this method as well.
        
        List<List<PathNode>> insAndOuts = new List<List<PathNode>>()
        {
            Intersection.InputsA,
            Intersection.InputsB,
            Intersection.InputsC,
            Intersection.InputsD,
            Intersection.OutputsA,
            Intersection.OutputsB,
            Intersection.OutputsC,
            Intersection.OutputsD,
        };

        var goo = new List<PathNode>();
        var pointsCount = (insAndOuts
            .OrderByDescending(oneSideInsOrOuts => oneSideInsOrOuts.Count)
            .FirstOrDefault() ?? new List<PathNode>()).Count();
        var distance = Size / (pointsCount + 1);

        return new IntersectionInsOuts
        {
            InsA = GeneratePathNodePositions(pointsCount, distance, Sides.A, Vector3.back),
            InsB = GeneratePathNodePositions(pointsCount, distance, Sides.B, Vector3.right),
            InsC = GeneratePathNodePositions(pointsCount, distance, Sides.C, Vector3.forward),
            InsD = GeneratePathNodePositions(pointsCount, distance, Sides.D, Vector3.left),
            OutsA = GeneratePathNodePositions(pointsCount, distance, Sides.A, Vector3.forward),
            OutsB = GeneratePathNodePositions(pointsCount, distance, Sides.B, Vector3.left),
            OutsC = GeneratePathNodePositions(pointsCount, distance, Sides.C, Vector3.back),
            OutsD = GeneratePathNodePositions(pointsCount, distance, Sides.D, Vector3.right)
        };
    }
    
    public IntersectionCorners ComputeCorners() {
        var (ab, bc, cd, da) = (
            Sides.A + Vector3.back * Size,
            Sides.B + Vector3.right * Size,
            Sides.C + Vector3.forward * Size,
            Sides.D + Vector3.left * Size
        );
        return new IntersectionCorners()
        {
            AB = ab,
            BC = bc,
            CD = cd,
            DA = da
        };
    }
    
    private List<Vector3> GeneratePathNodePositions(int pointsCount, float distance, Vector3 srcPoint, Vector3 direction) {
        var positions = new List<Vector3>();
        var currentPos = srcPoint;
        for (int i = 0; i < pointsCount; i++) {
            positions.Add(srcPoint + direction * distance);
        }

        return positions;
    }
}