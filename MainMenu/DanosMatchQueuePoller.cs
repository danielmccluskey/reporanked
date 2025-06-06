﻿using MenuLib.MonoBehaviors;
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
using Steamworks.Ugc;

namespace RepoRanked.MainMenu
{
    public class DanosMatchQueuePoller : MonoBehaviour
    {
        public static DanosMatchQueuePoller Instance { get; private set; } = null!;
        private Coroutine? pollingCoroutine;
        private REPOPopupPage? queuePage;


        public static QueueTypes QueueType { get; private set; } = QueueTypes.ranked;

        public static void Create(QueueTypes queue)
        {
            if (Instance != null) return;

            var obj = new GameObject("DanosMatchQueuePoller");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<DanosMatchQueuePoller>();

            QueueType = queue;
        }

        public void StartPolling(REPOPopupPage? pageToClose, QueueTypes queue)
        {
            if (pollingCoroutine != null) return;
            QueueType = queue;

            pageToClose?.ClosePage(true);
            var queuePosition = new Vector2(400, 80);
            queuePage = MenuAPI.CreateREPOPopupPage("Searching for Match...", REPOPopupPage.PresetSide.Right, false, true, 1.5f);
            queuePage.AddElement(parent =>
            {
                MenuAPI.CreateREPOButton("Cancel", async () =>
                {
                    var steamId = (long)SteamClient.SteamId.Value;

                    var success = await DanosAPI.DeQueue(steamId);
                    if (success)
                    {
                        DanosMatchQueuePoller.Instance?.StopPolling();

                    }
                    else
                    {
                        Debug.LogError("Failed to dequeue for match.");
                    }
                }, parent, queuePosition);
            });


            queuePage.OpenPage(false);

            pollingCoroutine = StartCoroutine(PollForMatch());
        }

        private IEnumerator PollForMatch()
        {
            long steamId = (long)SteamClient.SteamId.Value;

            while (true)
            {
                yield return new WaitForSeconds(4f);

                // call the async method via a wrapper
                var task = CheckMatchStatusAsync(steamId);
                while (!task.IsCompleted) yield return null;

                if (task.Result)
                {
                    pollingCoroutine = null;



                    yield break;
                }
            }
        }

        private async Task<bool> CheckMatchStatusAsync(long steamId)
        {
            var status = await DanosAPI.GetMatchmakingStatus(steamId);
            if (status?.Status == "matched")
            {
                queuePage?.ClosePage(true);

                

                if(QueueType == QueueTypes.ranked)
                {
                    RankedGameManager.StartMatch(status);

                }
                else if (QueueType == QueueTypes.unranked)
                {
                    RankedGameManager.StartMatch(status);

                }
                else if (QueueType == QueueTypes.monthlychallenge)
                {
                    MonthlyGameManager.StartMatch(status);
                }



                return true;
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
