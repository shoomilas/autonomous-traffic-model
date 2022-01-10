/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMonkey.Utils {

    /*
     * Executes a Function periodically
     * */
    public class FunctionPeriodic {


        private static List<FunctionPeriodic> funcList; // Holds a reference to all active timers

        private static GameObject
            initGameObject; // Global game object used for initializing class, is destroyed on scene change

        public Action action;
        private readonly float baseTimer;
        private readonly string functionName;


        private readonly GameObject gameObject;
        public Func<bool> testDestroy;
        private float timer;
        private readonly bool useUnscaledDeltaTime;


        private FunctionPeriodic(GameObject gameObject, Action action, float timer, Func<bool> testDestroy,
            string functionName, bool useUnscaledDeltaTime) {
            this.gameObject = gameObject;
            this.action = action;
            this.timer = timer;
            this.testDestroy = testDestroy;
            this.functionName = functionName;
            this.useUnscaledDeltaTime = useUnscaledDeltaTime;
            baseTimer = timer;
        }

        private static void InitIfNeeded() {
            if (initGameObject == null) {
                initGameObject = new GameObject("FunctionPeriodic_Global");
                funcList = new List<FunctionPeriodic>();
            }
        }


        // Persist through scene loads
        public static FunctionPeriodic Create_Global(Action action, Func<bool> testDestroy, float timer) {
            var functionPeriodic = Create(action, testDestroy, timer, "", false, false, false);
            Object.DontDestroyOnLoad(functionPeriodic.gameObject);
            return functionPeriodic;
        }


        // Trigger [action] every [timer], execute [testDestroy] after triggering action, destroy if returns true
        public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer) {
            return Create(action, testDestroy, timer, "", false);
        }

        public static FunctionPeriodic Create(Action action, float timer) {
            return Create(action, null, timer, "", false, false, false);
        }

        public static FunctionPeriodic Create(Action action, float timer, string functionName) {
            return Create(action, null, timer, functionName, false, false, false);
        }

        public static FunctionPeriodic Create(Action callback, Func<bool> testDestroy, float timer, string functionName,
            bool stopAllWithSameName) {
            return Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);
        }

        public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer, string functionName,
            bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName) {
            InitIfNeeded();

            if (stopAllWithSameName) StopAllFunc(functionName);

            var gameObject = new GameObject("FunctionPeriodic Object " + functionName, typeof(MonoBehaviourHook));
            var functionPeriodic = new FunctionPeriodic(gameObject, action, timer, testDestroy, functionName,
                useUnscaledDeltaTime);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionPeriodic.Update;

            funcList.Add(functionPeriodic);

            if (triggerImmediately) action();

            return functionPeriodic;
        }


        public static void RemoveTimer(FunctionPeriodic funcTimer) {
            InitIfNeeded();
            funcList.Remove(funcTimer);
        }

        public static void StopTimer(string _name) {
            InitIfNeeded();
            for (var i = 0; i < funcList.Count; i++)
                if (funcList[i].functionName == _name) {
                    funcList[i].DestroySelf();
                    return;
                }
        }

        public static void StopAllFunc(string _name) {
            InitIfNeeded();
            for (var i = 0; i < funcList.Count; i++)
                if (funcList[i].functionName == _name) {
                    funcList[i].DestroySelf();
                    i--;
                }
        }

        public static bool IsFuncActive(string name) {
            InitIfNeeded();
            for (var i = 0; i < funcList.Count; i++)
                if (funcList[i].functionName == name)
                    return true;
            return false;
        }

        public void SkipTimerTo(float timer) {
            this.timer = timer;
        }

        private void Update() {
            if (useUnscaledDeltaTime)
                timer -= Time.unscaledDeltaTime;
            else
                timer -= Time.deltaTime;
            if (timer <= 0) {
                action();
                if (testDestroy != null && testDestroy()) //Destroy
                    DestroySelf();
                else //Repeat
                    timer += baseTimer;
            }
        }

        public void DestroySelf() {
            RemoveTimer(this);
            if (gameObject != null) Object.Destroy(gameObject);
        }

        /*
         * Class to hook Actions into MonoBehaviour
         * */
        private class MonoBehaviourHook : MonoBehaviour {

            public Action OnUpdate;

            private void Update() {
                if (OnUpdate != null) OnUpdate();
            }
        }
    }
}