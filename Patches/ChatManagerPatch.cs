using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ChatManager;
using static UnityEngine.InputSystem.InputRemoting;

namespace RepoRanked.Patches
{
    [HarmonyPatch]
    class ChatManagerPatch
    {
        [HarmonyPatch(typeof(ChatManager), "Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix(ChatManager __instance)
        {
            instance.chatActive = true;
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
            ImportantFetches(__instance);
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


        private static void ImportantFetches(ChatManager __instance)
        {
            if (!__instance.chatText)
            {
                __instance.textMeshFetched = false;
            }
            if (!__instance.playerAvatar)
            {
                __instance.localPlayerAvatarFetched = false;
            }
            if (!__instance.textMeshFetched && (bool)ChatUI.instance && (bool)ChatUI.instance.chatText)
            {
                __instance.chatText = ChatUI.instance.chatText;
                __instance.textMeshFetched = true;
            }
            if (__instance.localPlayerAvatarFetched)
            {
                return;
            }
            
                List<PlayerAvatar> list = SemiFunc.PlayerGetList();
                if (list.Count <= 0)
                {
                    return;
                }
                {
                    foreach (PlayerAvatar item in list)
                    {
                        if (item.isLocal)
                        {
                            __instance.playerAvatar = item;
                            __instance.localPlayerAvatarFetched = true;
                            break;
                        }
                    }
                    return;
                }
            
            __instance.playerAvatar = PlayerAvatar.instance;
            __instance.localPlayerAvatarFetched = true;
        }



    }
}

