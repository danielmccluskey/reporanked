using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ChatManager;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    class ChatManagerPatch
    {
        [HarmonyPatch(typeof(ChatManager), "Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix(ChatManager __instance)
        {

            instance.PossessionActive();
            if ((bool)instance.playerAvatar && instance.playerAvatar.isDisabled && (instance.possessBatchQueue.Count > 0 || instance.currentBatch != null))
            {
                instance.InterruptCurrentPossessBatch();
            }
            if (!LevelGenerator.Instance.Generated)
            {
                instance.NewLevelResets();
                return false;
            }
            instance.ImportantFetches();
            instance.PossessChatCustomLogic();
            if (!instance.textMeshFetched || !instance.localPlayerAvatarFetched)
            {
                return false;
            }
            switch (instance.chatState)
            {
                case ChatState.Inactive:
                    instance.StateInactive();
                    break;
                case ChatState.Active:
                    instance.StateActive();
                    break;
                case ChatState.Possessed:
                    instance.StatePossessed();
                    break;
                case ChatState.Send:
                    instance.StateSend();
                    break;
            }
            instance.PossessChatCustomLogic();
            if (!SemiFunc.IsMultiplayer())
            {
                if (instance.chatState != 0)
                {
                    instance.StateSet(ChatState.Inactive);
                }
                instance.chatActive = false;
                return false;
            }
            if (instance.spamTimer > 0f)
            {
                instance.spamTimer -= Time.deltaTime;
            }
            if (SemiFunc.FPSImpulse15() && instance.betrayalActive && PlayerController.instance.playerAvatarScript.RoomVolumeCheck.inTruck)
            {
                instance.PossessCancelSelfDestruction();
            }










            // Prevent the original method from executing
            return false;
        }






    }
}

