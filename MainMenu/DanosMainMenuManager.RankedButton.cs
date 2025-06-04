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


        public REPOButton GetRankedButton(Transform parent)
        {
            return CreateQueueButton(
                "Ranked",
                new Vector2(380, 240),
                response =>
                {
                    Debug.Log($"Registered with Elo {response.EloRating}");
                    ShowRankedPopup(response.EloRating, response.Message, response.Matches);
                    return Task.CompletedTask;
                },
                parent
            );
        }
    }
}
