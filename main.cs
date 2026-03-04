using HarmonyLib;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players;
using Il2CppTMPro;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using System;
using System.Collections;
using UnityEngine;

namespace HandDistanceRecorder
{
    public class main : MelonMod
    {
        private static bool init = false;
        private static GameObject lastHandPositionL, lastHandPositionR;
        private static Transform handLTransform, handRTransform;
        private float distanceL = 0;
        private float distanceR = 0;
        private static DateTime waitTill;
        private static TextMeshPro textL, textR;

        [HarmonyPatch(typeof(PlayerController), "Initialize", new Type[] { typeof(Player) })]
        public static class PlayerSpawn
        {
            private static void Postfix(ref PlayerController __instance, ref Player player)
            {
                if (__instance.controllerType == ControllerType.Local)
                {
                    init = false;
                    waitTill = DateTime.Now.AddSeconds(2);
                    MelonCoroutines.Start(SetInitialHandPositions());
                }
            }
        }

        public static IEnumerator SetInitialHandPositions()
        {
            yield return new WaitForFixedUpdate();

            lastHandPositionL = new GameObject();
            lastHandPositionL.name = "HandDistanceTrackerL";
            lastHandPositionL.transform.parent = PlayerManager.instance.localPlayer.Controller.PlayerVR.transform;
            handLTransform = PlayerManager.instance.localPlayer.Controller.PlayerVR.leftController.Transform;

            lastHandPositionR = new GameObject();
            lastHandPositionR.name = "HandDistanceTrackerR";
            lastHandPositionR.transform.parent = PlayerManager.instance.localPlayer.Controller.PlayerVR.transform;
            handRTransform = PlayerManager.instance.localPlayer.Controller.PlayerVR.rightController.Transform;

            GameObject textLeft = Create.NewText();
            textLeft.name = "TextLeft";
            textLeft.transform.parent = PlayerManager.instance.localPlayer.Controller.PlayerScaling.RigDefinition.leftHandDefinition.Transform;
            textLeft.transform.localPosition = new Vector3(0.0372f, 0.115f, 0);
            textLeft.transform.localRotation = Quaternion.Euler(7.2175f, 269.9012f, 358.3884f);
            textL = textLeft.GetComponent<TextMeshPro>();
            textL.autoSizeTextContainer = true;
            textL.enableWordWrapping = false;
            textL.alignment = TextAlignmentOptions.Center;
            textL.text = "0";
            textL.fontSize = 0.25f;
            textL.color = Color.white;
            textL.outlineColor = Color.black;
            textL.outlineWidth = 0.25f;

            GameObject textRight = Create.NewText();
            textRight.name = "TextRight";
            textRight.transform.parent = PlayerManager.instance.localPlayer.Controller.PlayerScaling.RigDefinition.rightHandDefinition.Transform;
            textRight.transform.localPosition = new Vector3(-0.0372f, 0.1148f, 0);
            textRight.transform.localRotation = Quaternion.Euler(7.1158f, 93.6177f, 0.9292f);
            textR = textRight.GetComponent<TextMeshPro>();
            textR.autoSizeTextContainer = true;
            textR.enableWordWrapping = false;
            textR.alignment = TextAlignmentOptions.Center;
            textR.text = "0";
            textR.fontSize = 0.25f;
            textR.color = Color.white;
            textR.outlineColor = Color.black;
            textR.outlineWidth = 0.25f;

            lastHandPositionL.transform.position = handLTransform.localPosition;
            lastHandPositionR.transform.position = handRTransform.localPosition;
            init = true;
            yield break;
        }

        public override void OnUpdate()
        {
            if (!init) { return; }
            if (lastHandPositionL == null || lastHandPositionR == null) { return; }
            if (DateTime.Now < waitTill)
            {
                handLTransform = PlayerManager.instance.localPlayer.Controller.PlayerVR.leftController.Transform;
                handRTransform = PlayerManager.instance.localPlayer.Controller.PlayerVR.rightController.Transform;
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
    }
}
