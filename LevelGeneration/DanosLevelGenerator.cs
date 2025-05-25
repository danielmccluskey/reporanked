using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static LevelGenerator;

namespace RepoRanked.LevelGeneration
{
    public partial class DanosLevelGenerator
    {
        public static DanosLevelGenerator? Instance { get; private set; }
        public System.Random rng;
        private DanosLevelGenerator(int seed)
        {
            rng = new System.Random(seed);
        }

        public static void Create(int seed)
        {

            Instance = new DanosLevelGenerator(seed);
        }

        public static void Delete()
        {
            Instance = null;
        }

        public int Range(int min, int max) => rng.Next(min, max);

        


        public static IEnumerator GenerateWithSeed(LevelGenerator instance, int seedBase)
        {
            



            yield return new WaitForSeconds(0.2f);
            if (!SemiFunc.IsMultiplayer())
                instance.AllPlayersReady = true;

            while (!instance.AllPlayersReady)
            {
                instance.State = LevelState.Load;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.2f);

            instance.Level = RunManager.instance.levelCurrent;
            RunManager.instance.levelPrevious = instance.Level;

            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                instance.ModuleAmount = instance.Level.ModuleAmount;
                if (instance.DebugAmount > 0)
                    instance.ModuleAmount = instance.DebugAmount;

                instance.StartCoroutine(DanosLevelGenerator.TileGeneration(instance));
                while (instance.waitingForSubCoroutine)
                    yield return null;

                instance.StartCoroutine(DanosLevelGenerator.StartRoomGeneration(instance));
                while (instance.waitingForSubCoroutine)
                    yield return null;

                instance.StartCoroutine(DanosLevelGenerator.GenerateConnectObjects(instance));
                while (instance.waitingForSubCoroutine)
                    yield return null;

                instance.StartCoroutine(DanosLevelGenerator.ModuleGeneration(instance));
                while (instance.waitingForSubCoroutine)
                    yield return null;

                instance.StartCoroutine(DanosLevelGenerator.GenerateBlockObjects(instance));
                while (instance.waitingForSubCoroutine)
                    yield return null;

                if (GameManager.instance.gameMode == 1)
                    instance.PhotonView.RPC("ModuleAmountRPC", RpcTarget.AllBuffered, instance.ModuleAmount);
            }

            while (instance.ModulesSpawned < instance.ModuleAmount - 1)
            {
                instance.State = LevelState.ModuleSpawnLocal;
                yield return new WaitForSeconds(0.1f);
            }

            if (GameManager.instance.gameMode == 1)
                instance.PhotonView.RPC("ModulesReadyRPC", RpcTarget.AllBuffered);

            while (GameManager.instance.gameMode == 1 && instance.ModulesReadyPlayers < PhotonNetwork.CurrentRoom.PlayerCount)
            {
                instance.State = LevelState.ModuleSpawnRemote;
                yield return new WaitForSeconds(0.1f);
            }

            EnvironmentDirector.Instance.Setup();
            PostProcessing.Instance.Setup();
            LevelMusic.instance.Setup();
            ConstantMusic.instance.Setup();

            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                while (instance.LevelPathPoints.Count == 0)
                {
                    instance.State = LevelState.LevelPoint;
                    yield return new WaitForSeconds(0.1f);
                }

                instance.State = LevelState.Item;
                if (!SemiFunc.IsMultiplayer())
                    instance.ItemSetup();
                else
                    instance.PhotonView.RPC("ItemSetup", RpcTarget.AllBuffered);

                UnityEngine.Random.InitState(seedBase + 5);
                instance.StartCoroutine(DanosValuableGeneration.Instance.SetupHost(ValuableDirector.instance));
                while (!ValuableDirector.instance.setupComplete)
                {
                    instance.State = LevelState.Valuable;
                    yield return new WaitForSeconds(0.1f);
                }

                instance.NavMeshSetup();
                yield return null;

                while (GameDirector.instance.PlayerList.Count == 0)
                {
                    instance.State = LevelState.PlayerSetup;
                    yield return new WaitForSeconds(0.1f);
                }

                instance.PlayerSpawn();
                yield return null;

                while (instance.playerSpawned < GameDirector.instance.PlayerList.Count)
                {
                    instance.State = LevelState.PlayerSpawn;
                    yield return new WaitForSeconds(0.1f);
                }

