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


        //Same for amounts per level
        public static void LogLevelAmounts()
        {
            var output = new Dictionary<int, Dictionary<string, int>>();

            for (int levelsCompleted = 0; levelsCompleted <= 150; levelsCompleted++)
            {
                RunManager.instance.levelsCompleted = levelsCompleted;

                float time;
                Dictionary<string, int> levelData = new();

                if (SemiFunc.RunGetDifficultyMultiplier2() > 0f && !SemiFunc.RunIsArena())
                {
                    time = SemiFunc.RunGetDifficultyMultiplier2();

                    levelData["totalMaxValue"] = Mathf.RoundToInt(ValuableDirector.instance.totalMaxValueCurve2.Evaluate(time));
                    levelData["tiny"] = Mathf.RoundToInt(ValuableDirector.instance.tinyMaxAmountCurve2.Evaluate(time));
                    levelData["small"] = Mathf.RoundToInt(ValuableDirector.instance.smallMaxAmountCurve2.Evaluate(time));
                    levelData["medium"] = Mathf.RoundToInt(ValuableDirector.instance.mediumMaxAmountCurve2.Evaluate(time));
                    levelData["big"] = Mathf.RoundToInt(ValuableDirector.instance.bigMaxAmountCurve2.Evaluate(time));
                    levelData["wide"] = Mathf.RoundToInt(ValuableDirector.instance.wideMaxAmountCurve2.Evaluate(time));
                    levelData["tall"] = Mathf.RoundToInt(ValuableDirector.instance.tallMaxAmountCurve2.Evaluate(time));
                    levelData["veryTall"] = Mathf.RoundToInt(ValuableDirector.instance.veryTallMaxAmountCurve2.Evaluate(time));
                }
                else
                {
                    time = SemiFunc.RunGetDifficultyMultiplier1();

                    levelData["totalMaxValue"] = Mathf.RoundToInt(ValuableDirector.instance.totalMaxValueCurve1.Evaluate(time));
                    levelData["tiny"] = Mathf.RoundToInt(ValuableDirector.instance.tinyMaxAmountCurve1.Evaluate(time));
                    levelData["small"] = Mathf.RoundToInt(ValuableDirector.instance.smallMaxAmountCurve1.Evaluate(time));
                    levelData["medium"] = Mathf.RoundToInt(ValuableDirector.instance.mediumMaxAmountCurve1.Evaluate(time));
                    levelData["big"] = Mathf.RoundToInt(ValuableDirector.instance.bigMaxAmountCurve1.Evaluate(time));
                    levelData["wide"] = Mathf.RoundToInt(ValuableDirector.instance.wideMaxAmountCurve1.Evaluate(time));
                    levelData["tall"] = Mathf.RoundToInt(ValuableDirector.instance.tallMaxAmountCurve1.Evaluate(time));
                    levelData["veryTall"] = Mathf.RoundToInt(ValuableDirector.instance.veryTallMaxAmountCurve1.Evaluate(time));
                }

                output[levelsCompleted] = levelData;
            }

            string json = JsonConvert.SerializeObject(output, Formatting.Indented);
            File.WriteAllText(Path.Combine(Paths.BepInExRootPath, "levelAmounts.json"), json);
            RepoRanked.Logger.LogInfo("Generated levelAmounts.json");
        }



    }
}
