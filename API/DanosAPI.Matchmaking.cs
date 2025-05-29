using RepoRankedApiResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RepoRanked.API
{
    public static partial class DanosAPI
    {
        

        

        public static async Task<bool> Enqueue(long steamId, QueueTypes queueTypes = QueueTypes.ranked, string passPhrase = "")
        {
            var request = new EnqueueRequest { SteamId = steamId, QueueTypes = queueTypes, PassPhrase = passPhrase };

            var result = await DanosAPIHandler.PostAsync<EnqueueRequest, EnqueueResponse>(
                "matchmaking/enqueue", request
            );

            if (result?.Status == "queued")
            {
                Debug.Log("Player enqueued successfully.");
                return true;
            }

            Debug.LogWarning("Failed to enqueue player.");
            return false;
        }

        //dequeue at /api/matchmaking/cancel
        public static async Task<bool> DeQueue(long steamId)
        {
            var request = new EnqueueRequest { SteamId = steamId };
            var result = await DanosAPIHandler.PostAsync<EnqueueRequest, EnqueueResponse>(
                "matchmaking/cancel", request
            );
            if (result?.Status == "cancelled")
            {
                Debug.Log("Player dequeued successfully.");
                return true;
            }
            Debug.LogWarning("Failed to dequeue player.");
            return false;
        }

        public static async Task<MatchmakingStatusResponse?> GetMatchmakingStatus(long steamId)
        {
            return await DanosAPIHandler.GetAsync<MatchmakingStatusResponse>(
                $"matchmaking/status?steam_id={steamId}"
            );
        }
    }
}
