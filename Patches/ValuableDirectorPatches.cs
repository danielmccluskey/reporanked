using HarmonyLib;
using Photon.Pun;
using RepoRanked.LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    class ValuableDirectorPatches
    {




        [HarmonyPatch(typeof(ValuableDirector), nameof(ValuableDirector.Spawn))]
        [HarmonyPrefix]
        private static bool SpawnPrefix(ValuableDirector __instance, GameObject _valuable, ValuableVolume _volume, string _path)
        {
            if (GameManager.instance.gameMode == 0)
            {
                UnityEngine.Object.Instantiate(_valuable, _volume.transform.position, _volume.transform.rotation);
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject(__instance.resourcePath + _path + "/" + _valuable.name, _volume.transform.position, _volume.transform.rotation, 0);
            }
            ValuableObject component = _valuable.GetComponent<ValuableObject>();
            component.dollarValueSet = false;

            DanosValuableGeneration.Instance.DollarValueSetLogic(component);
            __instance.valuableTargetAmount++;
            __instance.totalCurrentValue += component.dollarValueCurrent * 0.001f;
            if (__instance.totalCurrentValue > __instance.totalMaxValue)
            {
                __instance.totalMaxAmount = __instance.valuableTargetAmount;
            }

            return false; // Skip original method
        }

    }
}
