using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
using RepoRanked.LevelControllers;
using RepoRanked.Mods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public class DanosMainMenuManager : MonoBehaviour
    {
        public static DanosMainMenuManager Instance { get; private set; } = null!;
        public REPOPopupPage? eloPopupPage;
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

            

            foreach (Transform child in parent)
            {

                switch (child.name)
                {
                    case "Menu Button - host game":
                        hostPosition = child.localPosition;
                        Destroy(child.gameObject);
                        break;

                    case "Menu Button - join game":
                        joinPosition = child.localPosition;
                        Destroy(child.gameObject);
                        break;

                    case "Menu Button - singleplayer":
                    case "Menu Button - Tutorial":
                        Destroy(child.gameObject);
                        break;
                }
            }

            var rankedButton = MenuAPI.CreateREPOButton(
                "REPORanked",
                async () =>
                {
                    if (isProcessing)
                    {
                        return;
                    }
                    isProcessing = true;



                    Debug.Log("REPORanked clicked!");
                    DanosModChecker.Create();
                    DanosModChecker.Instance.ForceCheck();


                    if (DanosModChecker.Instance != null && !DanosModChecker.Instance.IsCompliant)
                    {
                       ShowErrorPopup("You are using disallowed mods. \nWe do this to keep competitive integrity.\n\nAllowed mods are:\n-REPORanked\n-MenuLib\n\n\nPlease remove any others to play :).");
                       isProcessing = false;
                       return;
                    }


                    var response = await DanosAPI.RegisterPlayer((long)SteamClient.SteamId.Value, SteamClient.Name);
                    if (response != null)
                    {


                        Debug.Log($"Registered with Elo {response.EloRating}");



                        if (response.Status == "ok")
                        {
                            ShowEloPopup(response.EloRating);
                        }
                        else if (response.Status == "error")
                        {
                            ShowErrorPopup(response.Message);
                        }
                        else
                        {
                            Debug.LogError("Unknown response status.");
                        }
                    }

                    else
                    {
                        Debug.LogError("Failed to register player.");

                    }

                    isProcessing = false;
                },
                parent,
                localPosition: hostPosition
            );


                //Add a discord button that uses Application.OpenURL
                MenuAPI.CreateREPOButton(
                    "Join Discord",
                    () =>
                    {
                        Application.OpenURL(DiscordInviteUrl);
                    },
                    parent,
                    localPosition: hostPosition + new Vector2(0, -40) // place below the main button
                );




                if (RankedGameManager.LastMatchId != -1)
                {
                    MenuAPI.CreateREPOButton(
                        "Last Match Results",
                        async () =>
                        {
                            if (isProcessing) return;
                            isProcessing = true;

                            var matchId = RankedGameManager.LastMatchId;
                            var response = await DanosAPI.GetMatchResults(matchId);
                            if (response != null)
                            {
                                var popup = MenuAPI.CreateREPOPopupPage(
                                    "Last Match Results",
                                    REPOPopupPage.PresetSide.Right,
                                    shouldCachePage: false,
                                    pageDimmerVisibility: true,
                                    spacing: 1.5f
                                );

                                popup.AddElement(scrollParent =>
                                {
                                    var header = $"Match #{response.MatchId}";
                                    MenuAPI.CreateREPOLabel(header, scrollParent, new Vector2(400,260));
                                });

                                int y = 220;
                                foreach (var player in response.Players)
                                {
                                    var steamId = player.Key;
                                    var name = player.Value;
                                    var result = response.Results.ContainsKey(steamId) ? response.Results[steamId] : "unknown";
                                    var eloChange = response.EloChanges.ContainsKey(steamId)
                                        ? response.EloChanges[steamId] - 0
                                        : 0;

                                    string display = $"{name} - {result}, Elo Δ: {eloChange}";
                                    popup.AddElement(scrollParent =>
                                    {
                                        var lab = MenuAPI.CreateREPOLabel(display, scrollParent, new Vector2(400, y));

                                        lab.labelTMP.fontSize = 10;

                                        fontAsset = lab.labelTMP.font;
                                        y -= 30;
                                    });
                                }

                                popup.AddElement(scrollParent =>
                                {
                                    MenuAPI.CreateREPOButton("Close", () => popup.ClosePage(false), scrollParent, new Vector2(400, y - 30));
                                });

                                popup.OpenPage(false);
                            }
                            else
                            {
                                Debug.LogError("Failed to retrieve match result.");
                            }

                            isProcessing = false;
                        },
                        parent,
                        localPosition: hostPosition + new Vector2(0, -80) // place below the main button
                    );
                }

            });

        }

        private void ShowEloPopup(int elo)
        {
            if (eloPopupPage == null)
            {
                eloPopupPage = MenuAPI.CreateREPOPopupPage(
                    "REPORanked",
                    REPOPopupPage.PresetSide.Right,
                    shouldCachePage: true,
                    pageDimmerVisibility: true,
                    spacing: 1.5f
                );

                eloPopupPage.AddElement(parent =>
                {
                    MenuAPI.CreateREPOLabel($"Elo Rating: {elo}", parent, new Vector2(400, 220));
                });
                eloPopupPage.AddElement(parent =>
                {
                    

                    MenuAPI.CreateREPOButton("Queue for Match", async () =>
                    {
                        if (isProcessing)
                        {
                            return;
                        }
                        isProcessing = true;
                        var steamId = (long)SteamClient.SteamId.Value;

                        var success = await DanosAPI.Enqueue(steamId);
                        if (success)
                        {
                            DanosMatchQueuePoller.Create();
                            DanosMatchQueuePoller.Instance.StartPolling();
                        }
                        else
                        {
                            Debug.LogError("Failed to queue for match.");
                        }

                        isProcessing = false;
                    }, parent, new Vector2(400, 180));


                });

                eloPopupPage.AddElement(parent =>
                {
                    MenuAPI.CreateREPOButton("Close", () => eloPopupPage.ClosePage(false), parent, new Vector2(400, 140));
                });


                



                
            }

            eloPopupPage.OpenPage(false);
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

    }


}
