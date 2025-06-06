﻿using MenuLib;
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


        public REPOButton GetPassphraseButton(Transform parent)
        {
            var discordButton = MenuAPI.CreateREPOButton(
                "Set Unranked Passphrase",
                () =>
                {
                    if (isProcessing) return;
                    isProcessing = true;
                    ShowPassphrasePopup();
                    isProcessing = false;
                },
                parent,
                localPosition: new Vector2(400, 140)
            );

            return discordButton;

        }
    }
}
