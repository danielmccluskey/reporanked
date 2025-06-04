using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.MainMenu;
using RepoRankedApiResponseModel;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoRanked.Patches
{
    [HarmonyPatch]

    class MainMenuButtonPatch
    {
        [HarmonyPatch(typeof(MenuPageEsc), "ButtonEventQuitToMenu")]
        [HarmonyPrefix]
        public static bool Prefix()
        {
            ForfitMatch();
            return true;


        }

        [HarmonyPatch(typeof(MenuPageEsc), "ButtonEventQuit")]
        [HarmonyPrefix]
        public static bool OtherPrefix()
        {
            ForfitMatch();
            return true;


        }



        private static void ForfitMatch()
        {


            switch (DanosMatchQueuePoller.QueueType)
            {
                case QueueTypes.ranked:
                    RankedGameManager inst = RankedGameManager.Instance;
                    if (inst == null)
                    {
                        return;
                    }
                    inst.CompleteMatch("forfeit");
                    break;

                case QueueTypes.unranked:
                    RankedGameManager unrankedInst = RankedGameManager.Instance;
                    if (unrankedInst == null)
                    {
                        return;
                    }
                    unrankedInst.CompleteMatch("forfeit");
                    break;

                case QueueTypes.monthlychallenge:
                    MonthlyGameManager monthlyInst = MonthlyGameManager.Instance;
                    if (monthlyInst == null)
                    {
                        return;
                    }
                    monthlyInst.CompleteMatch("forfeit");
                    break;
            }
        }
    }
}
