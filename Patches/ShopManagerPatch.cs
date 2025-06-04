using HarmonyLib;
using RepoRanked.LevelControllers;
using RepoRanked.LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static SemiFunc;

namespace RepoRanked.Patches
{
    public class ShopItemManagerPatch
    {
        [HarmonyPatch(typeof(ShopManager), "GetAllItemsFromStatsManager")]
        [HarmonyPrefix]
        static bool Prefix(ShopManager __instance)
        {
            System.Random rng = new System.Random(DanosLevelGenerator.Seed);


            if (SemiFunc.IsNotMasterClient())
            {
                return false; // skip original method
            }

            __instance.potentialItems.Clear();
            __instance.potentialItemConsumables.Clear();
            __instance.potentialItemUpgrades.Clear();
            __instance.potentialItemHealthPacks.Clear();
            __instance.potentialSecretItems.Clear();

            foreach (var value in StatsManager.instance.itemDictionary.Values)
            {
                int num = SemiFunc.StatGetItemsPurchased(value.itemAssetName);
                float num2 = value.value.valueMax / 1000f * __instance.itemValueMultiplier;

                if (value.itemType == SemiFunc.itemType.item_upgrade)
                {
                    num2 -= num2 * 0.05f * (GameDirector.instance.PlayerList.Count - 1);
                    int upgradesPurchased = StatsManager.instance.GetItemsUpgradesPurchased(value.itemAssetName);
                    num2 += num2 * __instance.upgradeValueIncrease * upgradesPurchased;
                    num2 = Mathf.Ceil(num2);
                }

                if (value.itemType == SemiFunc.itemType.healthPack)
                {
                    num2 += num2 * __instance.healthPackValueIncrease * RunManager.instance.levelsCompleted;
                    num2 = Mathf.Ceil(num2);
                }

                if (value.itemType == SemiFunc.itemType.power_crystal)
                {
                    num2 += num2 * __instance.crystalValueIncrease * RunManager.instance.levelsCompleted;
                    num2 = Mathf.Ceil(num2);
                }

                float num3 = Mathf.Clamp(num2, 1f, num2);
                bool isCrystal = value.itemType == SemiFunc.itemType.power_crystal;
                bool isUpgrade = value.itemType == SemiFunc.itemType.item_upgrade;
                bool isHealth = value.itemType == SemiFunc.itemType.healthPack;
                int maxInShop = value.maxAmountInShop;

                if (num >= maxInShop ||
                    (value.maxPurchase && StatsManager.instance.GetItemsUpgradesPurchasedTotal(value.itemAssetName) >= value.maxPurchaseAmount) ||
                    (!(num3 <= __instance.totalCurrency) && rng.Next(0, 100) >= 25))
                {
                    continue;
                }

                for (int i = 0; i < maxInShop - num; i++)
                {
                    if (isUpgrade)
                    {
                        __instance.potentialItemUpgrades.Add(value);
                        continue;
                    }
                    if (isHealth)
                    {
                        __instance.potentialItemHealthPacks.Add(value);
                        continue;
                    }
                    if (isCrystal)
                    {
                        __instance.potentialItemConsumables.Add(value);
                        continue;
                    }
                    if (value.itemSecretShopType == SemiFunc.itemSecretShopType.none)
                    {
                        __instance.potentialItems.Add(value);
                        continue;
                    }
                    if (!__instance.potentialSecretItems.ContainsKey(value.itemSecretShopType))
                    {
                        __instance.potentialSecretItems.Add(value.itemSecretShopType, new List<Item>());
                    }
                    __instance.potentialSecretItems[value.itemSecretShopType].Add(value);
                }
            }

            __instance.potentialItems.ShuffleWithRng(rng);
            __instance.potentialItemConsumables.ShuffleWithRng(rng);
            __instance.potentialItemUpgrades.ShuffleWithRng(rng);
            __instance.potentialItemHealthPacks.ShuffleWithRng(rng);
            foreach (var list in __instance.potentialSecretItems.Values)
            {
                list.ShuffleWithRng(rng);
            }

            return false; // skip original method
        }


        [HarmonyPatch(typeof(ShopManager), "GetAllItemVolumesInScene")]
        [HarmonyPrefix]
        static bool GetAllItemVolumesInScenePrefix(ShopManager __instance)
        {
            System.Random rng = new System.Random(DanosLevelGenerator.Seed);

            if (SemiFunc.IsNotMasterClient())
            {
                return false;
            }
            __instance.itemVolumes.Clear();
            ItemVolume[] array = UnityEngine.Object.FindObjectsOfType<ItemVolume>();
            foreach (ItemVolume itemVolume in array)
            {
                if (itemVolume.itemSecretShopType == SemiFunc.itemSecretShopType.none)
                {
                    __instance.itemVolumes.Add(itemVolume);
                    continue;
                }
                if (!__instance.secretItemVolumes.ContainsKey(itemVolume.itemSecretShopType))
                {
                    __instance.secretItemVolumes.Add(itemVolume.itemSecretShopType, new List<ItemVolume>());
                }
                __instance.secretItemVolumes[itemVolume.itemSecretShopType].Add(itemVolume);
            }
            foreach (List<ItemVolume> value in __instance.secretItemVolumes.Values)
            {
                value.ShuffleWithRng(rng);
            }
            __instance.itemVolumes.ShuffleWithRng(rng);

            return false; // skip original method
        }
    }

}
