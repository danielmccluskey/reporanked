using MenuLib.MonoBehaviors;
using MenuLib;
using RepoRanked.API;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using RepoRanked.LevelControllers;
using RepoRankedApiResponseModel;
using RepoRanked.MainMenu.QueuePollers;

namespace RepoRanked.MainMenu
{
    public class DanosLobbyQueuePoller : MonoBehaviour
    {
        public static DanosLobbyQueuePoller Instance { get; private set; } = null!;
        private Coroutine? pollingCoroutine;
        private REPOPopupPage? queuePage;
        private REPOLabel? statusLabel;
        private REPOLabel? playersLabel;
        private REPOLabel? countdownLabel;

        public static void Create()
        {
            if (Instance != null) return;

            var obj = new GameObject("DanosLobbyQueuePoller");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<DanosLobbyQueuePoller>();
        }

        public void StartPolling(REPOPopupPage? pageToClose)
        {
            if (pollingCoroutine != null) return;

            pageToClose?.ClosePage(true);
            var queuePosition = new Vector2(400, 80);

            queuePage = MenuAPI.CreateREPOPopupPage("First To Ten Lobby", REPOPopupPage.PresetSide.Right, false, true, 1.5f);

            queuePage.AddElement(parent =>
            {
                statusLabel = MenuAPI.CreateREPOLabel("Joining lobby...", parent, new Vector2(400, 250));
                statusLabel.labelTMP.fontSize = 10;
                statusLabel.labelTMP.color = Color.white;
            });

            queuePage.AddElement(parent =>
            {
                playersLabel = MenuAPI.CreateREPOLabel("Waiting for players...", parent, new Vector2(400, 220));
                playersLabel.labelTMP.fontSize = 10;
                playersLabel.labelTMP.color = Color.white;
            });
            queuePage.AddElement(parent =>
            {
                countdownLabel = MenuAPI.CreateREPOLabel("", parent, new Vector2(400, 270));
                countdownLabel.labelTMP.fontSize = 10;
                countdownLabel.labelTMP.color = Color.yellow;
            });


            queuePage.AddElement(parent =>
            {
                MenuAPI.CreateREPOButton("Cancel", async () =>
                {
                    var steamId = (long)SteamClient.SteamId.Value;
                    var success = await DanosAPI.LeaveLobby(steamId);
                    if (success)
                    {
                        DanosLobbyQueuePoller.Instance?.StopPolling();
                    }
                    else
                    {
                        Debug.LogError("Failed to leave lobby.");
                    }
                }, parent, queuePosition);
            });

            queuePage.OpenPage(false);
            pollingCoroutine = StartCoroutine(PollLobbyStatus());
        }

        private IEnumerator PollLobbyStatus()
        {
            DanosMatchQueuePoller.QueueType = QueueTypes.firsttoten;

            long steamId = (long)SteamClient.SteamId.Value;
            while (true)
            {
                yield return new WaitForSeconds(1f);

                var task = CheckLobbyStatusAsync(steamId);
                while (!task.IsCompleted) yield return null;

                if (task.Result)
                {
                    pollingCoroutine = null;
                    yield break;
                }
            }
        }

        private async Task<bool> CheckLobbyStatusAsync(long steamId)
        {
            var status = await DanosAPI.GetLobbyStatus(steamId);
            if (status == null)
            {
                statusLabel?.labelTMP.SetText("Failed to get lobby status.");
                return false;
            }

            statusLabel?.labelTMP.SetText($"Status: {status.Status}");

            var sb = new StringBuilder();
            sb.AppendLine($"Players ({status.Players.Count}):");

            foreach (var kvp in status.Players)
            {
                sb.AppendLine(kvp.Value);
            }

            playersLabel?.labelTMP.SetText(sb.ToString());

            if (status.Status == "started" && status.MatchInfo != null)
            {
                queuePage?.ClosePage(true);
                if (status.MatchInfo.QueueType == QueueTypes.firsttoten)
                {
                    FirstToTenGameManager.StartMatch(status.MatchInfo);
                    return true;
                }
            }

            if (status.CountdownStarted.HasValue && status.RemainingSeconds.HasValue && status.TotalRequiredSeconds.HasValue)
            {
                countdownLabel?.labelTMP.SetText($"Countdown: {status.RemainingSeconds.Value}s remaining");
            }
            else
            {
                countdownLabel?.labelTMP.SetText(""); // Hide if not active
            }


            return false;
        }

        public void StopPolling()
        {
            if (pollingCoroutine != null)
            {
                StopCoroutine(pollingCoroutine);
                pollingCoroutine = null;
            }

            queuePage?.ClosePage(false);
        }
    }
}
