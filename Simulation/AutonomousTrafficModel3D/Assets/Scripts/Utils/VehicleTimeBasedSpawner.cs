using System.Collections;
using System.Linq;
using PathCreator.Aggregator;
using PathCreator.Vehicles;
using UnityEngine;

public class VehicleTimeBasedSpawner : MonoBehaviour {
    [Range(.1f, 10)] public float gizmoSize = 2.5f;
    public PathProviderMethod providerMethod = PathProviderMethod.AlwaysRandomRightForward;
    public bool recurring;
    public int hardInstantiationLimit = 5;

    [InspectorName("Offset (Initial Delay)")] [Range(0f, 10)]
    public float offset;

    [Range(.1f, 10)] public float interval = 1;
    public GameObject vehiclePrefab;
    public Direction Direction;

    private void Start() {
        StartCoroutine(InstantiatorWithPathProviderMethod());
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0, 0, 1, 0.4f);
        var position = transform.position;
        Gizmos.DrawSphere(position, gizmoSize);
    }

    private Quaternion PrepVehicleSpawnQuaternion() {
        var spawnNode = gameObject.InstantiateComponent<PathNode>();
        var spawnPoint = transform.position;
        var nextPoint = spawnNode.nextPathNodes.First().transform.position;
        var difference = nextPoint - spawnPoint;
        var quaternion = Quaternion.LookRotation(difference, Vector3.up);
        return quaternion;
    }

    private IEnumerator InstantiatorWithPathProviderMethod() {
        var spawnHeight = 0.05f;
        var position = transform.position;
        var quaternion = PrepVehicleSpawnQuaternion();
        for (var i = 0; i < hardInstantiationLimit; i++) {
            var vehicle = Instantiate(vehiclePrefab, position + Vector3.one * spawnHeight, quaternion);
            var vehicleComponent = vehicle.InstantiateComponent<VehicleTimeBased>();

            // TODO: make it actually  base off current position and path provider's result
            vehicleComponent.closestTurnDirection =
                EnumExtensions.EnumGetRandomValue(Direction.Left, Direction.Right, Direction.Forward);
            vehicleComponent.startNode = gameObject.GetComponent<PathNode>();
            vehicleComponent.ProviderMethod = providerMethod;

            if (!recurring) yield break;

            yield return new WaitForSeconds(interval);
        }
    }
}