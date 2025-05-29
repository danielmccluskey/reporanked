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


        public REPOButton GetDiscordButton(Transform parent)
        {
            var discordButton = MenuAPI.CreateREPOButton(
                "Join Discord",
                () =>
                {
                    Application.OpenURL(DiscordInviteUrl);
                },
                parent,
                localPosition: hostPosition + new Vector2(0, -40) // place below the main button
            );

            return discordButton;

        }
    }
}
