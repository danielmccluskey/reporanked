using HarmonyLib;
using RepoRanked.LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoRanked.Patches.LevelGeneration
{
    [HarmonyPatch(typeof(LevelGenerator), "Start")]
    public class Patch_LevelManager_Generate
    {
        static bool Prefix(LevelGenerator __instance)
        {
            if (StatsManager.instance.teamName == "REPORanked")
            {
                RepoRanked.Logger.LogInfo("Using custom Generate() coroutine for REPORanked.");
                __instance.StartCoroutine(DanosLevelGenerator.GenerateWithSeed(__instance, 0));
                return false; // skip original
            }

            return true; // run original
        }
    }
}
