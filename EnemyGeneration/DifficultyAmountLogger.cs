using BepInEx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RepoRanked.EnemyGeneration
{
    public static class DifficultyAmountLogger
    {
        private static readonly Dictionary<int, Dictionary<int, int>> Log = new();

        

        public static void GenerateDifficultyAmountTable(AnimationCurve curve1, AnimationCurve curve2, AnimationCurve curve3)
        {
            int maxLevels = RunManager.instance.levelsMax;
            var output = new Dictionary<int, Dictionary<int, int>>();

            for (int levelsCompleted = 0; levelsCompleted <= 150; levelsCompleted++)
            {
                float multiplier = (float)levelsCompleted / maxLevels;

                int value1 = Mathf.RoundToInt(curve1.Evaluate(multiplier));
                int value2 = Mathf.RoundToInt(curve2.Evaluate(multiplier));
                int value3 = Mathf.RoundToInt(curve3.Evaluate(multiplier));

                output[levelsCompleted] = new Dictionary<int, int>
            {
                { 1, value1 },
                { 2, value2 },
                { 3, value3 }
            };
            }

            string json = JsonConvert.SerializeObject(output, Formatting.Indented);
            File.WriteAllText(Path.Combine(Paths.BepInExRootPath, "difficultyAmounts.json"), json);

            RepoRanked.Logger.LogInfo("Generated difficultyAmounts.json");
        }
    }
}
