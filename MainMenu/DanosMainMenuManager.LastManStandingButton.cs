using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
using RepoRanked.Mods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager : MonoBehaviour
    {


        public REPOButton GetLMSButton(Transform parent)
        {
            var rankedButton = MenuAPI.CreateREPOButton(
                    "More coming soon!",
                    async () =>
                    {
                       

                        
                    },
                    parent,
                    localPosition: new Vector2(380, 160)
                );


            rankedButton.GetComponent<MenuButton>().enabled = false;
            rankedButton.GetComponent<MenuSelectableElement>().enabled = false;
            rankedButton.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(1, 1, 1, 0.1f);

            return rankedButton;

        }
    }
}
