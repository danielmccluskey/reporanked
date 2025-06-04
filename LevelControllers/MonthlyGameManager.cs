using BepInEx;
using Newtonsoft.Json;
using RepoRanked.API;
using RepoRankedApiResponseModel;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RepoRanked.LevelControllers
{
    public class MonthlyGameManager : MonoBehaviour
    {
        public static MonthlyGameManager Instance { get; private set; } = null!;

        public static long LastMatchId { get; private set; } = -1;
        private Coroutine? pingCoroutine;
        public MatchmakingStatusResponse matchData;
        private long localSteamId;
        private float pingInterval = 4f;


        public FirstToTenData FirstToTenData { get; private set; }
        public int CurrentLevelIndex = 0;


        



        public static void StartMatch(MatchmakingStatusResponse matchInfo)
        {
            if (Instance != null)
            {
                Debug.LogWarning("MonthlyGameManager already running.");
                return;
            }

            var obj = new GameObject("MonthlyGameManager");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<MonthlyGameManager>();
            Instance.BeginMatch(matchInfo);
        }

        private void BeginMatch(MatchmakingStatusResponse matchInfo)
        {
            CurrentLevelIndex = 0; 
            LastMatchId = matchInfo.MatchId;
            matchData = matchInfo;
            localSteamId = (long)SteamClient.SteamId.Value;
            if(matchInfo.FirstToTenData != null)
            {
                FirstToTenData = matchInfo.FirstToTenData;
            }
            MonthlyStatusUI.CreateUI();

            FTTDMapData currentMapData = GetCurrentMapData();

            if (currentMapData == null)
            {
                Debug.LogWarning("[REPORanked] Current map data is null.");
            }
            MonthlyStatusUI.Instance?.SetEnemyHash(currentMapData.Enemies);
            MonthlyStatusUI.Instance?.SetSeed(currentMapData.seed);
            RankedGameManagerUtils.CreateSave(matchInfo);
            SemiFunc.MenuActionSingleplayerGame("REPORanked");


            // Begin periodic ping
            pingCoroutine = StartCoroutine(PingLoop());




            //Debug serialize the FirstToTenData and write it to a file in the game root
            if (FirstToTenData != null)
            {
                string json = JsonConvert.SerializeObject(FirstToTenData);
                var outputPath = Path.Combine(Paths.BepInExRootPath, "FirstToTenData.json");
                try
                {
                    File.WriteAllText(outputPath, json);
                    Debug.Log($"[REPORanked] FirstToTenData saved to {outputPath}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[REPORanked] Failed to save FirstToTenData: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning("[REPORanked] FirstToTenData is null.");
            }

        }


        private IEnumerator PingLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(pingInterval);
                _ = PingMatch();
            }
        }

        private async Task PingMatch()
        {
            var progress = GetCurrentProgress();
            var extracts = GetCompletedExtractions();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var level = GetCurrentLevel();
            var result = await DanosAPI.SendMatchPing(matchData.MatchId, localSteamId, progress, extracts, timestamp, level);
            if (result == null)
            {
                Debug.LogWarning("[REPORanked] Failed to ping match.");
                return;
            }

            FTTDMapData currentMapData = GetCurrentMapData();

            if (currentMapData == null)
            {
                Debug.LogWarning("[REPORanked] Current map data is null.");
                return;
            }

            MonthlyStatusUI.Instance?.SetEnemyHash(currentMapData.Enemies);
            MonthlyStatusUI.Instance?.SetSeed(currentMapData.seed);


            if (result?.GameOver == true)
            {
                Debug.Log("[REPORanked] Opponent has finished or disconnected.");
                EndMatch();
            }
        }


        public void CompleteMatch(string reason)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _ = DanosAPI.CompleteMatch(matchData.MatchId, localSteamId, timestamp, reason);
            EndMatch();
        }

        private void EndMatch()
        {
            if (pingCoroutine != null)
                StopCoroutine(pingCoroutine);

            MonthlyStatusUI.Instance?.DestroyUI();

            foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
            {
                player.OutroStartRPC();
            }
            NetworkManager.instance.leavePhotonRoom = true;

            Destroy(gameObject);
            Instance = null;


        }

        private int GetCurrentProgress()
        {
            int current = RoundDirector.instance != null ? RoundDirector.instance.extractionPoints : 0;
            return current;
        }

        private int GetCurrentLevel()
        {
            int current = SemiFunc.RunGetLevelsCompleted();
            return current;
        }

        private int GetCompletedExtractions()
        {
            //extractionPointsCompleted
            int completed = RoundDirector.instance != null ? RoundDirector.instance.extractionPointsCompleted : 0;

            return completed;
        }

        private int GetCurrentLevelIndex()
        {
            if (FirstToTenData != null)
            {
                return CurrentLevelIndex;
            }
            return 0;
        }

        public FTTDMapData GetCurrentMapData()
        {
            if (FirstToTenData != null && CurrentLevelIndex < FirstToTenData.maps.Count)
            {
                return FirstToTenData.maps[CurrentLevelIndex];
            }
            return null;
        }

        public bool IsLastShop()
        {
            //If the index is the last one in the map data, return true
            if (FirstToTenData != null && CurrentLevelIndex >= FirstToTenData.maps.Count - 1)
            {
                return true;
            }
            return false;
        }

        public void CompletedRound()
        {
            CurrentLevelIndex++;

            if(IsLastShop())
            {
                Debug.Log("[REPORanked] Last shop reached, completing match.");
                CompleteMatch("finished");
                return;
            }



            FTTDMapData currentMapData = GetCurrentMapData();

            if (currentMapData == null)
            {
                Debug.LogWarning("[REPORanked] Current map data is null.");
                return;
            }

            MonthlyStatusUI.Instance?.SetEnemyHash(currentMapData.Enemies);
            MonthlyStatusUI.Instance?.SetSeed(currentMapData.seed);
        }
    }
}
