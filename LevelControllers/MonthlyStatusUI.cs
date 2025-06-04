using RepoRanked.API;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using MenuLib;
using SingularityGroup.HotReload;
using RepoRanked.MainMenu;
using Newtonsoft.Json;

namespace RepoRanked.LevelControllers
{
    public class MonthlyStatusUI : MonoBehaviour
    {
        public static MonthlyStatusUI Instance { get; private set; } = null!;

        private TextMeshProUGUI nameText;
        private TextMeshProUGUI progressText;

        private string LastProgressText = ""; // This is used to detect when progressText has changed to trigger the animation

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void CreateUI()
        {
            Debug.Log("[REPORanked] Creating MonthlyStatusUI");
            if (Instance != null) return;

            var canvasObj = new GameObject("MonthlyStatusCanvas", typeof(Canvas));
            var canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            DontDestroyOnLoad(canvasObj);

            var uiObj = new GameObject("MonthlyStatusUI", typeof(RectTransform));
            uiObj.transform.SetParent(canvas.transform);
            var rect = uiObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 80);
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(0, -20);

            var manager = uiObj.AddComponent<MonthlyStatusUI>();
            manager.BuildUI(uiObj.transform);
        }

        private void BuildUI(Transform parent)
        {
            Debug.Log("[REPORanked] Building MonthlyStatusUI");
            

            var nameObj = new GameObject("Name", typeof(TextMeshProUGUI));
            nameObj.transform.SetParent(parent);
            nameText = nameObj.GetComponent<TextMeshProUGUI>();
            nameText.fontSize = 15;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.rectTransform.anchoredPosition = new Vector2(0, 0);
            nameText.rectTransform.sizeDelta = new Vector2(500, 30);          
            


            var progressObj = new GameObject("Progress", typeof(TextMeshProUGUI));
            progressObj.transform.SetParent(parent);
            progressText = progressObj.GetComponent<TextMeshProUGUI>();
            progressText.fontSize = 15;
            progressText.alignment = TextAlignmentOptions.Center;
            progressText.rectTransform.anchoredPosition = new Vector2(0, -50);
            progressText.rectTransform.sizeDelta = new Vector2(500, 30);



            // Set the font to Teko - VariableFont_wght SDF

            if (DanosMainMenuManager.fontAsset != null)
            {
                var font = DanosMainMenuManager.fontAsset;
                if (font != null)
                {
                    nameText.font = font;
                    progressText.font = font;
                }
            }
            // Set the color to white
            nameText.color = GetColor();
            progressText.color = GetColor();
        }

        public Color GetColor()
        {
            return new Color(1, 0.449203f, 0, 1); // White color
        }



        // Called by RankedGameManager
        public void SetEnemyHash(Dictionary<string,int> enemyList)
        {
            if (nameText != null)
            {
                //Serialize it as json, then hash the string to a max 10 character hash
                var json = JsonConvert.SerializeObject(enemyList, Formatting.None);

                var hash = json.GetHashCode().ToString("X");

                if (hash.Length > 10)
                {
                    hash = hash.Substring(0, 10);
                }


                nameText.text = $"hash: {hash}";

            }
        }

        

        public void SetSeed(int seed)
        {
            if (progressText != null)
            {
                progressText.text = $"seed: {seed}";
            }
        }

        public void DestroyUI()
        {
            Destroy(gameObject);
            Instance = null;
        }


        

        
    }
}
