using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace manhnd_sdk.Editor.PlayerPrefsEditor
{
    public class PlayerPrefsEditor : EditorWindow
    {
        private IPlayerPrefsProvider playerPrefsProvider =
#if UNITY_EDITOR_WIN
                                                            new WindowsPlayerPrefsProvider();
#else 
                                                            null;
#endif
        
        [MenuItem("manhnd_sdk/Player Prefs Editor")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<PlayerPrefsEditor>();
            window.minSize = new Vector2(760f, 300);
            window.titleContent = new GUIContent("Player Prefs Editor");
            window.Show();
        }

        private List<PlayerPrefsPair> PlayerPrefsPairs => playerPrefsProvider.PlayerPrefsPairs;

        private string searchField = "";
        private Vector2 scrollPos = Vector2.zero;
        private Dictionary<string, string> inputPlayerPrefs = new();
        
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
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Color originalColor = GUI.color;
            Color originalBg = GUI.backgroundColor;

            int maxWidth = (int)position.width;
            int keyWidth = (int)(maxWidth * 0.25f);
            int typeWidth = (int)(maxWidth * 0.1f);
            int valueWidth = (int)(maxWidth * 0.4f);
            int actionWidth = (int)(maxWidth * 0.23f / 3f);

            for (int i = 0; i < PlayerPrefsPairs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                PlayerPrefsPair currPair = PlayerPrefsPairs[i];
                
                // Key
                GUILayout.Label(currPair.Key, GUILayout.Width(keyWidth));
                
                // Type
                Type type = currPair.Value.GetType();
                GUIStyle typeStyle = new GUIStyle(GUI.skin.label);
                typeStyle.alignment = TextAnchor.LowerCenter;
                typeStyle.normal.textColor = currPair.TypeColor;
                GUILayout.Label(currPair.AliasType, typeStyle, GUILayout.Width(typeWidth));
                
                // Value
                bool isChanged = inputPlayerPrefs.ContainsKey(currPair.Key);
                string value = isChanged ? inputPlayerPrefs[currPair.Key] : currPair.Value.ToString();

                if (isChanged)
                    GUI.backgroundColor = Color.red;
                
                value = GUILayout.TextArea(value, GUILayout.Width(valueWidth));
                OnValueChanged(currPair, value);
                GUI.backgroundColor = originalBg;
                
                // Action
                GUI.color = Color.green;
                if (GUILayout.Button("Save", GUILayout.Width(actionWidth)))
                {
                    if (inputPlayerPrefs.ContainsKey(currPair.Key))
                    {
                        if(Save(currPair.Key, currPair.Value, inputPlayerPrefs[currPair.Key]))
                            inputPlayerPrefs.Remove(currPair.Key);
                    }
                }
            }

            GUI.color = originalColor;
            GUI.backgroundColor = originalBg;
            EditorGUILayout.EndScrollView();
        }

        private void OnValueChanged(PlayerPrefsPair playerPrefsPair, string value)
        {
            if (value != playerPrefsPair.Value.ToString())
                inputPlayerPrefs[playerPrefsPair.Key] = value;
            else
                inputPlayerPrefs.Remove(playerPrefsPair.Key);
        }

        private bool Save(string ppKey, object oldValue, string ppNewValue)
        {
            switch (oldValue)
            {
                case int:
                    if (int.TryParse(ppNewValue, out int intVal))
                    {
                        PlayerPrefs.SetInt(ppKey, intVal);
                        return true;
                    }
                    break;
                case float:
                    if (float.TryParse(ppNewValue, out float floatVal))
                    {
                        PlayerPrefs.SetFloat(ppKey, floatVal);
                        return true;
                    }
                    break;
                case string:
                    PlayerPrefs.SetString(ppKey, ppNewValue);
                    return true;
            }

            return false;
        }

        private void SaveAll()
        {
            
        }
    }
}