                if (instance.Level.HasEnemies && !instance.DebugNoEnemy)
                {
                    instance.EnemySetup();
                    yield return null;

                    if (GameManager.Multiplayer())
                    {
                        while (!instance.EnemyReady)
                        {
                            instance.State = LevelState.Enemy;
                            if (instance.EnemyReadyPlayers >= PhotonNetwork.CurrentRoom.PlayerCount || instance.EnemiesSpawnTarget <= 0)
                            {
                                instance.PhotonView.RPC("EnemyReadyAllRPC", RpcTarget.AllBuffered);
                                instance.EnemyReady = true;
                            }
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else
                    {
                        while (instance.EnemiesSpawned < instance.EnemiesSpawnTarget)
                            yield return new WaitForSeconds(0.1f);
                    }
                }

                instance.State = LevelState.Done;
                if (!SemiFunc.IsMultiplayer())
                    instance.GenerateDone();
                else
                    instance.PhotonView.RPC("GenerateDone", RpcTarget.AllBuffered);

                SessionManager.instance.CrownPlayer();
            }
            else
            {
                while (!instance.Generated)
                    yield return new WaitForSeconds(0.1f);
            }


        }

        public static IEnumerator TileGeneration(LevelGenerator instance)
        {
            instance.waitingForSubCoroutine = true;
            instance.State = LevelGenerator.LevelState.Tiles;
            instance.LevelWidth = Mathf.Max(2, Mathf.CeilToInt(instance.LevelWidth * instance.DebugLevelSize));
            instance.LevelHeight = Mathf.Max(2, Mathf.CeilToInt(instance.LevelHeight * instance.DebugLevelSize));
            var LevelGrid = new LevelGenerator.Tile[instance.LevelWidth, instance.LevelHeight];
            for (int i = 0; i < instance.LevelWidth; i++)
            {
                for (int j = 0; j < instance.LevelHeight; j++)
                {
                    LevelGrid[i, j] = new LevelGenerator.Tile { x = i, y = j, active = false };
                }
            }
            instance.GetType().GetField("LevelGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(instance, LevelGrid);

            instance.ExtractionAmount = 0;
            if (instance.ModuleAmount > 4)
            {
                instance.ModuleAmount = Mathf.Min(5 + RunManager.instance.levelsCompleted, 10);
                instance.ModuleAmount = Mathf.CeilToInt(instance.ModuleAmount * instance.DebugLevelSize);
                if (!instance.DebugModule)
                {
                    instance.DeadEndAmount = Mathf.CeilToInt(instance.ModuleAmount / 3);
                    instance.ExtractionAmount = instance.ModuleAmount switch
                    {
                        >= 10 => 3,
                        >= 8 => 2,
                        >= 6 => 1,
                        _ => 0
                    };
                }
            }

            if (instance.Level == RunManager.instance.levelShop)
            {
                instance.DeadEndAmount = 1;
            }

            int moduleAmount = instance.ModuleAmount;
            LevelGrid[instance.LevelWidth / 2, 0].active = true;
            LevelGrid[instance.LevelWidth / 2, 0].first = true;
            moduleAmount--;

            int num = instance.LevelWidth / 2;
            int num2 = 0;

            while (moduleAmount > 0)
            {
                int num3 = -999, num4 = -999;
                while (num + num3 < 0 || num + num3 >= instance.LevelWidth || num2 + num4 < 0 || num2 + num4 >= instance.LevelHeight)
                {
                    num3 = 0;
                    num4 = 0;
                    int num5 = DanosLevelGenerator.Instance.rng.Next(0, (num2 == 1 ? 3 : 4));
                    if (instance.DebugPassage)
                        num5 = 2;

                    switch (num5)
                    {
                        case 0: num3--; break;
                        case 1: num3++; break;
                        case 2: num4++; break;
                        case 3: num4--; break;
                    }
                }

                num += num3;
                num2 += num4;
                if (!LevelGrid[num, num2].active)
                {
                    LevelGrid[num, num2].active = true;
                    moduleAmount--;
                }
            }

            yield return null;

            List<LevelGenerator.Tile> possibleExtractionTiles = new();
            if (!instance.DebugNormal && !instance.DebugPassage && !instance.DebugDeadEnd)
            {
                for (int k = 0; k < instance.LevelWidth; k++)
                {
                    for (int l = 0; l < instance.LevelHeight; l++)
                    {
                        if (!LevelGrid[k, l].active)
                        {
                            int num6 = 0;
                            if (instance.GridGetTile(k, l + 1)?.active == true) num6++;
                            if (instance.GridGetTile(k + 1, l)?.active == true) num6++;
                            if (instance.GridGetTile(k, l - 1)?.active == true) num6++;
                            if (instance.GridGetTile(k - 1, l)?.active == true) num6++;

                            if (num6 == 1)
                                possibleExtractionTiles.Add(LevelGrid[k, l]);
                        }
                    }
                }
            }

            yield return null;

            int num7 = instance.ExtractionAmount;
            var tile5 = new LevelGenerator.Tile { x = instance.LevelWidth / 2, y = -1 };
            List<LevelGenerator.Tile> _extractionTiles = new() { tile5 };

            while (num7 > 0 && possibleExtractionTiles.Count > 0)
            {
                LevelGenerator.Tile tile6 = null;
                float num8 = 0f;
                foreach (var item in possibleExtractionTiles)
                {
                    float closest = _extractionTiles.Min(e => Vector2.Distance(new Vector2(e.x, e.y), new Vector2(item.x, item.y)));
                    if (closest > num8)
                    {
                        num8 = closest;
                        tile6 = item;
                    }
                }
                instance.SetExtractionTile(Module.Type.Extraction, tile6, ref _extractionTiles, ref possibleExtractionTiles);
                num7--;
            }

            yield return null;

            int num11 = instance.DeadEndAmount;
            while (num11 > 0 && possibleExtractionTiles.Count > 0)
            {
                var tile7 = possibleExtractionTiles[DanosLevelGenerator.Instance.rng.Next(possibleExtractionTiles.Count)];
                instance.SetExtractionTile(Module.Type.DeadEnd, tile7, ref _extractionTiles, ref possibleExtractionTiles);
                num11--;
            }

            yield return null;
            instance.waitingForSubCoroutine = false;
        }


        public static IEnumerator StartRoomGeneration(LevelGenerator instance)
        {
            instance.waitingForSubCoroutine = true;
            instance.State = LevelGenerator.LevelState.StartRoom;

            List<GameObject> list = new List<GameObject>();
            list.AddRange(instance.Level.StartRooms);

            // Custom shuffle using seeded RNG
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = DanosLevelGenerator.Instance.rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            if (instance.DebugStartRoom != null)
            {
                list[0] = instance.DebugStartRoom;
            }

            GameObject prefab = list[0];
            GameObject go = (GameManager.instance.gameMode != 0)
                ? PhotonNetwork.InstantiateRoomObject(
                    $"Level/{instance.Level.ResourcePath}/Start Room/{prefab.name}",
                    Vector3.zero,
                    Quaternion.identity,
                    0)
                : UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);

            go.transform.parent = instance.LevelParent.transform;

            yield return null;
            instance.waitingForSubCoroutine = false;
        }

