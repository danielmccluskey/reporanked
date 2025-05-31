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


        public REPOButton GetRankedButton(Transform parent)
        {
            var rankedButton = MenuAPI.CreateREPOButton(
                    "Ranked",
                    async () =>
                    {
                        if (isProcessing)
                        {
                            return;
                        }
                        isProcessing = true;

                        queueSelectorPopup?.ClosePage(true);


                        Debug.Log("REPORanked clicked!");
                        DanosModChecker.Create();
                        DanosModChecker.Instance.ForceCheck();


                        if (DanosModChecker.Instance != null && !DanosModChecker.Instance.IsCompliant)
                        {
                            ShowErrorPopup("You are using disallowed mods. \nWe do this to keep competitive integrity.\n\nAllowed mods are:\n-REPORanked\n-MenuLib\n\n\nPlease remove any others to play :).");
                            isProcessing = false;
                            return;
                        }
                        if (DanosModChecker.Instance != null && !DanosModChecker.Instance.IsGameVersionCompliant)
                        {
                            ShowErrorPopup("Your game version is not compliant with REPORanked.\n\nPlease update your game to the latest version.\n\nIf you are using the beta branch, please revert to the normal one.");
                            isProcessing = false;
                            return;
                        }

                        ShowPrivacyConsentPopup(async () =>
                        {
                            var response = await DanosAPI.RegisterPlayer((long)SteamClient.SteamId.Value, SteamClient.Name);
                            if (response != null)
                            {
                                Debug.Log($"Registered with Elo {response.EloRating}");
                                if (response.Status == "ok")
                                {
                                    ShowRankedPopup(response.EloRating, response.Message, response.Matches);
                                }
                                else if (response.Status == "error")
                                {
                                    ShowErrorPopup(response.Message);
                                }
                                else
                                {
                                    Debug.LogError("Unknown response status.");
                                }
                            }
                            else
                            {
                                Debug.LogError("Failed to register player.");
                            }

                            isProcessing = false;
                        });



                        isProcessing = false;
                    },
                    parent,
                    localPosition: new Vector2(380, 240)
                );

            return rankedButton;

        }
    }
}
