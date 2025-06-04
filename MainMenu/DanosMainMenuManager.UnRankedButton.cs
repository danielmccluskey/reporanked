using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
using RepoRanked.Mods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager : MonoBehaviour
    {


        public REPOButton GetUnRankedButton(Transform parent)
        {
            return CreateQueueButton(
                "Un-Ranked",
                new Vector2(380, 200),
                response =>
                {
                    ShowUnRankedPopup();
                    return Task.CompletedTask;
                },
                parent
            );
        }
    }
}
