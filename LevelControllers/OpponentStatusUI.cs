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

namespace RepoRanked.LevelControllers
{
    public class OpponentStatusUI : MonoBehaviour
    {
        public static OpponentStatusUI Instance { get; private set; } = null!;

        private TextMeshProUGUI nameText;
        private TextMeshProUGUI progressText;

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
            Debug.Log("[REPORanked] Creating OpponentStatusUI");
            if (Instance != null) return;

            var canvasObj = new GameObject("OpponentStatusCanvas", typeof(Canvas));
            var canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            DontDestroyOnLoad(canvasObj);

            var uiObj = new GameObject("OpponentStatusUI", typeof(RectTransform));
            uiObj.transform.SetParent(canvas.transform);
            var rect = uiObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 80);
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -20);

            var manager = uiObj.AddComponent<OpponentStatusUI>();
            manager.BuildUI(uiObj.transform);
        }

        private void BuildUI(Transform parent)
        {
            Debug.Log("[REPORanked] Building OpponentStatusUI");
            

            var nameObj = new GameObject("Name", typeof(TextMeshProUGUI));
            nameObj.transform.SetParent(parent);
            nameText = nameObj.GetComponent<TextMeshProUGUI>();
            nameText.fontSize = 22;
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.rectTransform.anchoredPosition = new Vector2(-50, 0);
            nameText.rectTransform.sizeDelta = new Vector2(200, 30);          
            


            var progressObj = new GameObject("Progress", typeof(TextMeshProUGUI));
            progressObj.transform.SetParent(parent);
            progressText = progressObj.GetComponent<TextMeshProUGUI>();
            progressText.fontSize = 20;
            progressText.alignment = TextAlignmentOptions.Left;
            progressText.rectTransform.anchoredPosition = new Vector2(-50, -30);
            progressText.rectTransform.sizeDelta = new Vector2(200, 30);



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
        public void SetOpponentName(string name)
        {
            if (nameText != null)
                nameText.text = name;
        }

        

        public void SetOpponentProgress(int progress, int extractions)
        {
            if (progressText != null)
            {
                progressText.text = GetText(progress, extractions);
            }
        }

        public void DestroyUI()
        {
            Destroy(gameObject);
            Instance = null;
        }


        public string GetText(int progress, int extractions)
        {
            if(progress == 0 && extractions == 0)
            {
                return "Connecting...";
            }

            if (progress >=extractions)
            {
                return "Heading back to truck.";
            }

            return $"{progress}/{extractions}";
        }
    }
}
