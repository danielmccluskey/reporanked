using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.AI;
using UnityEngine;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    public class ValuableObjectPatch
    {
        [HarmonyPatch(typeof(ValuableObject), "DollarValueSetLogic")]
        [HarmonyPrefix]
        public static bool DollarValueSetLogicPrefix(ValuableObject __instance)
        {
            if (DanosValuableGeneration.Instance == null)
            {
                return true;
            }


            DanosValuableGeneration.Instance.DollarValueSetLogic(__instance);
            return false;


        }


        [HarmonyPatch(typeof(ValuableObject), "Start")]
        [HarmonyPrefix]

        static bool QueueObjectPrefix(ValuableObject __instance)
        {


            __instance.physGrabObject = __instance.GetComponent<PhysGrabObject>();
            __instance.roomVolumeCheck = __instance.GetComponent<RoomVolumeCheck>();
            __instance.navMeshObstacle = __instance.GetComponent<NavMeshObstacle>();
            if ((bool)__instance.navMeshObstacle)
            {
                Debug.LogError(__instance.gameObject.name + " has a NavMeshObstacle component. Please remove it.");
            }
            __instance.StartCoroutine(DanosValuableGeneration.CustomDollarValueSet(__instance));
            __instance.rigidBodyMass = __instance.physAttributePreset.mass;
            __instance.rb = __instance.GetComponent<Rigidbody>();
            if ((bool)__instance.rb)
            {
                __instance.rb.mass = __instance.rigidBodyMass;
            }
            __instance.physGrabObject.massOriginal = __instance.rigidBodyMass;
            if (!LevelGenerator.Instance.Generated)
            {
                ValuableDirector.instance.valuableSpawnAmount++;
                ValuableDirector.instance.valuableList.Add(__instance);
            }
            if (__instance.volumeType <= ValuableVolume.Type.Small)
            {
                __instance.physGrabObject.clientNonKinematic = true;
            }


            return false;
        }

    }
}
