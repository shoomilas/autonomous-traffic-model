#region "Imports"

using System;
using UnityEngine;
using UnityEngine.Serialization;

#endregion


namespace RoadArchitect {
    [Serializable]
    public class TrafficLightController {
        //Enums for controller:
        public enum iLightControllerEnum {
            Regular,
            LeftTurn,
            MasterLeft1,
            MasterLeft2,
            Red
        }

        //Enums for actual lights:
        public enum iLightStatusEnum {
            Regular,
            LeftTurn,
            MasterLeft,
            Red,
            RightTurn
        }

        public enum iLightSubStatusEnum {
            Green,
            Yellow,
            Red
        }

        public enum iLightYieldSubStatusEnum {
            Green,
            Yellow,
            Red,
            YellowTurn,
            GreenTurn
        }


        #region "Constructor"

        public TrafficLightController(ref GameObject _LightLeft, ref GameObject _LightRight, ref GameObject[] _Lights,
            ref MeshRenderer _MR_Left, ref MeshRenderer _MR_Right, ref MeshRenderer[] MR_Mains) {
            lightLeftObject = _LightLeft;
            lightRightObject = _LightRight;
            lightsObjects = _Lights;

            leftMR = _MR_Left;
            rightMR = _MR_Right;
            mainMRStorage = MR_Mains;
            mainMR = MR_Mains[0];

            Light[] tLights;
            if (lightLeftObject != null) {
                tLights = lightLeftObject.transform.GetComponentsInChildren<Light>();
                foreach (var tLight in tLights) {
                    if (tLight.transform.name.ToLower().Contains("redlight")) lightLeftR = tLight;
                    if (tLight.transform.name.ToLower().Contains("yellowlight")) lightLeftY = tLight;
                    if (tLight.transform.name.ToLower().Contains("greenl")) lightLeftG = tLight;
                }
            }

            if (lightRightObject != null) {
                tLights = lightRightObject.transform.GetComponentsInChildren<Light>();
                foreach (var tLight in tLights) {
                    if (tLight.transform.name.ToLower().Contains("redlight")) lightRightR = tLight;
                    if (tLight.transform.name.ToLower().Contains("yellowlight")) lightRightY = tLight;
                    if (tLight.transform.name.ToLower().Contains("greenl")) lightRightG = tLight;
                }
            }

            var mCount = lightsObjects.Length;
            lightsR = new Light[mCount];
            lightsY = new Light[mCount];
            lightsG = new Light[mCount];
            for (var index = 0; index < mCount; index++) {
                tLights = lightsObjects[index].transform.GetComponentsInChildren<Light>();
                foreach (var tLight in tLights) {
                    if (tLight.transform.name.ToLower().Contains("redlight")) lightsR[index] = tLight;
                    if (tLight.transform.name.ToLower().Contains("yellowlight")) lightsY[index] = tLight;
                    if (tLight.transform.name.ToLower().Contains("greenl")) lightsG[index] = tLight;
                }
            }
        }

        #endregion


        #region "Update"

        public void UpdateLights(iLightStatusEnum _lightStatus, iLightSubStatusEnum _lightSubStatus,
            bool _isLightsEnabled) {
            isLightsEnabled = _isLightsEnabled;
            lightStatus = _lightStatus;
            lightSubStatus = _lightSubStatus;
            isUsingSharedMaterial = false;
            switch (lightStatus) {
                case iLightStatusEnum.Regular:
                    TriggerRegular();
                    break;
                case iLightStatusEnum.LeftTurn:
                    TriggerLeftTurn();
                    break;
                case iLightStatusEnum.MasterLeft:
                    TriggerMasterLeft();
                    break;
                case iLightStatusEnum.Red:
                    TriggerRed();
                    break;
                case iLightStatusEnum.RightTurn:
                    TriggerRightTurn();
                    break;
            }
        }

        #endregion


        /// <summary> Changes _MR mainTextureOffset of the material based on _lightYieldSub </summary>
        private void MRChange(ref MeshRenderer _MR, iLightSubStatusEnum _lightSub) {
            Material meshMaterial;

            if (isUsingSharedMaterial)
                meshMaterial = _MR.sharedMaterial;
            else
                meshMaterial = _MR.material;


            if (_lightSub == iLightSubStatusEnum.Green)
                meshMaterial.mainTextureOffset = new Vector2(0.667f, 0f);
            else if (_lightSub == iLightSubStatusEnum.Yellow)
                meshMaterial.mainTextureOffset = new Vector2(0.334f, 0f);
            else if (_lightSub == iLightSubStatusEnum.Red) meshMaterial.mainTextureOffset = new Vector2(0f, 0f);
        }


