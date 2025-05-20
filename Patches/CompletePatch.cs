using HarmonyLib;
using RepoRanked.LevelControllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoRanked.Patches
{
   

    [HarmonyPatch]
    public class CompletePatch
    {
        [HarmonyPatch(typeof(TruckScreenText), "GotoNextLevel")]
        [HarmonyPrefix]
        public static bool GoToNextLevelPrefix()
        {            
            RankedGameManager inst = RankedGameManager.Instance;
            if (inst == null)
            {
                return true;
            }

            inst.CompleteMatch("finished");

            return false;


        }

    }
}
