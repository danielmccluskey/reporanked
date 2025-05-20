using HarmonyLib;
using RepoRanked.LevelControllers;
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
            Debug.Log($"[REPORanked] PlayerDeathRPCPostfix called for {__instance.name} with enemyIndex {enemyIndex}");
            RankedGameManager inst = RankedGameManager.Instance;
            if (inst == null)
            {
                return;
            }

            inst.CompleteMatch("dead");




        }

    }
}
