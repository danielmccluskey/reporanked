using ExitGames.Client.Photon;
using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.LevelGeneration;
using RepoRanked.MainMenu;
using RepoRankedApiResponseModel;
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
            if (StatsManager.instance.teamName != "REPORanked")
                return true;

            RepoRanked.Logger.LogInfo("Using custom Generate() coroutine for REPORanked.");

            string seed = null;

            switch (DanosMatchQueuePoller.QueueType)
            {
                case QueueTypes.ranked:
                    if (RankedGameManager.Instance == null)
                    {
                        RepoRanked.Logger.LogError("RankedGameManager instance is null.");
                        return true;
                    }
                    seed = RankedGameManager.Instance.matchData.Seed;
                    break;

                case QueueTypes.unranked:
                    if (RankedGameManager.Instance == null)
                    {
                        RepoRanked.Logger.LogError("RankedGameManager instance is null.");
                        return true;
                    }
                    seed = RankedGameManager.Instance.matchData.Seed;
                    break;

                case QueueTypes.monthlychallenge:
                    if (MonthlyGameManager.Instance == null)
                    {
                        RepoRanked.Logger.LogError("MonthlyGameManager instance is null.");
                        return true;
                    }

                    FTTDMapData currentMapData = MonthlyGameManager.Instance.GetCurrentMapData();

                    seed = currentMapData.seed.ToString();



                    break;

                // Add other cases as needed
                default:
                    RepoRanked.Logger.LogError("Unsupported queue type.");
                    return true;
            }

            if (string.IsNullOrEmpty(seed))
            {
                RepoRanked.Logger.LogError("Seed is null or empty.");
                return true;
            }

            if (!int.TryParse(seed, out int seedInt))
            {
                RepoRanked.Logger.LogError($"Failed to parse seed '{seed}' as int.");
                return true;
            }

            DanosLevelGenerator.Create(seedInt);
            DanosValuableGeneration.Create(seedInt);
            UnityEngine.Random.InitState(seedInt); // catch-all for anything else
            __instance.StartCoroutine(DanosLevelGenerator.GenerateWithSeed(__instance, seedInt));
            return false;
        }

    }
}
