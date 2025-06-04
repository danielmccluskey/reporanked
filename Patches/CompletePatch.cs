using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.MainMenu;
using RepoRankedApiResponseModel;
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
            switch(DanosMatchQueuePoller.QueueType)
            {
                case QueueTypes.ranked:
                    RankedGameManager inst = RankedGameManager.Instance;
                    if (inst == null)
                    {
                        return true;
                    }
                    inst.CompleteMatch("finished");
                    break;

                case QueueTypes.unranked:
                    RankedGameManager unrankedInst = RankedGameManager.Instance;
                    if (unrankedInst == null)
                    {
                        return true;
                    }
                    unrankedInst.CompleteMatch("finished");
                    break;

                case QueueTypes.monthlychallenge:
                    MonthlyGameManager monthlyInst = MonthlyGameManager.Instance;
                    if (monthlyInst == null)
                    {
                        return true;
                    }
                    monthlyInst.CompletedRound();
                    return true;
                    break;
            }           


            return false;


        }

    }
}
