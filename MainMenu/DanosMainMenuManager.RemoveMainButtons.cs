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
                        Destroy(child.gameObject);
                        break;
                }
            }

        }
    }
}
