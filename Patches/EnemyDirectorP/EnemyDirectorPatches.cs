using HarmonyLib;
using RepoRanked.EnemyGeneration;
using RepoRanked.LevelControllers;
using RepoRanked.MainMenu;
using RepoRankedApiResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using static Steamworks.InventoryItem;

namespace RepoRanked.Patches.EnemyDirectorP
{
    [HarmonyPatch(typeof(EnemyDirector), nameof(EnemyDirector.AmountSetup))]
    public class Patch_EnemyDirector_AmountSetup
    {
        static bool Prefix(EnemyDirector __instance)
        {
            //DifficultyAmountLogger.GenerateDifficultyAmountTable(__instance.amountCurve1, __instance.amountCurve2, __instance.amountCurve3);
            //EnemyExtractor.GenerateEnemiesFullJson(__instance);
            //EnemyExtractor.GenerateEnemiesJson(__instance);
            if (StatsManager.instance.teamName == "REPORanked")
            {
                EnemySetup customGen;

                switch(DanosMatchQueuePoller.QueueType)
                {
                    case QueueTypes.ranked:
                        if (RankedGameManager.Instance == null)
                        {
                            RepoRanked.Logger.LogWarning("RankedGameManager.Instance is null.");
                            return true;
                        }

                        if (RankedGameManager.Instance.matchData == null)
                        {
                            RepoRanked.Logger.LogWarning("RankedGameManager.Instance.matchData is null.");
                            return true;
                        }
                        customGen = DanosEnemyGenerator.CreateCustomEnemySetup(RankedGameManager.Instance.matchData.Enemies);
                        break;
                    case QueueTypes.unranked:
                        if (RankedGameManager.Instance == null)
                        {
                            RepoRanked.Logger.LogWarning("RankedGameManager.Instance is null.");
                            return true;
                        }

                        if (RankedGameManager.Instance.matchData == null)
                        {
                            RepoRanked.Logger.LogWarning("RankedGameManager.Instance.matchData is null.");
                            return true;
                        }
                        customGen = DanosEnemyGenerator.CreateCustomEnemySetup(RankedGameManager.Instance.matchData.Enemies);
                        break;
                    case QueueTypes.monthlychallenge:
                        customGen = DanosEnemyGenerator.CreateCustomEnemySetup(MonthlyGameManager.Instance.GetCurrentMapData().Enemies);
                        break;
                    default:
                        RepoRanked.Logger.LogWarning("Unsupported queue type for EnemyDirector.AmountSetup.");
                        return true; // Proceed with original method
                }



                
                __instance.enemyList = new List<EnemySetup>();
                if (customGen != null)
                {
                    List<EnemySetup> customEnemyLlist = new List<EnemySetup>();

                    //Add the custom enemy setup to the list

                    customEnemyLlist.Add(customGen);
                    EnemyDirector.instance.PickEnemies(customEnemyLlist);
                    EnemyDirector.instance.totalAmount = 1;




                    return false; // Skip original method
                }

            }
            return true; // Proceed with original

        }
    }
}
