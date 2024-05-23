using MelonLoader;
using UnityEngine;
using RumbleModUI;
using System.Collections;
using RUMBLE.Managers;
using System;

namespace HandDistanceRecorder
{
    public class main : MelonMod
    {
        private bool init = false;
        private Vector3 lastHandPositionL, lastHandPositionR;
        private Transform handLTransform, handRTransform;
        private float distanceL = 0;
        private float distanceR = 0;
        UI UI = UI.instance;
        private Mod HandDistanceRecorder = new Mod();
        private DateTime waitTill;

        public override void OnLateInitializeMelon()
        {
            HandDistanceRecorder.ModName = "HandDistanceRecorder";
            HandDistanceRecorder.ModVersion = "1.0.0";
            HandDistanceRecorder.SetFolder("HandDistanceRecorder");
            HandDistanceRecorder.AddToList("Description", ModSetting.AvailableTypes.Description, "", "Hand Distances Recorded:" + Environment.NewLine + "Left: 0" + Environment.NewLine + "Right: 0");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            init = false;
            waitTill = DateTime.Now.AddSeconds(1);
            if (sceneName != "Loader") { MelonCoroutines.Start(SetInitialHandPositions()); }
        }

        public override void OnUpdate()
        {
            if (!init) { return; }
            if (DateTime.Now < waitTill)
            {
                lastHandPositionL = handLTransform.position;
                lastHandPositionR = handRTransform.position;
                return;
            }
            distanceL += Vector3.Distance(lastHandPositionL, handLTransform.position);
            distanceR += Vector3.Distance(lastHandPositionR, handRTransform.position);
            lastHandPositionL = handLTransform.position;
            lastHandPositionR = handRTransform.position;
            HandDistanceRecorder.Settings[0].Description = "Hand Distances Recorded:" + Environment.NewLine + "Left: " + distanceL.ToString("0.###") + " Units" + Environment.NewLine + "Right: " + distanceR.ToString("0.###") + " Units";
        }

        public override void OnFixedUpdate()
        {
            if (UI.GetInit() && !HandDistanceRecorder.GetUIStatus()) { UI.AddMod(HandDistanceRecorder); }
        }

        public IEnumerator SetInitialHandPositions()
        {
            bool gotHands = false;
            while (!gotHands)
            {
                try
                {
                    GameObject VR = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(1).gameObject;
                    handLTransform = VR.transform.GetChild(1);
                    handRTransform = VR.transform.GetChild(2);
                    lastHandPositionL = handLTransform.localPosition;
                    lastHandPositionR = handRTransform.localPosition;
                    if (handLTransform.position != null && handRTransform.position != null) { gotHands = true; }
                }
                catch { }
                if (!gotHands)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            init = true;
        }
    }
}
