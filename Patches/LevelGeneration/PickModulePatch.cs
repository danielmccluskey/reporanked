using HarmonyLib;
using RepoRanked.LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.Patches.LevelGeneration
{
    [HarmonyPatch(typeof(LevelGenerator), "PickModule")]
    class PickModulePatch
    {
        static bool Prefix(
            ref GameObject __result,
            List<GameObject> _list1,
            List<GameObject> _list2,
            List<GameObject> _list3,
            ref int _index1,
            ref int _index2,
            ref int _index3,
            ref int _loops1,
            ref int _loops2,
            ref int _loops3
        )
        {
            DanosLevelGenerator danosLevelGenerator = DanosLevelGenerator.Instance;

            if(danosLevelGenerator == null)
            {
                RepoRanked.Logger.LogError("DanosLevelGenerator instance is null.");
                return true; // run original
            }

            LevelGenerator __instance = LevelGenerator.Instance;
            if (__instance == null)
            {
                RepoRanked.Logger.LogError("LevelGenerator instance is null.");
                return true; // run original
            }

            __result = DanosLevelGenerator.PickModule(
                _list1,
                _list2,
                _list3,
                ref _index1,
                ref _index2,
                ref _index3,
                ref _loops1,
                ref _loops2,
                ref _loops3,
                __instance,
                danosLevelGenerator.rng
            );

            if (__result == null)
            {
                RepoRanked.Logger.LogError("Picked module is null. Check your module lists and RNG settings.");
                return true; // run original
            }



            // Skip original function
            return false;
        }
    }
}
