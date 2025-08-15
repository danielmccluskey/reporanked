using Newtonsoft.Json;
using RepoRanked.MainMenu;
using RepoRankedApiResponseModel;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RepoRanked.LevelControllers
{
    public class FirstToTenStatusUI : MonoBehaviour
    {
        public static FirstToTenStatusUI Instance { get; private set; } = null!;

        private TextMeshProUGUI playersText;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void CreateUI()
        {
            Debug.Log("[REPORanked] Creating FirstToTenStatusUI");
            if (Instance != null) return;

            var canvasObj = new GameObject("FirstToTenCanvas", typeof(Canvas));
            var canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            DontDestroyOnLoad(canvasObj);



            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080); // pick your design res
            scaler.matchWidthOrHeight = 1f; // 0 = width, 1 = height


            var uiObj = new GameObject("FirstToTenStatusUI", typeof(RectTransform));
            uiObj.transform.SetParent(canvas.transform);
            var rect = uiObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 500);
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(10, 0);

            var manager = uiObj.AddComponent<FirstToTenStatusUI>();
            manager.BuildUI(uiObj.transform);
        }

        private void BuildUI(Transform parent)
        {
            var playersObj = new GameObject("PlayersText", typeof(TextMeshProUGUI));
            playersObj.transform.SetParent(parent);
            playersText = playersObj.GetComponent<TextMeshProUGUI>();
            playersText.enableAutoSizing = true;
            playersText.fontSizeMin = 12f;
            playersText.fontSizeMax = 36f;

            playersText.alignment = TextAlignmentOptions.TopLeft;
            playersText.enableWordWrapping = true;


            var rt = playersText.rectTransform;
            rt.pivot = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(10, -10);
            rt.sizeDelta = new Vector2(480, 480);

            if (DanosMainMenuManager.fontAsset != null)
            {
                playersText.font = DanosMainMenuManager.fontAsset;
            }

            playersText.color = GetColor();
        }

        public Color GetColor()
        {
            return new Color(1, 0.449203f, 0, 1);
        }

        public void UpdatePlayers(List<MatchPingFTTResponse> data, Dictionary<long, string> PlayersNames)
        {
            if (playersText == null || data == null) return;

            var sb = new System.Text.StringBuilder();
            foreach (var player in data.OrderByDescending(p => p.Level))
            {
                if (player.OpponentProgress == 0 && player.OpponentExtractions == 0)
                {
                    player.OpponentExtractions = 1;
                }

                player.Level += 1;


                string playerName = PlayersNames.ContainsKey(player.OpponentSteamID) ? PlayersNames[player.OpponentSteamID] : "Unknown Player";
                if(player.InShop == 1)
                {
                    sb.AppendLine($"{playerName} - {player.Level} - In Shop");

                }
                else
                {
                    sb.AppendLine($"{playerName} - {player.Level} - {player.OpponentProgress}/{player.OpponentExtractions}");
                }
            }

            playersText.text = sb.ToString();
        }

        public void DestroyUI()
        {
            Destroy(gameObject);
            Instance = null;
        }
    }
}
