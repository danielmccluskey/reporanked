using HarmonyLib;
using Photon.Pun;
using RepoRanked.LevelGeneration;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static SemiFunc;
using static UnityEngine.Rendering.DebugUI;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    public class ItemAttributePatches
    {
        [HarmonyPatch(typeof(ItemAttributes), "GetValue")]
        [HarmonyPrefix]
        public static bool GetValuePrefix(ItemAttributes __instance)
        {
            try
            {



                if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
                {
                    float num = DanosItemAttributeManager.Range(__instance.itemValueMin, __instance.itemValueMax)
                                 * ShopManager.instance.itemValueMultiplier;
                    if (num < 1000f)
                    {
                        num = 1000f;
                    }
                    num = Mathf.Ceil(num / 1000f);
                    if (__instance.itemType == SemiFunc.itemType.item_upgrade)
                    {
                        num = ShopManager.instance.UpgradeValueGet(num, __instance.item);
                    }
                    else if (__instance.itemType == SemiFunc.itemType.healthPack)
                    {
                        num = ShopManager.instance.HealthPackValueGet(num);
                    }
                    else if (__instance.itemType == SemiFunc.itemType.power_crystal)
                    {
                        num = ShopManager.instance.CrystalValueGet(num);
                    }
                    __instance.value = (int)num;
                    if (GameManager.Multiplayer())
                    {
                        __instance.photonView.RPC("GetValueRPC", RpcTarget.Others, __instance.value);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in ItemAttributes.GetValuePrefix: {e.Message}\n{e.StackTrace}");
                return true;
            }
            return false;
        }
    }
}
