﻿using RepoRanked.API;
using RepoRankedApiResponseModel;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RepoRanked.LevelControllers
{
    public class RankedGameManager : MonoBehaviour
    {
        public static RankedGameManager Instance { get; private set; } = null!;

        public static long LastMatchId { get; private set; } = -1;
        private Coroutine? pingCoroutine;
        public MatchmakingStatusResponse matchData;
        private long localSteamId;
        private float pingInterval = 3f;

        public static void StartMatch(MatchmakingStatusResponse matchInfo)
        {
            if (Instance != null)
            {
                Debug.LogWarning("RankedGameManager already running.");
                return;
            }

            var obj = new GameObject("RankedGameManager");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<RankedGameManager>();
            Instance.BeginMatch(matchInfo);
        }

        private void BeginMatch(MatchmakingStatusResponse matchInfo)
        {
            LastMatchId = matchInfo.MatchId;
            matchData = matchInfo;
            localSteamId = (long)SteamClient.SteamId.Value;

            Debug.Log($"[REPORanked] Starting match {matchInfo.MatchId} with seed: {matchInfo.Seed}");
            OpponentStatusUI.CreateUI();
            // Find the opponent
            foreach (var kvp in matchInfo.Players)
            {
                if (kvp.Key != localSteamId)
                {
                    OpponentStatusUI.Instance.SetOpponentName(kvp.Value);
                    break;
                }
            }

            RankedGameManagerUtils.CreateSave(matchInfo);
            SemiFunc.MenuActionSingleplayerGame("REPORanked");


            // Begin periodic ping
            pingCoroutine = StartCoroutine(PingLoop());

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

            var result = await DanosAPI.SendMatchPing(matchData.MatchId, localSteamId, progress, extracts, timestamp);
            if (result == null)
            {
                Debug.LogWarning("[REPORanked] Failed to ping match.");
                return;
            }
            OpponentStatusUI.Instance?.SetOpponentProgress(
    result.OpponentProgress,
    result.OpponentExtractions
);


            if (result?.GameOver == true)
            {
                Debug.Log("[REPORanked] Opponent has finished or disconnected.");
                EndMatch();
                // TODO: show result screen or return to menu
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

            //PlayerAvatar.instance.playerHealth.health = 0;
            //PlayerAvatar.instance.playerHealth.Hurt(1, savingGrace: false);


            OpponentStatusUI.Instance?.DestroyUI();

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

        private int GetCompletedExtractions()
        {
            //extractionPointsCompleted
            int completed = RoundDirector.instance != null ? RoundDirector.instance.extractionPointsCompleted : 0;

            return completed;
        }
    }
}
