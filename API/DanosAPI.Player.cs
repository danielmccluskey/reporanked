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
        public static async Task<RegisterPlayerResponse?> RegisterPlayer(long steamId, string displayName)
        {
            var request = new RegisterPlayerRequest
            {
                SteamId = steamId,
                DisplayName = displayName
            };

            var response = await DanosAPIHandler.PostAsync<RegisterPlayerRequest, RegisterPlayerResponse>(
                "player/register", request
            );

            if (response == null)
            {
                Debug.LogError("Failed to register player.");
            }

            return response;
        }
    }
}
