using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LevelGenerator;

namespace RepoRanked.LevelGeneration
{
    public partial class DanosLevelGenerator
    {
        private static IEnumerator GenerateBlockObjects(LevelGenerator instance)
        {
            instance.waitingForSubCoroutine = true;
            instance.State = LevelState.BlockObjects;
            float moduleWidth = ModuleWidth * TileSize;
            for (int x = 0; x < instance.LevelWidth; x++)
            {
                for (int y = 0; y < instance.LevelHeight; y++)
                {
                    if (instance.LevelGrid[x, y].active && instance.LevelGrid[x, y].type == Module.Type.Normal)
                    {
                        float num = (float)x * moduleWidth - (float)(instance.LevelWidth / 2) * moduleWidth;
                        float num2 = (float)y * moduleWidth + moduleWidth / 2f;
                        if (y + 1 >= instance.LevelHeight || !instance.LevelGrid[x, y + 1].active)
                        {
                            instance.SpawnBlockObject(new Vector3(num, 0f, num2 + moduleWidth / 2f), new Vector3(0f, 180f, 0f));
                        }
                        if (x + 1 >= instance.LevelWidth || !instance.LevelGrid[x + 1, y].active)
                        {
                            instance.SpawnBlockObject(new Vector3(num + moduleWidth / 2f, 0f, num2), new Vector3(0f, -90f, 0f));
                        }
                        if ((y - 1 < 0 || !instance.LevelGrid[x, y - 1].active) && (x != instance.LevelWidth / 2 || y != 0))
                        {
                            instance.SpawnBlockObject(new Vector3(num, 0f, num2 - moduleWidth / 2f), new Vector3(0f, 0f, 0f));
                        }
                        if (x - 1 < 0 || !instance.LevelGrid[x - 1, y].active)
                        {
                            instance.SpawnBlockObject(new Vector3(num - moduleWidth / 2f, 0f, num2), new Vector3(0f, 90f, 0f));
                        }
                        yield return null;
                    }
                }
            }
            instance.waitingForSubCoroutine = false;
        }
    }
}
