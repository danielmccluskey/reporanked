using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    class CurrencyUIPatch
    {
        [HarmonyPatch(typeof(CurrencyUI), "Update")]
        [HarmonyPostfix]
        public static void Postfix()
        {
            CurrencyUI.instance.Text.text = ""; // Remove the current money text
        }
    }
}
