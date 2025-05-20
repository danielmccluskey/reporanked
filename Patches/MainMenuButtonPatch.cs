using HarmonyLib;
using RepoRanked.LevelControllers;
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
            RankedGameManager inst = RankedGameManager.Instance;
            if (inst == null)
            {
                return;
            }
            inst.CompleteMatch("forfeit");
        }
    }
}
