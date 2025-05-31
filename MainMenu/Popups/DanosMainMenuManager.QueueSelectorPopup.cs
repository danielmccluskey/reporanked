using MenuLib.MonoBehaviors;
using MenuLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager
    {
        private REPOPopupPage? queueSelectorPopup;

        public void ShowQueueSelectorPopup()
        {
            if (queueSelectorPopup == null)
            {
                queueSelectorPopup = MenuAPI.CreateREPOPopupPage(
                    "Available Queues",
                    REPOPopupPage.PresetSide.Right,
                    shouldCachePage: false,
                    pageDimmerVisibility: true,
                    spacing: 1.5f
                );

                queueSelectorPopup.AddElement(parent =>
                {
                    var lab = MenuAPI.CreateREPOLabel($"Select a queue to join:", parent, new Vector2(400, 260));
                    lab.labelTMP.color = Color.white;
                    lab.labelTMP.fontSize = 10;
                });

                queueSelectorPopup.AddElement(parent =>
                {
                    var rankedButton = GetRankedButton(parent);

                });

                queueSelectorPopup.AddElement(parent =>
                {
                    var unrankedButton = GetUnRankedButton(parent);
                });

                queueSelectorPopup.AddElement(parent =>
                {
                    var unrankedButton = GetLMSButton(parent);
                });






            }

            queueSelectorPopup?.OpenPage(false);
        }
    }
}
