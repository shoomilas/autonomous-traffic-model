using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreator.Aggregator;
using PathCreator.Vehicles;
using UnityEngine;

public class VehicleTimeBased : MonoBehaviour
{
    [HideInInspector] public IVehiclePathProvider vehiclePathProvider;   // TODO: Change to interface
    [HideInInspector] public VehicleTimeBasedFollower follower;
    public Direction closestTurnDirection = Direction.Unknown; 
    private PathProviderMethod providerMethod;
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
    public PathNode startNode;

    

    // Start is called before the first frame update
    void Reset() {
        Start();
    }

    private void OnValidate() {
        Start();
    }

    private void Start() {
        InstantiateComponents();
        SetNewPointsToFollow();
    }

    private void InstantiateComponents() {
        vehiclePathProvider = gameObject.InstantiateComponent<VehiclePathProvider>();
        follower = gameObject.InstantiateComponent<VehicleTimeBasedFollower>();
    }

    private void SetNewPointsToFollow() {
        follower.PointsToFollow = new List<Transform>();
        var list = vehiclePathProvider.Provide(startNode).Select(_ => _.transform ).ToList();
        follower.PointsToFollow = list;  
    }
}
