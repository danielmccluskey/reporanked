using ExitGames.Client.Photon;
using HarmonyLib;
using RepoRanked.LevelControllers;
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

                RankedGameManager gameManager = RankedGameManager.Instance;
                if (gameManager == null)
                {
                    RepoRanked.Logger.LogError("RankedGameManager instance is null.");
                    return true; // run original
                }

                // Get the seed from the RankedGameManager
                string seed = gameManager.matchData.Seed;
                if (string.IsNullOrEmpty(seed))
                {
                    RepoRanked.Logger.LogError("Seed is null or empty.");
                    return true; // run original
                }

                //try parse as int
                if (!int.TryParse(seed, out int seedInt))
                {
                    RepoRanked.Logger.LogError($"Failed to parse seed '{seed}' as int.");
                    return true; // run original
                }


                DanosLevelGenerator.Create(seedInt);
                DanosValuableGeneration.Create(seedInt);
                UnityEngine.Random.InitState(seedInt);//catch all for anything else I missed
                __instance.StartCoroutine(DanosLevelGenerator.GenerateWithSeed(__instance, seedInt));
                return false; // skip original
            }

            return true; // run original
        }
    }
}
