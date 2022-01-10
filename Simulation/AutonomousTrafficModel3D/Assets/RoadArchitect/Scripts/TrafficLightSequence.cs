using UnityEngine.Serialization;

namespace RoadArchitect {
    public class TrafficLightSequence {
        [FormerlySerializedAs("bLightMasterPath1")]
        public bool isLightMasterPath1 = true;

        [FormerlySerializedAs("iLightController")]
        public TrafficLightController.iLightControllerEnum lightController =
            TrafficLightController.iLightControllerEnum.Regular;

        [FormerlySerializedAs("iLightSubcontroller")]
        public TrafficLightController.iLightSubStatusEnum lightSubcontroller =
            TrafficLightController.iLightSubStatusEnum.Green;

        [FormerlySerializedAs("tTime")] public float time = 10f;


        public TrafficLightSequence(bool _isPath1, TrafficLightController.iLightControllerEnum _lightController,
            TrafficLightController.iLightSubStatusEnum _lightSubcontroller, float _time) {
            isLightMasterPath1 = _isPath1;
            lightController = _lightController;
            lightSubcontroller = _lightSubcontroller;
            time = _time;
        }


        public string ToStringRA() {
            return "Path1: " + isLightMasterPath1 + " iLightController: " + lightController + " iLightSubcontroller: " +
                   lightSubcontroller + " tTime: " + time.ToString("0F");
        }
    }
}