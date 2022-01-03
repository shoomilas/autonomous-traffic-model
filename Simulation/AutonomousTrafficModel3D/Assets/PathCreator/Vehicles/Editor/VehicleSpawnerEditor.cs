using UnityEditor;

namespace PathCreator.Vehicles.Editor {
    [CustomEditor(typeof(VehicleSpawner))]
    public class VehicleSpawnerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }
}