        /// <summary> Changes _MR mainTextureOffset of the material based on _lightYieldSub </summary>
        private void MRChangeLeftYield(ref MeshRenderer _MR, iLightYieldSubStatusEnum _lightYieldSub) {
            Material meshMaterial;

            if (isUsingSharedMaterial)
                meshMaterial = _MR.sharedMaterial;
            else
                meshMaterial = _MR.material;


            if (_lightYieldSub == iLightYieldSubStatusEnum.Green)
                meshMaterial.mainTextureOffset =
                    isUsingSharedMaterial ? new Vector2(0.667f, 0f) : new Vector2(0.4f, 0f);
            else if (_lightYieldSub == iLightYieldSubStatusEnum.Yellow)
                meshMaterial.mainTextureOffset =
                    isUsingSharedMaterial ? new Vector2(0.334f, 0f) : new Vector2(0.2f, 0f);
            else if (_lightYieldSub == iLightYieldSubStatusEnum.Red)
                meshMaterial.mainTextureOffset = new Vector2(0f, 0f);
            else if (_lightYieldSub == iLightYieldSubStatusEnum.YellowTurn)
                meshMaterial.mainTextureOffset = new Vector2(0.6f, 0f);
            else if (_lightYieldSub == iLightYieldSubStatusEnum.GreenTurn)
                meshMaterial.mainTextureOffset = new Vector2(0.8f, 0f);
        }


        /// <summary> Change lights to current light status </summary>
        private void LightChange(int _index, iLightSubStatusEnum _lightSub) {
            if (!isLightsEnabled) {
                var meshCount = mainMRStorage.Length;
                for (var index = 0; index < meshCount; index++) {
                    lightsR[index].enabled = false;
                    lightsY[index].enabled = false;
                    lightsG[index].enabled = false;
                }

                if (lightLeftR != null) lightLeftR.enabled = false;
                if (lightLeftY != null) lightLeftY.enabled = false;
                if (lightLeftG != null) lightLeftG.enabled = false;
                if (lightRightR != null) lightRightR.enabled = false;
                if (lightRightY != null) lightRightY.enabled = false;
                if (lightRightG != null) lightRightG.enabled = false;
                return;
            }

            if (_index == 0) {
                //Main:
                var mCount = mainMRStorage.Length;
                for (var index = 0; index < mCount; index++)
                    LightChangeHelper(ref lightsR[index], ref lightsY[index], ref lightsG[index], _lightSub);
            }
            else if (_index == 1) {
                //Left:
                LightChangeHelper(ref lightLeftR, ref lightLeftY, ref lightLeftG, _lightSub);
            }
            else if (_index == 2) {
                //Right:
                LightChangeHelper(ref lightRightR, ref lightRightY, ref lightRightG, _lightSub);
            }
        }


        /// <summary> Change active light </summary>
        private void LightChangeHelper(ref Light _red, ref Light _yellow, ref Light _green,
            iLightSubStatusEnum _lightSub) {
            if (_lightSub == iLightSubStatusEnum.Green) {
                _red.enabled = false;
                _yellow.enabled = false;
                _green.enabled = true;
            }
            else if (_lightSub == iLightSubStatusEnum.Yellow) {
                _red.enabled = false;
                _yellow.enabled = true;
                _green.enabled = false;
            }
            else if (_lightSub == iLightSubStatusEnum.Red) {
                _red.enabled = true;
                _yellow.enabled = false;
                _green.enabled = false;
            }
        }


        #region "Vars"

        [FormerlySerializedAs("LightLeftObj")] public GameObject lightLeftObject;

        [FormerlySerializedAs("LightRightObj")]
        public GameObject lightRightObject;

        [FormerlySerializedAs("LightsObj")] public GameObject[] lightsObjects;

        [FormerlySerializedAs("MR_Left")] public MeshRenderer leftMR;

        [FormerlySerializedAs("MR_Right")] public MeshRenderer rightMR;

        [FormerlySerializedAs("MR_MainsStorage")]
        public MeshRenderer[] mainMRStorage;

        [FormerlySerializedAs("MR_Main")] public MeshRenderer mainMR;

        [FormerlySerializedAs("LightLeft_R")] public Light lightLeftR;

        [FormerlySerializedAs("LightLeft_Y")] public Light lightLeftY;

        [FormerlySerializedAs("LightLeft_G")] public Light lightLeftG;

        [FormerlySerializedAs("LightRight_R")] public Light lightRightR;

        [FormerlySerializedAs("LightRight_Y")] public Light lightRightY;

        [FormerlySerializedAs("LightRight_G")] public Light lightRightG;

        [FormerlySerializedAs("Lights_R")] public Light[] lightsR;

        [FormerlySerializedAs("Lights_Y")] public Light[] lightsY;

        [FormerlySerializedAs("Lights_G")] public Light[] lightsG;

        [FormerlySerializedAs("iLightStatus")] public iLightStatusEnum lightStatus = iLightStatusEnum.Red;

        [FormerlySerializedAs("iLightSubStatus")]
        public iLightSubStatusEnum lightSubStatus = iLightSubStatusEnum.Green;

        [FormerlySerializedAs("bLeft")] private bool isLeft;

        [FormerlySerializedAs("bRight")] private bool isRight;

        [FormerlySerializedAs("bMain")] private bool isMain;

        [FormerlySerializedAs("bUseSharedMaterial")]
        private bool isUsingSharedMaterial;

