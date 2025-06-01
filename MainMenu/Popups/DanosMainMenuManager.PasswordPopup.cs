using MenuLib.MonoBehaviors;
using MenuLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace RepoRanked.MainMenu
{
    public partial class DanosMainMenuManager
    {
        private REPOPopupPage? passphrasePopup;

        public void ShowPassphrasePopup()
        {
            if (passphrasePopup == null)
            {
                queueSelectorPopup?.ClosePage(true);
                unRankedPopupPage?.ClosePage(true);
                Destroy(unRankedPopupPage);
                passphrasePopup = MenuAPI.CreateREPOPopupPage(
                    "Set Passphrase",
                    REPOPopupPage.PresetSide.Right,
                    shouldCachePage: false,
                    pageDimmerVisibility: true,
                    spacing: 1.5f
                );

                passphrasePopup.AddElement(parent =>
                {
                    var lab = MenuAPI.CreateREPOLabel($"Set a passphrase for the unranked queue here!\nMax 7 characters, everything else is truncated\nOnly letters and numbers.", parent, new Vector2(400, 260));
                    lab.labelTMP.color = Color.white;
                    lab.labelTMP.fontSize = 10;
                });

                

                passphrasePopup.AddElement(parent =>
                {

                    var passphrase = DanosPassphraseManager.GetPassphrase();

                    REPOInputField? field = null;
                    field = MenuAPI.CreateREPOInputField(
                        labelText: "Passphrase",
                        onValueChanged: value => { DanosPassphraseManager.SetPassphrase(value); if (value.Length > 7) field?.inputStringSystem.SetValue(value.Substring(0, 7), true); },
                        parent: parent,
                        localPosition: new Vector2(400, 200),
                        onlyNotifyOnSubmit: false,
                        placeholder: "Enter passphrase...",
                        defaultValue: passphrase

                    );
                });

                passphrasePopup.AddElement(parent =>
                {
                    MenuAPI.CreateREPOButton("Close", () => { passphrasePopup.ClosePage(true); Destroy(passphrasePopup); ShowUnRankedPopup(); }, parent, new Vector2(400, 150));
                });
            }

            passphrasePopup.OpenPage(false);
        }
    }
}
