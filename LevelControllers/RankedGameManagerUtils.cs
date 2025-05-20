using HarmonyLib.Tools;
using RepoRankedApiResponseModel;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RepoRanked.LevelControllers
{
    public static class RankedGameManagerUtils
    {

        

        public static void CreateSave(MatchmakingStatusResponse chall)
        {
            string loggedinSteamID = SteamClient.SteamId.ToString();
            string name = "REPORanked";

            string savesPath = Path.Combine(Application.persistentDataPath, "saves");
            string rankedFolderPath = Path.Combine(savesPath, name);
            string rankedSavePath = Path.Combine(rankedFolderPath, $"{name}.es3");

            if (Directory.Exists(rankedFolderPath))
            {

                //Delete the es3 file if it exists
                if (File.Exists(rankedSavePath))
                {
                    File.Delete(rankedSavePath);
                }

                //Delete the folder if it exists
                Directory.Delete(rankedFolderPath, true);


            }


            Directory.CreateDirectory(rankedFolderPath);

            string fullPath = Path.Combine(rankedFolderPath, $"{name}.es3");
            string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var teamName = name;
            var timePlayed = 0f;
            var playerNames = new Dictionary<string, string>();
            var dictionaryOfDictionaries = new Dictionary<string, Dictionary<string, int>>();

            SteamIDApplier(loggedinSteamID, chall.saveData);
            PopulateDictionaries(dictionaryOfDictionaries, chall.saveData);

            var settings = new ES3Settings(fullPath)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = "Why would you want to cheat?... :o It's no fun. :') :'D"
            };

            ES3.Save("teamName", teamName, settings);
            ES3.Save("dateAndTime", date, settings);
            ES3.Save("timePlayed", timePlayed, settings);
            ES3.Save("playerNames", playerNames, settings);
            ES3.Save("dictionaryOfDictionaries", dictionaryOfDictionaries, settings);

        }
        private static void PopulateDictionaries(Dictionary<string, Dictionary<string, int>> target, DanosSaveData saveData)
        {

            target["runStats"] = new Dictionary<string, int>(saveData.runStats);
            target["itemsPurchased"] = new Dictionary<string, int>(saveData.itemsPurchased);
            target["itemsPurchasedTotal"] = new Dictionary<string, int>(saveData.itemsPurchasedTotal);
            target["itemsUpgradesPurchased"] = new Dictionary<string, int>(saveData.itemsUpgradesPurchased);
            target["itemBatteryUpgrades"] = new Dictionary<string, int>(saveData.itemBatteryUpgrades);
            target["itemUpgradesPurchased"] = new Dictionary<string, int>(saveData.itemUpgradesPurchased);
            target["itemStatBattery"] = new Dictionary<string, int>(saveData.itemStatBattery);
            target["playerHealth"] = new Dictionary<string, int>(saveData.playerHealth);
            target["playerUpgradeHealth"] = new Dictionary<string, int>(saveData.playerUpgradeHealth);
            target["playerUpgradeStamina"] = new Dictionary<string, int>(saveData.playerUpgradeStamina);
            target["playerUpgradeExtraJump"] = new Dictionary<string, int>(saveData.playerUpgradeExtraJump);
            target["playerUpgradeLaunch"] = new Dictionary<string, int>(saveData.playerUpgradeLaunch);
            target["playerUpgradeMapPlayerCount"] = new Dictionary<string, int>(saveData.playerUpgradeMapPlayerCount);
            target["playerUpgradeSpeed"] = new Dictionary<string, int>(saveData.playerUpgradeSpeed);
            target["playerUpgradeStrength"] = new Dictionary<string, int>(saveData.playerUpgradeStrength);
            target["playerUpgradeRange"] = new Dictionary<string, int>(saveData.playerUpgradeRange);
            target["playerUpgradeThrow"] = new Dictionary<string, int>(saveData.playerUpgradeThrow);
            target["playerHasCrown"] = new Dictionary<string, int>(saveData.playerHasCrown);

        }
        public static void SteamIDApplier(string steamID, DanosSaveData saveData)
        {
            ApplySteamIDToKeys(saveData.runStats, steamID);
            ApplySteamIDToKeys(saveData.itemsPurchased, steamID);
            ApplySteamIDToKeys(saveData.itemsPurchasedTotal, steamID);
            ApplySteamIDToKeys(saveData.itemsUpgradesPurchased, steamID);
            ApplySteamIDToKeys(saveData.itemBatteryUpgrades, steamID);
            ApplySteamIDToKeys(saveData.itemUpgradesPurchased, steamID);
            ApplySteamIDToKeys(saveData.itemStatBattery, steamID);
            ApplySteamIDToKeys(saveData.playerHealth, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeHealth, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeStamina, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeExtraJump, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeLaunch, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeMapPlayerCount, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeSpeed, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeStrength, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeRange, steamID);
            ApplySteamIDToKeys(saveData.playerUpgradeThrow, steamID);
            ApplySteamIDToKeys(saveData.playerHasCrown, steamID);
        }

        private static void ApplySteamIDToKeys<T>(Dictionary<string, T> dictionary, string steamID)
        {
            if (dictionary == null || string.IsNullOrEmpty(steamID))
            {
                return;
            }

            //if there are no keys
            if (dictionary.Count == 0)
            {
                return;
            }

            foreach (var key in dictionary.Keys.Where(k => k.Contains("STEAMID")).ToList())
            {


                var newKey = key.Replace("STEAMID", steamID);
                dictionary[newKey] = dictionary[key];
                dictionary.Remove(key);
            }
        }

    }
}
