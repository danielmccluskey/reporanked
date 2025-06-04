using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.MainMenu;
using RepoRankedApiResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.Patches.LevelGeneration
{

    //[HarmonyPatch(typeof(RunManager), "Awake")]
    //public class RunManagerPatch_SetRunLevel2
    //{
    //    static void Postfix(RunManager __instance)
    //    {
    //        foreach (var lvl in RunManager.instance.levels)
    //        {
    //            Debug.Log($"Level: {lvl.ResourcePath}");
    //        }
    //    }
    //}

    [HarmonyPatch(typeof(RunManager), "SetRunLevel")]
    public class RunManagerPatch_SetRunLevel
    {
        static void Postfix(RunManager __instance)
        {
            
            Debug.Log($"RunManagerPatch_SetRunLevel: DanosMatchQueuePoller.QueueType = {DanosMatchQueuePoller.QueueType}");

            switch (DanosMatchQueuePoller.QueueType)
            {
                case QueueTypes.ranked:
                    RankedLogic(__instance);
                    break;
                case QueueTypes.unranked:
                    RankedLogic(__instance);
                    break;
                case QueueTypes.monthlychallenge:
                    MonthlyLogic(__instance);
                    break;
                default:
                    RepoRanked.Logger.LogWarning("Unsupported queue type for SetRunLevel.");
                    break;
            }





        }

        private static void MonthlyLogic(RunManager instance)
        {
            List<Level> levels = instance.levels;
            if (levels == null || levels.Count == 0)
            {
                RepoRanked.Logger.LogWarning("RunManager.levels is empty or null.");
                return;
            }
            if (MonthlyGameManager.Instance == null)
            {
                RepoRanked.Logger.LogWarning("MonthlyGameManager.Instance is null.");
                return;
            }
            if (MonthlyGameManager.Instance.FirstToTenData == null)
            {
                RepoRanked.Logger.LogWarning("MonthlyGameManager.Instance is null.");
                return;
            }



            FTTDMapData currentMapData = MonthlyGameManager.Instance.GetCurrentMapData();
            if (currentMapData == null)
            {
                RepoRanked.Logger.LogWarning("Current map data is null.");
                return;
            }

            string mapName = currentMapData.map;
            if (string.IsNullOrEmpty(mapName))
            {
                RepoRanked.Logger.LogWarning("Map name is null or empty.");
                return;
            }

            //Set the level
            foreach (var level in levels)
            {
                if (level != null && level.ResourcePath.Contains(mapName))
                {
                    instance.levelCurrent = level;
                    RepoRanked.Logger.LogInfo($"Set currentLevel to: {level.ResourcePath}");
                    break;
                }
            }

        }

        private static void RankedLogic(RunManager __instance)
        {
            List<Level> levels = __instance.levels;
            if (levels == null || levels.Count == 0)
            {
                RepoRanked.Logger.LogWarning("RunManager.levels is empty or null.");
                return;
            }
            if (RankedGameManager.Instance == null)
            {
                RepoRanked.Logger.LogWarning("RankedGameManager.Instance is null.");
                return;
            }

            if (RankedGameManager.Instance.matchData == null)
            {
                RepoRanked.Logger.LogWarning("RankedGameManager.Instance.matchData is null.");
                return;
            }

            string mapName = RankedGameManager.Instance.matchData.Map;
            if (mapName == "unknown")
            {
                mapName = "Wizard";
            }

            //build a string from the matchdata.players e.g. Player.Value1 vs Player.Value2
            string VSText = "";
            foreach (var player in RankedGameManager.Instance.matchData.Players)
            {
                if (player.Value != null)
                {
                    VSText += player.Value + " vs ";
                }
            }

            //remove the last 4 characters from VSText
            if (VSText.Length > 4)
            {
                VSText = VSText.Substring(0, VSText.Length - 4);
            }


            //Set runmanager.currentlevel to the one with "Manor" in the resource path
            foreach (var level in levels)
            {
                if (level != null && level.ResourcePath.Contains(mapName))
                {
                    __instance.levelCurrent = level;
                    __instance.levelCurrent.NarrativeName = VSText;
                    RepoRanked.Logger.LogInfo($"Set currentLevel to: {level.ResourcePath}");
                    break;
                }
            }

        }
    }
}
