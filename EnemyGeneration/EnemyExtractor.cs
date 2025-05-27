using BepInEx;
using Newtonsoft.Json;
using RepoRankedApiResponseModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RepoRanked.EnemyGeneration
{

    public class EnemyExtractor : MonoBehaviour
    {
        public static void GenerateEnemiesFullJson(EnemyDirector director)
        {
            var allDifficulties = new Dictionary<int, List<EnemySetup>>
    {
        { 1, director.enemiesDifficulty1 },
        { 2, director.enemiesDifficulty2 },
        { 3, director.enemiesDifficulty3 },
    };

            var result = new Dictionary<int, List<SerializableEnemySetup>>();

            foreach (var kvp in allDifficulties)
            {
                int difficulty = kvp.Key;
                var setups = kvp.Value;
                var serializedSetups = new List<SerializableEnemySetup>();

                foreach (var setup in setups)
                {
                    var enemyNames = new List<string>();
                    foreach (var obj in setup.spawnObjects)
                    {
                        if (obj == null) continue;
                        string name = GetReadableEnemyName(obj);
                        if (!string.IsNullOrWhiteSpace(name))
                            enemyNames.Add(name);
                    }

                    var serializableSetup = new SerializableEnemySetup
                    {
                        Enemies = enemyNames,
                        LevelsCompletedCondition = setup.levelsCompletedCondition,
                        LevelsCompletedMin = setup.levelsCompletedMin,
                        LevelsCompletedMax = setup.levelsCompletedMax,
                        RarityPreset = new SerializableRarityPreset
                        {
                            Name = setup.rarityPreset?.name ?? "None",
                            Chance = setup.rarityPreset != null ? setup.rarityPreset.chance : 0f
                        },
                        RunsPlayed = setup.runsPlayed
                    };

                    serializedSetups.Add(serializableSetup);
                }

                result[difficulty] = serializedSetups;
            }

            var outputPath = Path.Combine(Paths.BepInExRootPath, "enemiesFull.json");
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(outputPath, json);
            RepoRanked.Logger.LogInfo($"Full enemy setups written to {outputPath}");
        }




        public static void GenerateEnemiesJson(EnemyDirector director)
        {
            var allDifficulties = new Dictionary<int, List<EnemySetup>>
        {
            { 1, director.enemiesDifficulty1 },
            { 2, director.enemiesDifficulty2 },
            { 3, director.enemiesDifficulty3 },
        };

            var result = new Dictionary<int, List<Dictionary<string, int>>>();

            foreach (var kvp in allDifficulties)
            {
                var difficultyLevel = kvp.Key;
                var setups = kvp.Value;
                var combinations = new List<Dictionary<string, int>>();

                foreach (var setup in setups)
                {
                    var spawnDict = new Dictionary<string, int>();

                    foreach (var obj in setup.spawnObjects)
                    {
                        if (obj == null) continue;

                        string enemyName = GetReadableEnemyName(obj);
                        if (string.IsNullOrWhiteSpace(enemyName)) continue;

                        if (!spawnDict.ContainsKey(enemyName))
                            spawnDict[enemyName] = 1;
                        else
                            spawnDict[enemyName]++;
                    }

                    if (spawnDict.Count > 0)
                        combinations.Add(spawnDict);
                }

                result[difficultyLevel] = combinations;
            }

            var outputPath = Path.Combine(Paths.BepInExRootPath, "enemies.json");
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(outputPath, json);

            RepoRanked.Logger.LogInfo($"Enemy combinations written to {outputPath}");
        }

        private static string GetReadableEnemyName(GameObject obj)
        {
            if (obj.GetComponent<EnemyGnomeDirector>() != null) return "Enemy - Gnome Director";
            if (obj.GetComponent<EnemyBangDirector>() != null) return "Enemy - Bang Director";

            var parent = obj.GetComponent<EnemyParent>();
            if (parent != null)
            {
                return $"{parent.name}";
            }

            return obj.name.StartsWith("Enemy") ? obj.name : null;
        }
    }
}
