using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using PathCreator.Vehicles;
using UnityEngine;

public interface IVehicle {
    Direction ClosestTurnDirection { get; set; }
}

public class VehicleTimeBased : MonoBehaviour, IVehicle {
    [HideInInspector] public VehicleTimeBasedFollower follower;
    public Direction closestTurnDirection = Direction.Unknown;
    public PathNode startNode;
    private PathProviderMethod providerMethod;
    [HideInInspector] public IVehiclePathProvider vehiclePathProvider; // TODO: Change to interface

    public PathProviderMethod ProviderMethod
    {
        get => providerMethod;
        set
        {
            InstantiateComponents();
            providerMethod = value;
            vehiclePathProvider.CurrentMethod = providerMethod;
            SetNewPointsToFollow();
        }
    }


    // Start is called before the first frame update
    private void Reset() {
        Start();
    }

    private void Start() {
        InstantiateComponents();
        SetNewPointsToFollow();
    }

    private void OnValidate() {
        Start();
    }

    public Direction ClosestTurnDirection
    {
        get => closestTurnDirection;
        set => closestTurnDirection = value;
    }

    private void InstantiateComponents() {
        vehiclePathProvider = gameObject.InstantiateComponent<VehiclePathProvider>();
        follower = gameObject.InstantiateComponent<VehicleTimeBasedFollower>();
    }

    private void SetNewPointsToFollow() {
        follower.PointsToFollow = new List<Transform>();
        var list = vehiclePathProvider.Provide(startNode).Select(_ => _.transform).ToList();
        follower.PointsToFollow = list;
    }
}