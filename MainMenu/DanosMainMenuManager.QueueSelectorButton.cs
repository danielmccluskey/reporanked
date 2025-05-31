using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
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


        public REPOButton GetQueueSelectorButton(Transform parent)
        {
            var discordButton = MenuAPI.CreateREPOButton(
                "REPORanked",
                () =>
                {
                    ShowQueueSelectorPopup();
                },
                parent,
                localPosition: hostPosition
            );

            return discordButton;

        }
    }
}
