using RepoRankedApiResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.WebRequestMethods;

namespace RepoRanked.API
{
    public static partial class DanosAPI
    {
        public static async Task<LobbyStatusResponse?> GetLobbyStatus(long steamId)
        {
            var request = new LobbyStatusRequest { SteamId = steamId };
            //api/lobby/status
            return await DanosAPIHandler.PostAsync<LobbyStatusRequest,LobbyStatusResponse>(
                $"lobby/status", request
            );
        }

        public static async Task<bool> LeaveLobby(long steamId)
        {
            var request = new LobbyLeaveRequest { SteamId = steamId };
            //api/lobby/leave
            var result = await DanosAPIHandler.PostAsync<LobbyLeaveRequest, LobbyLeaveResponse>(
                "lobby/leave", request
            );
            if (result == null)
            {
                Debug.LogError("Failed to leave lobby, result is null.");
                return false;
            }
            return result.Success;
        }




        
    }
}
