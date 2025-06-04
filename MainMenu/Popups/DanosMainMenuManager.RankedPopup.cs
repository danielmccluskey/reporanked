using MenuLib.MonoBehaviors;
using MenuLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Steamworks;
using RepoRanked.API;
using RepoRankedApiResponseModel;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager
    {
        public REPOPopupPage? eloPopupPage;
        private void ShowRankedPopup(int elo, string bracket, int matches)
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
                            DanosMatchQueuePoller.Create(QueueTypes.ranked);
                            DanosMatchQueuePoller.Instance.StartPolling(eloPopupPage, RepoRankedApiResponseModel.QueueTypes.ranked);
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
                    MenuAPI.CreateREPOButton("Close", () => eloPopupPage.ClosePage(true), parent, new Vector2(400, 140));
                });







            }

            eloPopupPage.OpenPage(false);
        }
    }
}
