using RepoRanked.MainMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace RepoRanked.LevelControllers
{
    internal class EndStatusUI : MonoBehaviour
    {
        public static EndStatusUI Instance { get; private set; } = null!;

        private TextMeshProUGUI matchEndedText;

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
            Debug.Log("[REPORanked] Creating EndStatusUI");
            if (Instance != null) return;

            var canvasObj = new GameObject("EndStatusCanvas", typeof(Canvas));
            var canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            DontDestroyOnLoad(canvasObj);

            var uiObj = new GameObject("EndStatusUI", typeof(RectTransform));
            uiObj.transform.SetParent(canvas.transform);
            var rect = uiObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1920, 1080);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            var manager = uiObj.AddComponent<EndStatusUI>();
            manager.BuildUI(uiObj.transform);
        }

        private void BuildUI(Transform parent)
        {
            Debug.Log("[REPORanked] Building EndStatusUI");


            var nameObj = new GameObject("Name", typeof(TextMeshProUGUI));
            nameObj.transform.SetParent(parent);
            matchEndedText = nameObj.GetComponent<TextMeshProUGUI>();
            matchEndedText.fontSize = 100;
            matchEndedText.alignment = TextAlignmentOptions.Center;
            matchEndedText.rectTransform.anchoredPosition = new Vector2(0, 0);
            matchEndedText.rectTransform.sizeDelta = new Vector2(1920, 1080);
            matchEndedText.fontStyle = FontStyles.Bold;

            // Set the font to Teko - VariableFont_wght SDF

            if (DanosMainMenuManager.fontAsset != null)
            {
                var font = DanosMainMenuManager.fontAsset;
                if (font != null)
                {
                    matchEndedText.font = font;
                }
            }
            // Set the color to white
            matchEndedText.color = GetColor();
        }

        public void EndMatchAnimation()
        {
            StartCoroutine(IEEndMatchAnimation());
        }

        public IEnumerator IEEndMatchAnimation()
        {
            matchEndedText.fontSize = 200;
            matchEndedText.text = "MATCH ENDED";
            matchEndedText.color = Color.white;
            while (matchEndedText.fontSize > 150.1f)
            {
                yield return new WaitForEndOfFrame();
                matchEndedText.fontSize = matchEndedText.fontSize - (matchEndedText.fontSize - 150) / 20;
                matchEndedText.color = matchEndedText.color - (matchEndedText.color - GetColor()) / 50;
            }
            yield return new WaitForSeconds(3);
            while (matchEndedText.color.a > 0.01f)
            {
                yield return new WaitForEndOfFrame();
                matchEndedText.fontSize = matchEndedText.fontSize - (matchEndedText.fontSize - 100) / 20;
                matchEndedText.color = matchEndedText.color - (matchEndedText.color - new Color(GetColor().r, GetColor().g, GetColor().b, 0) ) / 50;
            }
            matchEndedText.color = new Color(0, 0, 0, 0);
            yield return null;
        }

        public Color GetColor()
        {
            return new Color(1, 0.449203f, 0, 1); // White color
        }

        public void DestroyUI()
        {
            Destroy(gameObject);
            Instance = null;
        }
    }
}
