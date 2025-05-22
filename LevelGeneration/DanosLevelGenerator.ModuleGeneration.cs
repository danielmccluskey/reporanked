using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static LevelGenerator;
using UnityEngine;

namespace RepoRanked.LevelGeneration
{
    public static class ListExtensions
    {
        public static void ShuffleWithRng<T>(this List<T> list, System.Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }

    public partial class DanosLevelGenerator
    {

        private static IEnumerator ModuleGeneration(LevelGenerator instance)
        {
            instance.waitingForSubCoroutine = true;
            instance.State = LevelState.ModuleGeneration;
            instance.ModulesNormalShuffled_1 = new List<GameObject>();
            instance.ModulesNormalShuffled_2 = new List<GameObject>();
            instance.ModulesNormalShuffled_3 = new List<GameObject>();
            instance.ModulesPassageShuffled_1 = new List<GameObject>();
            instance.ModulesPassageShuffled_2 = new List<GameObject>();
            instance.ModulesPassageShuffled_3 = new List<GameObject>();
            instance.ModulesDeadEndShuffled_1 = new List<GameObject>();
            instance.ModulesDeadEndShuffled_2 = new List<GameObject>();
            instance.ModulesDeadEndShuffled_3 = new List<GameObject>();
            instance.ModulesExtractionShuffled_1 = new List<GameObject>();
            instance.ModulesExtractionShuffled_2 = new List<GameObject>();
            instance.ModulesExtractionShuffled_3 = new List<GameObject>();
            instance.ModuleRarity1 = instance.DifficultyCurve1.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
            instance.ModuleRarity2 = instance.DifficultyCurve2.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
            instance.ModuleRarity3 = instance.DifficultyCurve3.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
            if (!instance.DebugModule)
            {
                instance.ModulesNormalShuffled_1.AddRange(instance.Level.ModulesNormal1);
                instance.ModulesNormalShuffled_1.ShuffleWithRng(Instance.rng);
                instance.ModulesNormalShuffled_2.AddRange(instance.Level.ModulesNormal2);
                instance.ModulesNormalShuffled_2.ShuffleWithRng(Instance.rng);
                instance.ModulesNormalShuffled_3.AddRange(instance.Level.ModulesNormal3);
                instance.ModulesNormalShuffled_3.ShuffleWithRng(Instance.rng);
                instance.ModulesPassageShuffled_1.AddRange(instance.Level.ModulesPassage1);
                instance.ModulesPassageShuffled_1.ShuffleWithRng(Instance.rng);
                instance.ModulesPassageShuffled_2.AddRange(instance.Level.ModulesPassage2);
                instance.ModulesPassageShuffled_2.ShuffleWithRng(Instance.rng);
                instance.ModulesPassageShuffled_3.AddRange(instance.Level.ModulesPassage3);
                instance.ModulesPassageShuffled_3.ShuffleWithRng(Instance.rng);
                instance.ModulesDeadEndShuffled_1.AddRange(instance.Level.ModulesDeadEnd1);
                instance.ModulesDeadEndShuffled_1.ShuffleWithRng(Instance.rng);
                instance.ModulesDeadEndShuffled_2.AddRange(instance.Level.ModulesDeadEnd2);
                instance.ModulesDeadEndShuffled_2.ShuffleWithRng(Instance.rng);
                instance.ModulesDeadEndShuffled_3.AddRange(instance.Level.ModulesDeadEnd3);
                instance.ModulesDeadEndShuffled_3.ShuffleWithRng(Instance.rng);
                instance.ModulesExtractionShuffled_1.AddRange(instance.Level.ModulesExtraction1);
                instance.ModulesExtractionShuffled_1.ShuffleWithRng(Instance.rng);
                instance.ModulesExtractionShuffled_2.AddRange(instance.Level.ModulesExtraction2);
                instance.ModulesExtractionShuffled_2.ShuffleWithRng(Instance.rng);
                instance.ModulesExtractionShuffled_3.AddRange(instance.Level.ModulesExtraction3);
                instance.ModulesExtractionShuffled_3.ShuffleWithRng(Instance.rng);
            }
            else
            {
                instance.ModulesNormalShuffled_1.Add(instance.DebugModule);
                instance.ModulesPassageShuffled_1.Add(instance.DebugModule);
                instance.ModulesDeadEndShuffled_1.Add(instance.DebugModule);
                instance.ModulesExtractionShuffled_1.Add(instance.DebugModule);
            }
            if (instance.ModulesNormalShuffled_1.Count == 0)
            {
                instance.waitingForSubCoroutine = false;
                yield break;
            }
            for (int x = 0; x < instance.LevelWidth; x++)
            {
                for (int y = 0; y < instance.LevelHeight; y++)
                {
                    if (!instance.LevelGrid[x, y].active)
                    {
                        continue;
                    }
                    Vector3 rotation = Vector3.zero;
                    Vector3 position = new Vector3((float)x * ModuleWidth * TileSize - (float)(instance.LevelWidth / 2) * ModuleWidth * TileSize, 0f, (float)y * ModuleWidth * TileSize + ModuleWidth * TileSize / 2f);
                    if (!instance.DebugNormal && !instance.DebugPassage && !instance.DebugDeadEnd && instance.LevelGrid[x, y].type == Module.Type.Extraction)
                    {
                        if (instance.GridCheckActive(x, y - 1))
                        {
                            rotation = Vector3.zero;
                        }
                        if (instance.GridCheckActive(x - 1, y))
                        {
                            rotation = new Vector3(0f, 90f, 0f);
                        }
                        if (instance.GridCheckActive(x, y + 1))
                        {
                            rotation = new Vector3(0f, 180f, 0f);
                        }
                        if (instance.GridCheckActive(x + 1, y))
                        {
                            rotation = new Vector3(0f, -90f, 0f);
                        }
                        instance.SpawnModule(x, y, position, rotation, Module.Type.Extraction);
                        continue;
                    }
                    if (instance.DebugDeadEnd || (!instance.DebugNormal && !instance.DebugPassage && instance.LevelGrid[x, y].type == Module.Type.DeadEnd))
                    {
                        if (instance.GridCheckActive(x, y - 1))
                        {
                            rotation = Vector3.zero;
                        }
                        if (instance.GridCheckActive(x - 1, y))
                        {
                            rotation = new Vector3(0f, 90f, 0f);
                        }
                        if (instance.GridCheckActive(x, y + 1))
                        {
                            rotation = new Vector3(0f, 180f, 0f);
                        }
                        if (instance.GridCheckActive(x + 1, y))
                        {
                            rotation = new Vector3(0f, -90f, 0f);
                        }
                        instance.SpawnModule(x, y, position, rotation, Module.Type.DeadEnd);
                        continue;
                    }
                    if (!instance.DebugNormal && (instance.DebugPassage || instance.PassageAmount < instance.Level.PassageMaxAmount))
                    {
                        if (instance.DebugPassage || (instance.GridCheckActive(x, y + 1) && (instance.GridCheckActive(x, y - 1) || instance.LevelGrid[x, y].first) && !instance.GridCheckActive(x + 1, y) && !instance.GridCheckActive(x - 1, y)))
                        {
                            if (Instance.rng.Next(0, 100) < 50)
                            {
                                rotation = new Vector3(0f, 180f, 0f);
                            }
                            instance.SpawnModule(x, y, position, rotation, Module.Type.Passage);
                            instance.PassageAmount++;
                            continue;
                        }
                        if (!instance.LevelGrid[x, y].first && instance.GridCheckActive(x + 1, y) && instance.GridCheckActive(x - 1, y) && !instance.GridCheckActive(x, y + 1) && !instance.GridCheckActive(x, y - 1))
                        {
                            rotation = new Vector3(0f, 90f, 0f);
                            if (Instance.rng.Next(0, 100) < 50)
                            {
                                rotation = new Vector3(0f, -90f, 0f);
                            }
                            instance.SpawnModule(x, y, position, rotation, Module.Type.Passage);
                            instance.PassageAmount++;
                            continue;
                        }
                    }
                    rotation = instance.ModuleRotations[Instance.rng.Next(0, instance.ModuleRotations.Length)];
                    instance.SpawnModule(x, y, position, rotation, Module.Type.Normal);
                    yield return null;
                }
            }
            instance.waitingForSubCoroutine = false;
        }
    }
}
