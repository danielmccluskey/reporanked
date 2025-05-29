using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MenuLib;
using RepoRanked.API;
using RepoRanked.MainMenu;
using RepoRanked.Mods;
using Steamworks;
using UnityEngine;

namespace RepoRanked;

[BepInPlugin("danos.RepoRanked", "RepoRanked", "0.9.8")]
[BepInDependency("nickklmao.menulib", BepInDependency.DependencyFlags.HardDependency)]

public class RepoRanked : BaseUnityPlugin
{
    internal static RepoRanked Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    private void Awake()
    {
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();

        DanosMainMenuManager.Create();
        DanosMainMenuManager.Instance.Init();


        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void Update()
    {
        // Code that runs every frame goes here
    }
}