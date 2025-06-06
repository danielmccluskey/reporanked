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
        public REPOPopupPage? fttPopupPage;

        private void ShowFTTPopup()
        {
            if (fttPopupPage == null)
            {
                queueSelectorPopup?.ClosePage(true);


                fttPopupPage = MenuAPI.CreateREPOPopupPage(
                    "REPORanked",
                    REPOPopupPage.PresetSide.Right,
                    shouldCachePage: true,
                    pageDimmerVisibility: true,
                    spacing: 1.5f
                );
                fttPopupPage.AddElement(parent =>
                {
                    var lab = MenuAPI.CreateREPOLabel($"10 Level Set Seed Comp\nCheck the Discord for more info :D", parent, new Vector2(400, 260));
                    lab.labelTMP.color = Color.white;
                    lab.labelTMP.fontSize = 10;
                });
                fttPopupPage.AddElement(parent =>
                {

                MenuAPI.CreateREPOButton("Queue", async () =>
                {
                    if (isProcessing)
                    {
                        return;
                    }
                    isProcessing = true;
                    var steamId = (long)SteamClient.SteamId.Value;
                    var success = await DanosAPI.Enqueue(steamId, RepoRankedApiResponseModel.QueueTypes.monthlychallenge);
                    if (success)
                    {
                        DanosMatchQueuePoller.Create(QueueTypes.monthlychallenge);
                        DanosMatchQueuePoller.Instance.StartPolling(fttPopupPage, RepoRankedApiResponseModel.QueueTypes.monthlychallenge);
                    }
                    else
                    {
                        Debug.LogError("Failed to queue for match.");
                    }

                    isProcessing = false;
                }, parent,  new Vector2(400, 180));


                });
                fttPopupPage.AddElement(parent =>
            {
                MenuAPI.CreateREPOButton("Close", () => fttPopupPage.ClosePage(true), parent, new Vector2(400, 100));
            });







        }

            fttPopupPage.OpenPage(false);
        }
}
}
