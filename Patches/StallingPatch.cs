using HarmonyLib;
using Photon.Pun;
using RepoRanked.LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    class StallingPatch
    {
        [HarmonyPatch(typeof(TruckScreenText), "NextLine")]
        [HarmonyPrefix]
        static bool Prefix(TruckScreenText __instance, int _currentLineIndex)
        {
            Random random = new Random(DanosLevelGenerator.Seed);
            var pages = __instance.pages;
            int currentPageIndex = __instance.currentPageIndex;
            int currentLineIndex = __instance.currentLineIndex;

            if (pages[currentPageIndex].textLines.Count != 0)
            {
                int lineCount = pages[currentPageIndex].textLines[currentLineIndex].textLines.Count();

                if (GameManager.instance.gameMode == 0)
                {
                    int index = random.Next(0, lineCount);
                    __instance.NextLineLogic(_currentLineIndex, index);
                }
                else if (PhotonNetwork.IsMasterClient)
                {
                    int index = random.Next(0, lineCount);
                    __instance.photonView.RPC("NextLineRPC", RpcTarget.All, index, _currentLineIndex);
                }
            }

            return false;
        }

    }
}
