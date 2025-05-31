using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
using RepoRanked.LevelControllers;
using RepoRanked.Mods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager : MonoBehaviour
    {
        public static DanosMainMenuManager Instance { get; private set; } = null!;

        public static bool SpawnedButtonWarning = false;
        
        Vector2 hostPosition = Vector2.zero;
        Vector2 joinPosition = Vector2.zero;
        private const string DiscordInviteUrl = "https://discord.gg/y7WBzTT9jF";

        public static TMP_FontAsset fontAsset;

        private bool isProcessing = false;


        public static void Create()
        {
            if (Instance != null)
                return;

            var obj = new GameObject("DanosMainMenuManager");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<DanosMainMenuManager>();
        }

        public void Init()
        {

            MenuAPI.AddElementToMainMenu(parent =>
            {
                var menuPage = parent.GetComponent<MenuPage>();

                if (menuPage == null)
                {
                    Debug.Log("MenuPage component not found in the parent object.");
                    return;
                }

                RemoveMainButtons(parent);

                // popup to explain the buttons a bit
                if (!SpawnedButtonWarning) // Avoid opening the menu more than once each time you open the game
                    MenuManager.instance.PagePopUpScheduled("RepoRankeds buttons warning", Color.yellow, "To avoid possible bugs, please be patient when pressing a button. Depending on your network, when you press a button, the menu could dissapear for some time. Avoid opening menus again without waiting at least one second.", "Let me play...");
                SpawnedButtonWarning = true;

                var rankedbutton = GetRankedButton(parent);
                var discordButton = GetDiscordButton(parent);

                if (RankedGameManager.LastMatchId != -1)
                {
                    var lastMatchButton = GetLastMatchButton(parent);
                    _ = RunLastMatchCheckAsync();
                    
                }
                var unrankedButton = GetUnRankedButton(parent);
                var passButton = GetPassphraseButton(parent);

            });

        }

        private async Task RunLastMatchCheckAsync() // Get the results for the popup
        {
            var matchId = RankedGameManager.LastMatchId;
            var response = await DanosAPI.GetMatchResults(matchId);
            if (response == null)
                return;

            string playerResults = "";
            foreach (var player in response.Players)
            {
                var steamId = player.Key;
                var name = player.Value;
                var result = response.Results.ContainsKey(steamId) ? response.Results[steamId] : "unknown";
                var eloChange = response.EloChanges.ContainsKey(steamId)
                    ? response.EloChanges[steamId] - 0
                    : 0;

                string display = $"{name} - {result}, Elo Δ: {eloChange}";
                playerResults = playerResults + "\n" + display;
            }

            MenuManager.instance.PagePopUpScheduled($"{((response.WinnerSteamId == (long)SteamClient.SteamId.Value) ? "WON" : "LOST")} MATCH #{response.MatchId}", (response.WinnerSteamId == (long)SteamClient.SteamId.Value) ? Color.green : Color.red, $"Elo Δ changed: {response.EloChanges[(long)SteamClient.SteamId.Value]}\n{playerResults}", "GG");
        }




        private REPOPopupPage ShowErrorPopup(string message)
        {
            var popup = MenuAPI.CreateREPOPopupPage(
                "Notice",
                REPOPopupPage.PresetSide.Right,
                shouldCachePage: false,
                pageDimmerVisibility: true,
                spacing: 1.5f
            );

            popup.AddElement(parent =>
            {
                var lab = MenuAPI.CreateREPOLabel(message, parent, new Vector2(400, 220));

                lab.labelTMP.fontSize = 10;
            });

            popup.AddElement(parent =>
            {
                MenuAPI.CreateREPOButton("Close", () => popup.ClosePage(false), parent, new Vector2(400, 100));
            });

            popup.OpenPage(false);

            return popup;
        }



        private void ShowPrivacyConsentPopup(Action onAccept)
        {
            var popup = MenuAPI.CreateREPOPopupPage(
                "Privacy Notice",
                REPOPopupPage.PresetSide.Right,
                shouldCachePage: false,
                pageDimmerVisibility: true,
                spacing: 1.5f
            );

            popup.AddElement(parent =>
            {
                var lab = MenuAPI.CreateREPOLabel(
                    "To use REPORanked, we collect your SteamID and match data.\n" +
                    "This is used solely for matchmaking, match tracking and\nElo calculations.\n\n" +
                    "As this is still in development, we cannot offer data deletion\nmethods just yet.\n\n" +
                    "By continuing, you consent to this data being collected\nand the above conditions regarding data management.",
                    parent,
                    new Vector2(400, 220)
                );
                lab.labelTMP.fontSize = 10;
            });

            popup.AddElement(parent =>
            {
                MenuAPI.CreateREPOButton("Accept", () =>
                {
                    onAccept.Invoke();
                    popup.ClosePage(false);

                }, parent, new Vector2(400, 140));

                MenuAPI.CreateREPOButton("Decline", () =>
                {
                    popup.ClosePage(false);
                }, parent, new Vector2(400, 100));
            });

            popup.OpenPage(false);
        }



    }
}
