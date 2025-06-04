using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.MainMenu;
using RepoRankedApiResponseModel;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    public class DeathPatches
    {
        [HarmonyPatch(typeof(PlayerAvatar), "PlayerDeath")]
        [HarmonyPostfix]
        public static void PlayerDeathRPCPostfix(PlayerAvatar __instance, int enemyIndex)
        {
            switch (DanosMatchQueuePoller.QueueType)
            {
                case QueueTypes.ranked:
                    RankedGameManager inst = RankedGameManager.Instance;
                    if (inst == null)
                    {
                        return;
                    }
                    inst.CompleteMatch("dead");
                    break;

                case QueueTypes.unranked:
                    RankedGameManager unrankedInst = RankedGameManager.Instance;
                    if (unrankedInst == null)
                    {
                        return;
                    }
                    unrankedInst.CompleteMatch("dead");
                    break;

                case QueueTypes.monthlychallenge:

                    if(SemiFunc.RunIsShop())
                    {
                        return;
                    }


                    MonthlyGameManager monthlyInst = MonthlyGameManager.Instance;
                    if (monthlyInst == null)
                    {
                        return;
                    }
                    monthlyInst.CompleteMatch("dead");
                    break;
            }




        }

    }
}
