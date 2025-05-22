using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.Mods
{
    using BepInEx;
    using global::RepoRanked.LevelControllers;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class DanosModChecker : MonoBehaviour
    {
        public static DanosModChecker Instance { get; private set; } = null!;

        public List<string> AllowedModGuids = new()
    {
        "danos.RepoRanked",
        "nickklmao.menulib",
    };

        public float CheckIntervalSeconds = 10f;

        public bool IsCompliant { get; private set; } = true;
        public bool IsGameVersionCompliant { get; private set; } = true;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Duplicate DanosModChecker detected. Destroying new instance.");
                Destroy(gameObject);
                return;
            }

            Debug.Log("DanosModChecker initialized.");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartCoroutine(ModCheckCoroutine());
        }

        private IEnumerator ModCheckCoroutine()
        {
            while (true)
            {
                RunCheck();
                yield return new WaitForSeconds(CheckIntervalSeconds);
            }
        }

        private void RunCheck()
        {
            var loadedMods = BepInEx.Bootstrap.Chainloader.PluginInfos.Values;

            bool compliant = true;

            foreach (var mod in loadedMods)
            {
                if (!AllowedModGuids.Contains(mod.Metadata.GUID))
                {
                    compliant = false;
                }
            }

            IsCompliant = compliant;

            if (!compliant)
            {
                if (RankedGameManager.Instance != null)
                {
                    if (RankedGameManager.Instance.matchData != null)
                    {
                        RankedGameManager.Instance.CompleteMatch("non_allowed_mods");
                    }
                }
            }

            RunGameVersionCheck();




        }

        private void RunGameVersionCheck()
        {
            Version currentVersion = null;

            BuildManager inst = BuildManager.instance;
            if (inst != null)
            {
                currentVersion = inst.version;
                if (currentVersion != null)
                {

                    if (currentVersion.title.ToLower().Contains("beta"))
                    {
                        IsGameVersionCompliant = false;
                    }
                    else
                    {
                        IsGameVersionCompliant = true;

                    }


                }
            }


        }






        public void ForceCheck()
        {
            RunCheck();
        }

        public static void Create()
        {
            if (Instance != null)
                return;

            var obj = new GameObject("DanosModChecker");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<DanosModChecker>();
        }
    }

}