        [FormerlySerializedAs("bLeftTurnYieldOnGreen")]
        private bool isLeftTurnYieldOnGreen = true;

        [FormerlySerializedAs("bLightsEnabled")]
        private bool isLightsEnabled = true;

        #endregion


        #region "Triggers"

        private void TriggerRegular() {
            if (isMain) {
                MRChange(ref mainMR, lightSubStatus);
                for (var index = 1; index < mainMRStorage.Length; index++)
                    MRChange(ref mainMRStorage[index], lightSubStatus);
                LightChange(0, lightSubStatus);
            }

            if (isLeft) {
                if (isLeftTurnYieldOnGreen) {
                    if (lightSubStatus == iLightSubStatusEnum.Green)
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.Green);
                    else if (lightSubStatus == iLightSubStatusEnum.Yellow)
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.Yellow);
                }
                else {
                    MRChange(ref leftMR, iLightSubStatusEnum.Red);
                    LightChange(1, iLightSubStatusEnum.Red);
                }
            }

            if (isRight) {
                MRChange(ref rightMR, iLightSubStatusEnum.Red);
                LightChange(2, iLightSubStatusEnum.Red);
            }
        }


        private void TriggerLeftTurn() {
            if (isMain) {
                MRChange(ref mainMR, iLightSubStatusEnum.Red);
                for (var i = 1; i < mainMRStorage.Length; i++) MRChange(ref mainMRStorage[i], iLightSubStatusEnum.Red);
                LightChange(0, iLightSubStatusEnum.Red);
            }

            if (isLeft) {
                if (isLeftTurnYieldOnGreen) {
                    if (lightSubStatus == iLightSubStatusEnum.Green)
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.GreenTurn);
                    else if (lightSubStatus == iLightSubStatusEnum.Yellow)
                        MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.YellowTurn);
                    LightChange(1, lightSubStatus);
                }
                else {
                    MRChange(ref leftMR, lightSubStatus);
                    LightChange(1, lightSubStatus);
                }
            }

            if (isRight) {
                MRChange(ref rightMR, iLightSubStatusEnum.Red);
                LightChange(2, iLightSubStatusEnum.Red);
            }
        }


        private void TriggerMasterLeft() {
            if (isMain) {
                MRChange(ref mainMR, lightSubStatus);
                for (var index = 1; index < mainMRStorage.Length; index++)
                    MRChange(ref mainMRStorage[index], lightSubStatus);
                LightChange(0, lightSubStatus);
            }

            if (isLeft) {
                if (lightSubStatus == iLightSubStatusEnum.Green)
                    MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.GreenTurn);
                else if (lightSubStatus == iLightSubStatusEnum.Yellow)
                    MRChangeLeftYield(ref leftMR, iLightYieldSubStatusEnum.YellowTurn);
                LightChange(1, lightSubStatus);
            }

            if (isRight) {
                MRChange(ref rightMR, lightSubStatus);
                LightChange(2, lightSubStatus);
            }
        }


        private void TriggerRightTurn() {
            if (isMain) {
                MRChange(ref mainMR, iLightSubStatusEnum.Red);
                for (var index = 1; index < mainMRStorage.Length; index++)
                    MRChange(ref mainMRStorage[index], iLightSubStatusEnum.Red);
                LightChange(0, iLightSubStatusEnum.Red);
            }

            if (isLeft) {
                MRChange(ref leftMR, iLightSubStatusEnum.Red);
                LightChange(1, iLightSubStatusEnum.Red);
            }

            if (isRight) {
                MRChange(ref rightMR, lightSubStatus);
                LightChange(2, lightSubStatus);
            }
        }


        private void TriggerRed() {
            if (isMain) {
                MRChange(ref mainMR, iLightSubStatusEnum.Red);
                for (var index = 1; index < mainMRStorage.Length; index++)
                    MRChange(ref mainMRStorage[index], iLightSubStatusEnum.Red);
                LightChange(0, iLightSubStatusEnum.Red);
            }

            if (isLeft) {
                MRChange(ref leftMR, iLightSubStatusEnum.Red);
                LightChange(1, iLightSubStatusEnum.Red);
            }

            if (isRight) {
                MRChange(ref rightMR, iLightSubStatusEnum.Red);
                LightChange(2, iLightSubStatusEnum.Red);
            }
        }

        #endregion


        #region "Setup"

        public void Setup(bool _isLeftYield) {
            SetupMainObjects();
            isLeft = leftMR != null;
            isRight = rightMR != null;
            isMain = mainMR != null;
            isLeftTurnYieldOnGreen = _isLeftYield;
        }


        private void SetupMainObjects() {
            if (mainMR == null) return;
            var meshCount = mainMRStorage.Length;
            if (meshCount <= 1) return;
            if (isUsingSharedMaterial) {
                for (var index = 1; index < meshCount; index++)
                    mainMRStorage[index].sharedMaterial = mainMR.sharedMaterial;
            }
            else {
                var materials = new Material[1];
                materials[0] = mainMR.materials[0];
                for (var index = 1; index < meshCount; index++) mainMRStorage[index].materials = materials;
            }
        }

        #endregion

    }
}