using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    public class ValuableObjectPatch
    {
        [HarmonyPatch(typeof(ValuableObject), "DollarValueSetLogic")]
        [HarmonyPrefix]
        public static bool DollarValueSetLogicPrefix(ValuableObject __instance)
        {
            if(DanosValuableGeneration.Instance == null)
            {
                return true;
            }


            DanosValuableGeneration.Instance.DollarValueSetLogic(__instance);
            return false;


        }

    }
}
