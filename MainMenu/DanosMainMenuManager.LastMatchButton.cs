using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
using RepoRanked.LevelControllers;
using RepoRanked.Mods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager : MonoBehaviour
    {


        public REPOButton GetLastMatchButton(Transform parent)
        {
            var lastMatchButton = MenuAPI.CreateREPOButton(
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
            return lastMatchButton;

        }
    }
}
