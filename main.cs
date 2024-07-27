using MelonLoader;
using UnityEngine;
using System.Collections;
using Il2CppRUMBLE.Managers;
using System;
using RumbleModdingAPI;
using Il2CppTMPro;

namespace HandDistanceRecorder
{
    public class main : MelonMod
    {
        private bool init = false;
        private GameObject lastHandPositionL, lastHandPositionR;
        private Transform handLTransform, handRTransform;
        private float distanceL = 0;
        private float distanceR = 0;
        private DateTime waitTill;
        private int sceneCount = 0;
        private TextMeshPro textL, textR;
        private string currentScene = "Loader";

        public override void OnLateInitializeMelon()
        {
            Calls.onMapInitialized += SceneLoaded;
        }

        private void SceneLoaded()
        {
            if (currentScene != "Loader")
            {
                MelonCoroutines.Start(WaitBeforeRunningInit(sceneCount));
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            sceneCount++;
            init = false;
            waitTill = DateTime.Now.AddSeconds(2);
        }

        public override void OnUpdate()
        {
            if (!init) { return; }
            if (lastHandPositionL == null || lastHandPositionR == null) { return; }
            if (DateTime.Now < waitTill)
            {
                GameObject VR = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(1).gameObject;
                handLTransform = VR.transform.GetChild(1);
                handRTransform = VR.transform.GetChild(2);
                lastHandPositionL.transform.position = handLTransform.position;
                lastHandPositionR.transform.position = handRTransform.position;
                return;
            }
            distanceL += Vector3.Distance(lastHandPositionL.transform.position, handLTransform.position);
            distanceR += Vector3.Distance(lastHandPositionR.transform.position, handRTransform.position);
            textL.text = distanceL.ToString("0.#");
            textR.text = distanceR.ToString("0.#");
            lastHandPositionL.transform.position = handLTransform.position;
            lastHandPositionR.transform.position = handRTransform.position;
        }

        public IEnumerator WaitBeforeRunningInit(int sceneChanges)
        {
            yield return new WaitForSeconds(1);
            if (sceneChanges == sceneCount)
            {
                MelonCoroutines.Start(SetInitialHandPositions(sceneChanges));
            }
            yield break;
        }

        public IEnumerator SetInitialHandPositions(int sceneChanges)
        {
            yield return new WaitForFixedUpdate();
            bool gotHands = false;
            while (!gotHands)
            {
                while (PlayerManager.instance.localPlayer.Controller == null)
                {
                    yield return new WaitForFixedUpdate();
                }
                if (sceneCount != sceneChanges)
                {
                    yield break;
                }
                GameObject VR = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(1).gameObject;
                lastHandPositionL = new GameObject();
                lastHandPositionR = new GameObject();
                lastHandPositionL.transform.parent = VR.transform;
                lastHandPositionR.transform.parent = VR.transform;
                handLTransform = VR.transform.GetChild(1);
                handRTransform = VR.transform.GetChild(2);
                GameObject textLeft = Calls.Create.NewText();
                textLeft.name = "TextLeft";
                textLeft.transform.parent = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(5).GetChild(7).GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
                textLeft.transform.localPosition = new Vector3(0.0272f, 0.115f, 0);
                textLeft.transform.localRotation = Quaternion.Euler(7.2175f, 269.9012f, 358.3884f);
                textL = textLeft.GetComponent<TextMeshPro>();
                textL.autoSizeTextContainer = true;
                textL.enableWordWrapping = false;
                textL.text = "0";
                textL.fontSize = 0.25f;
                textL.color = Color.white;
                textL.outlineColor = Color.black;
                textL.outlineWidth = 0.25f;
                GameObject textRight = Calls.Create.NewText();
                textRight.name = "TextRight";
                textRight.transform.parent = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(5).GetChild(7).GetChild(0).GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
                textRight.transform.localPosition = new Vector3(-0.0272f, 0.1148f, 0);
                textRight.transform.localRotation = Quaternion.Euler(7.1158f, 93.6177f, 0.9292f);
                textR = textRight.GetComponent<TextMeshPro>();
                textR.autoSizeTextContainer = true;
                textR.enableWordWrapping = false;
                textR.text = "0";
                textR.fontSize = 0.25f;
                textR.color = Color.white;
                textR.outlineColor = Color.black;
                textR.outlineWidth = 0.25f;
                lastHandPositionL.transform.position = handLTransform.localPosition;
                lastHandPositionR.transform.position = handRTransform.localPosition;
                if (handLTransform.position != null && handRTransform.position != null) { gotHands = true; }
                if (!gotHands)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            init = true;
            yield break;
        }
    }
}
