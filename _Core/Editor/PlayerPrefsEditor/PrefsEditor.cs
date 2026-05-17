using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace manhnd_sdk.Editor.PlayerPrefsEditor
{
    public class PrefsEditor : EditorWindow
    {
        private IPlayerPrefsProvider playerPrefsProvider =
#if UNITY_EDITOR_WIN
                                                            new WindowsPlayerPrefsProvider();
#else 
                                                            null;
#endif
        
        [MenuItem("mannd_sdk/Player Editor Prefs")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<PrefsEditor>();
            window.minSize = new Vector2(760f, 300);
            window.titleContent = new GUIContent("Player Prefs Editor");
            window.Show();
        }

        private List<PlayerPrefsPair> playerPrefs => playerPrefsProvider.PlayerPrefsPairs;

        private string searchField = "";
        private Vector2 scrollPos = Vector2.zero;
        
        private void OnGUI()
        {
            DrawQuickButtons();
            DrawPlayerPrefs();
        }

        private void DrawQuickButtons()
        {
            Color originalColor = GUI.color;
            GUILayout.BeginHorizontal();

            // GUI.color = Color.green;
            // if (GUILayout.Button("Save All"))
            //     SaveAll();
            
            GUILayout.EndHorizontal();
            GUI.color = originalColor;
        }
        
        private void DrawPlayerPrefs()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            Color originalColor = GUI.color;
            Color originalBg = GUI.backgroundColor;

            int maxWidth = (int)position.width;
            int keyWidth = (int)(maxWidth * 0.25f);
            int typeWidth = (int)(maxWidth * 0.1f);
            int valueWidth = (int)(maxWidth * 0.4f);
            int actionWidth = (int)(maxWidth * 0.23f / 3f);

            for (int i = 0; i < playerPrefs.Count; i++)
            {
                
            }

            GUI.color = originalColor;
            GUI.backgroundColor = originalBg;
            GUILayout.EndScrollView();
        }
    }
}