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
}