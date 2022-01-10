using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoadArchitect {
    public class WizardObject {
        [FormerlySerializedAs("Desc")] public string desc;

        [FormerlySerializedAs("DisplayName")] public string displayName;

        [FormerlySerializedAs("FileName")] public string fileName;

        [FormerlySerializedAs("FullPath")] public string FullPath;

        [FormerlySerializedAs("bIsBridge")] public bool isBridge;

        [FormerlySerializedAs("bIsDefault")] public bool isDefault;

        public int sortID = 0;

        [FormerlySerializedAs("Thumb")] public Texture2D thumb;

        [FormerlySerializedAs("ThumbString")] public string thumbString;


        public string ConvertToString() {
            var WOL = new WizardObjectLibrary();
            WOL.LoadFrom(this);
            return RootUtils.GetString<WizardObjectLibrary>(WOL);
        }


        public void LoadDataFromWOL(WizardObjectLibrary _wizardObjLib) {
            thumbString = _wizardObjLib.thumbString;
            displayName = _wizardObjLib.displayName;
            desc = _wizardObjLib.desc;
            isDefault = _wizardObjLib.isDefault;
            fileName = _wizardObjLib.fileName;
            isBridge = _wizardObjLib.isBridge;
        }


        public static WizardObject LoadFromLibrary(string _path) {
            var tData = File.ReadAllText(_path);
            var tSep = new string[2];
            tSep[0] = RoadUtility.FileSepString;
            tSep[1] = RoadUtility.FileSepStringCRLF;
            var tSplit = tData.Split(tSep, StringSplitOptions.RemoveEmptyEntries);
            var tSplitCount = tSplit.Length;
            WizardObjectLibrary WOL = null;
            for (var i = 0; i < tSplitCount; i++) {
                WOL = WizardObjectLibrary.WOLFromData(tSplit[i]);
                if (WOL != null) {
                    var WO = new WizardObject();
                    WO.LoadDataFromWOL(WOL);
                    return WO;
                }
            }

            return null;
        }


        [Serializable]
        public class WizardObjectLibrary {
            [FormerlySerializedAs("ThumbString")] public string thumbString;

            [FormerlySerializedAs("DisplayName")] public string displayName;

            [FormerlySerializedAs("Desc")] public string desc;

            [FormerlySerializedAs("bIsDefault")] public bool isDefault;

            [FormerlySerializedAs("bIsBridge")] public bool isBridge;

            [FormerlySerializedAs("FileName")] public string fileName;


            public void LoadFrom(WizardObject _wizardObj) {
                thumbString = _wizardObj.thumbString;
                displayName = _wizardObj.displayName;
                desc = _wizardObj.desc;
                isDefault = _wizardObj.isDefault;
                fileName = _wizardObj.fileName;
                isBridge = _wizardObj.isBridge;
            }


            public static WizardObjectLibrary WOLFromData(string _data) {
                try {
                    var WOL = RootUtils.LoadData<WizardObjectLibrary>(ref _data);
                    return WOL;
                }
                catch {
                    return null;
                }
            }
        }
    }
}