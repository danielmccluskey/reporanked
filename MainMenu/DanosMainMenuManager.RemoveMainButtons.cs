using MenuLib;
using MenuLib.MonoBehaviors;
using RepoRanked.API;
using RepoRanked.LevelControllers;
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


        public void RemoveMainButtons(Transform parent)
        {
            foreach (Transform child in parent)
            {

                switch (child.name)
                {
                    case "Menu Button - host game":
                        hostPosition = child.localPosition;
                        Destroy(child.gameObject);
                        break;

                    case "Menu Button - join game":
                        joinPosition = child.localPosition;
                        Destroy(child.gameObject);
                        break;

                    case "Menu Button - singleplayer":
                    case "Menu Button - Tutorial":
                        // Since destroying these buttons only makes the player think that the unrankeds buttons are buggy or not where they need to be, it's better if you can see that they are disabled
                        child.GetComponent<MenuButton>().enabled = false;
                        child.GetComponent<MenuSelectableElement>().enabled = false;
                        child.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(1, 1, 1, 0.1f);
                        if (child.name == "Menu Button - singleplayer" && RankedGameManager.LastMatchId != -1)
                            Destroy(child.gameObject);
                        break;
                }
            }

        }
    }
}