        private static IEnumerator GenerateConnectObjects(LevelGenerator instance)
        {
            instance.waitingForSubCoroutine = true;
            instance.State = LevelState.ConnectObjects;
            float moduleWidth = ModuleWidth * TileSize;
            for (int x = 0; x < instance.LevelWidth; x++)
            {
                for (int y = 0; y < instance.LevelHeight; y++)
                {
                    if (instance.LevelGrid[x, y].active)
                    {
                        if (instance.GridCheckActive(x, y + 1))
                        {
                            instance.LevelGrid[x, y].connections++;
                        }
                        if (instance.GridCheckActive(x, y - 1))
                        {
                            instance.LevelGrid[x, y].connections++;
                        }
                        if (instance.GridCheckActive(x + 1, y))
                        {
                            instance.LevelGrid[x, y].connections++;
                        }
                        if (instance.GridCheckActive(x - 1, y))
                        {
                            instance.LevelGrid[x, y].connections++;
                        }
                        float num = (float)x * moduleWidth - (float)(instance.LevelWidth / 2) * moduleWidth;
                        float num2 = (float)y * moduleWidth + moduleWidth / 2f;
                        if (y + 1 < instance.LevelHeight && instance.LevelGrid[x, y + 1].active && !instance.LevelGrid[x, y + 1].connectedBot)
                        {
                            instance.SpawnConnectObject(new Vector3(num, 0f, num2 + moduleWidth / 2f), Vector3.zero);
                            instance.LevelGrid[x, y].connectedTop = true;
                        }
                        if (x + 1 < instance.LevelWidth && instance.LevelGrid[x + 1, y].active && !instance.LevelGrid[x + 1, y].connectedLeft)
                        {
                            instance.SpawnConnectObject(new Vector3(num + moduleWidth / 2f, 0f, num2), new Vector3(0f, 90f, 0f));
                            instance.LevelGrid[x, y].connectedRight = true;
                        }
                        if ((y - 1 >= 0 && instance.LevelGrid[x, y - 1].active && !instance.LevelGrid[x, y - 1].connectedTop) || (x == instance.LevelWidth / 2 && y == 0))
                        {
                            instance.SpawnConnectObject(new Vector3(num, 0f, num2 - moduleWidth / 2f), Vector3.zero);
                            instance.LevelGrid[x, y].connectedBot = true;
                        }
                        if (x - 1 >= 0 && instance.LevelGrid[x - 1, y].active && !instance.LevelGrid[x - 1, y].connectedRight)
                        {
                            instance.SpawnConnectObject(new Vector3(num - moduleWidth / 2f, 0f, num2), Vector3.zero);
                            instance.LevelGrid[x, y].connectedLeft = true;
                        }
                        yield return null;
                    }
                }
            }
            instance.waitingForSubCoroutine = false;
        }




    }
}
