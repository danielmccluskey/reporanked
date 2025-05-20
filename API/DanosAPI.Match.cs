using RepoRankedApiResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepoRanked.API
{
    public static partial class DanosAPI
    {
        public static async Task<MatchPingResponse?> SendMatchPing(long matchId, long steamId, int progress, int extracts, long timestamp)
        {
            var payload = new MatchPingRequest
            {
                MatchId = matchId,
                SteamId = steamId,
                Progress = extracts,
                ExtractionsCompleted = progress,
                Timestamp = timestamp
            };

            return await DanosAPIHandler.PostAsync<object, MatchPingResponse>("match/ping", payload);
        }

        public static async Task<MatchCompleteResponse?> CompleteMatch(long matchId, long steamId, long timestamp, string reason)
        {
            var payload = new MatchCompleteRequest
            {
                MatchId = matchId,
                SteamId = steamId,
                Timestamp = timestamp,
                EndReason = reason
            };
            return await DanosAPIHandler.PostAsync<object, MatchCompleteResponse>("match/complete", payload);
        }

        public static async Task<MatchResultResponse?> GetMatchResults(long matchId)
        {
            return await DanosAPIHandler.GetAsync<MatchResultResponse>($"match/results?match_id={matchId}");
        }


    }
}
