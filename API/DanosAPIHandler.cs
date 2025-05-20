using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace RepoRanked.API
{
    public static class DanosAPIHandler
    {
        private static string baseUrl = "https://reporankedapi.splitstats.io/api"; // change this to your API base URL
        
        public static async Task<T?> GetAsync<T>(string endpoint)
        {
            //baseUrl = "https://localhost:7240/api";
            using var request = UnityWebRequest.Get($"{baseUrl}/{endpoint}");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"GET {endpoint} failed: {request.error}");
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse response for {endpoint}: {ex.Message}");
                return default;
            }
        }

        public static async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var json = JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(json);

            using var request = new UnityWebRequest($"{baseUrl}/{endpoint}", UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"POST {endpoint} failed: {request.error}");
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse response from {endpoint}: {ex.Message}");
                return default;
            }
        }

        public static async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data)
        {
            var json = JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(json);

            using var request = new UnityWebRequest($"{baseUrl}/{endpoint}", UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"POST {endpoint} failed: {request.error}");
                return false;
            }

            return true;
        }
    }
}
