using HarmonyLib;
using RepoRanked.EnemyGeneration;
using RepoRanked.LevelControllers;
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
            if (StatsManager.instance.teamName == "REPORanked")
            {
                if(RankedGameManager.Instance == null)
                {
                    RepoRanked.Logger.LogWarning("RankedGameManager.Instance is null.");
                    return true;
                }

                if (RankedGameManager.Instance.matchData == null)
                {
                    RepoRanked.Logger.LogWarning("RankedGameManager.Instance.matchData is null.");
                    return true;
                }
                __instance.enemyList = new List<EnemySetup>();
                EnemySetup customGen = DanosEnemyGenerator.CreateCustomEnemySetup(RankedGameManager.Instance.matchData.Enemies);
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
