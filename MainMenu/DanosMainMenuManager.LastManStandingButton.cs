using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
using RepoRanked.Mods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager : MonoBehaviour
    {


        public REPOButton GetFTTButton(Transform parent)
        {
            return CreateQueueButton(
                "Dev Queue",
                new Vector2(380, 160),
                response =>
                {
                    ShowFTTPopup();
                    return Task.CompletedTask;
                },
                parent
            );
        }
    }
}
