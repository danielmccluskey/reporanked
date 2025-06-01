using MenuLib.MonoBehaviors;
using MenuLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Steamworks;
using RepoRanked.API;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager
    {
        public REPOPopupPage? unRankedPopupPage;

        private void ShowUnRankedPopup()
        {
            if (unRankedPopupPage == null)
            {
                queueSelectorPopup?.ClosePage(true);


                unRankedPopupPage = MenuAPI.CreateREPOPopupPage(
                    "REPORanked",
                    REPOPopupPage.PresetSide.Right,
                    shouldCachePage: true,
                    pageDimmerVisibility: true,
                    spacing: 1.5f
                );
                unRankedPopupPage.AddElement(parent =>
                {
                    var lab = MenuAPI.CreateREPOLabel($"Play against your friends!\nMake sure to set a Passphrase in the main menu!", parent, new Vector2(400, 260));
                    lab.labelTMP.color = Color.white;
                    lab.labelTMP.fontSize = 10;
                });
                unRankedPopupPage.AddElement(parent =>
                {

                MenuAPI.CreateREPOButton("Queue with Password (Unranked)", async () =>
                {
                    if (isProcessing)
                    {
                        return;
                    }
                    isProcessing = true;
                    var steamId = (long)SteamClient.SteamId.Value;
                    var success = await DanosAPI.Enqueue(steamId, RepoRankedApiResponseModel.QueueTypes.unranked, DanosPassphraseManager.GetPassphrase());
                    if (success)
                    {
                        DanosMatchQueuePoller.Create();
                        DanosMatchQueuePoller.Instance.StartPolling(unRankedPopupPage);
                    }
                    else
                    {
                        Debug.LogError("Failed to queue for match.");
                    }

                    isProcessing = false;
                }, parent,  new Vector2(400, 180));


                });
                unRankedPopupPage.AddElement(parent =>
                {
                    var passbutton = GetPassphraseButton(parent);
                });
                unRankedPopupPage.AddElement(parent =>
            {
                MenuAPI.CreateREPOButton("Close", () => { unRankedPopupPage.ClosePage(true); Destroy(unRankedPopupPage); }, parent, new Vector2(400, 100));
            });







        }

            unRankedPopupPage.OpenPage(false);
        }
}
}
