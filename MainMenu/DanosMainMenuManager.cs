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

                //create a label to the side of the rankedbutton
                var label = MenuAPI.CreateREPOLabel("Hang tight! Buttons may take a second,\navoid clicking multiple times before it loads as it\nmay cause UI errors.", parent, new Vector2(hostPosition.x+160, hostPosition.y));
                label.labelTMP.fontSize = 16;
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
                        if (DanosModChecker.Instance != null && !DanosModChecker.Instance.IsGameVersionCompliant)
                        {
                            ShowErrorPopup("Your game version is not compliant with REPORanked.\n\nPlease update your game to the latest version.\n\nIf you are using the beta branch, please revert to the normal one.");
                            isProcessing = false;
                            return;
                        }

                        ShowPrivacyConsentPopup(async () =>
                        {
                            var response = await DanosAPI.RegisterPlayer((long)SteamClient.SteamId.Value, SteamClient.Name);
                            if (response != null)
                            {
                                Debug.Log($"Registered with Elo {response.EloRating}");
                                if (response.Status == "ok")
                                {
                                    ShowEloPopup(response.EloRating, response.Message, response.Matches);
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
                        });



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
                                    MenuAPI.CreateREPOLabel(header, scrollParent, new Vector2(400, 260));
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

        private void ShowEloPopup(int elo, string bracket, int matches)
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
                    var lab = MenuAPI.CreateREPOLabel($"{matches} matches in the last 30 mins!", parent, new Vector2(400, 260));
                    lab.labelTMP.color = Color.white;
                    lab.labelTMP.fontSize = 10;
                });
                eloPopupPage.AddElement(parent =>
                {
                    MenuAPI.CreateREPOLabel($"{bracket} ({elo})", parent, new Vector2(400, 220));
                });
                eloPopupPage.AddElement(parent =>
                {
                    var lab = MenuAPI.CreateREPOLabel($"Your rank:", parent, new Vector2(400, 235));
                    lab.labelTMP.color = Color.white;
                    lab.labelTMP.fontSize = 10;
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
