using System;
using System.Collections.Generic;
using manhnd_sdk.EditorTool.Common;
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

        private string searchField = "";
        private Vector2 scrollPos;
        private Dictionary<string, string> inputPlayerPrefs = new();
        private HashSet<string> savedKey = new();
        private List<PlayerPrefsPair> currentPairs;
        private int visibleCount;

        private GUIStyle typeStyle;
        private float cachedWidth;
        private GUILayoutOption[] keyWidthOpt, typeWidthOpt, valueWidthOpt, actionWidthOpt;

        private static readonly GUILayoutOption[] searchLabelWidth = { GUILayout.Width(50) };
        private static readonly GUILayoutOption[] clearSearchWidth = { GUILayout.Width(20) };

        private static readonly Color RowEvenColor = new(0f, 0f, 0f, 0.06f);
        private static readonly Color RowOddColor = new(0f, 0f, 0f, 0.14f);
        private static readonly Color ModifiedBgColor = new(1f, 0.6f, 0.2f, 0.5f);

        private string[] cachedValueStrs = Array.Empty<string>();

        private readonly GUIContent statusContent = new();
        private int lastStatusTotal = -1, lastStatusVisible = -1, lastStatusModified = -1;
        private bool lastStatusHasSearch;

        [MenuItem("manhnd_sdk/Player Prefs Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<PlayerPrefsEditor>();
            window.minSize = new Vector2(760f, 300);
            window.titleContent = new GUIContent("Player Prefs Editor");
            window.Show();
        }

        private void OnGUI()
        {
            StaticStyles.Ensure();
            EnsureLocalStyles();
            CacheLayoutOptions();

            if (playerPrefsProvider == null)
            {
                EditorGUILayout.HelpBox("PlayerPrefs Editor is only supported on Windows.", MessageType.Warning);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                currentPairs = playerPrefsProvider.PlayerPrefsPairs;
                CacheValueStrings();
                PurgeStaleDirtyEntries();
            }

            if (currentPairs == null) return;

            GUILayout.Label("Player Prefs Editor", StaticStyles.SectionTitle);
            DrawQuickButtons();
            DrawSearchField();
            DrawColumnHeaders();
            DrawPlayerPrefs();
            DrawStatusBar();
        }

        private void EnsureLocalStyles()
        {
            if (typeStyle != null) return;
            typeStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };
        }

        private void CacheLayoutOptions()
        {
            float w = position.width;
            if (Math.Abs(w - cachedWidth) < 1f) return;
            cachedWidth = w;

            int max = (int)w;
            keyWidthOpt = new[] { GUILayout.Width((int)(max * 0.25f)) };
            typeWidthOpt = new[] { GUILayout.Width((int)(max * 0.1f)) };
            valueWidthOpt = new[] { GUILayout.Width((int)(max * 0.4f)) };
            actionWidthOpt = new[] { GUILayout.Width((int)(max * 0.23f / 3f)) };
        }

        private void CacheValueStrings()
        {
            int count = currentPairs.Count;
            if (cachedValueStrs.Length < count)
                cachedValueStrs = new string[count];
            for (int i = 0; i < count; i++)
            {
                object v = currentPairs[i].Value;
                cachedValueStrs[i] = v is string s ? s : v.ToString();
            }
        }

        private void DrawQuickButtons()
        {
            Color orig = GUI.color;
            bool hasModified = inputPlayerPrefs.Count > 0;
            bool hasEntries = currentPairs.Count > 0;

            GUILayout.BeginHorizontal();

            GUI.enabled = hasModified;
            GUI.color = Color.green;
            if (GUILayout.Button("Save All"))
                SaveAll();

            GUI.color = Color.magenta;
            if (GUILayout.Button("Revert All"))
                RevertAll();

            GUI.enabled = hasEntries;
            GUI.color = StaticColor.DangerColor;
            if (GUILayout.Button("Delete All"))
            {
                string message = searchField.Length > 0
                    ? $"This will delete ALL {currentPairs.Count} entries, not just the filtered results.\nThis cannot be undone."
                    : $"Delete all {currentPairs.Count} entries? This cannot be undone.";

                if (EditorUtility.DisplayDialog("Delete All PlayerPrefs", message, "Delete All", "Cancel"))
                    DeleteAll();
            }

            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUI.color = orig;
        }

        private void DrawSearchField()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Search", searchLabelWidth);
            searchField = GUILayout.TextField(searchField);
            if (searchField.Length > 0 && GUILayout.Button("X", clearSearchWidth))
                searchField = "";
            GUILayout.EndHorizontal();
        }

        private void DrawColumnHeaders()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Key", EditorStyles.boldLabel, keyWidthOpt);
            GUILayout.Label("Type", EditorStyles.boldLabel, typeWidthOpt);
            GUILayout.Label("Value", EditorStyles.boldLabel, valueWidthOpt);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPlayerPrefs()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Color origColor = GUI.color;
            Color origBg = GUI.backgroundColor;
            visibleCount = 0;
            bool hasSearch = searchField.Length > 0;

            for (int i = 0; i < currentPairs.Count; i++)
            {
                PlayerPrefsPair pair = currentPairs[i];
                string valueStr = cachedValueStrs[i];

                if (hasSearch)
                {
                    bool match = pair.Key.IndexOf(searchField, StringComparison.OrdinalIgnoreCase) >= 0
                                 || valueStr.IndexOf(searchField, StringComparison.OrdinalIgnoreCase) >= 0
                                 || pair.AliasType.IndexOf(searchField, StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!match) continue;
                }

                Rect row = EditorGUILayout.BeginHorizontal();
                EditorGUI.DrawRect(row, visibleCount % 2 == 0 ? RowEvenColor : RowOddColor);
                visibleCount++;

                GUILayout.Label(pair.Key, keyWidthOpt);

                typeStyle.normal.textColor = pair.TypeColor;
                GUILayout.Label(pair.AliasType, typeStyle, typeWidthOpt);

                bool isChanged = inputPlayerPrefs.ContainsKey(pair.Key);
                string displayValue = isChanged ? inputPlayerPrefs[pair.Key] : valueStr;
                if (isChanged)
                    GUI.backgroundColor = ModifiedBgColor;
                string editedValue = GUILayout.TextArea(displayValue, valueWidthOpt);
                OnValueChanged(pair.Key, editedValue, valueStr);
                GUI.backgroundColor = origBg;

                GUI.enabled = isChanged;

                GUI.color = Color.green;
                if (GUILayout.Button("Save", actionWidthOpt)
                    && inputPlayerPrefs.TryGetValue(pair.Key, out string dirtyValue))
                {
                    if (Save(pair.Key, pair.Value, dirtyValue))
                    {
                        inputPlayerPrefs.Remove(pair.Key);
                        PlayerPrefs.Save();
                    }
                }

                GUI.color = Color.magenta;
                if (GUILayout.Button("Revert", actionWidthOpt))
                    inputPlayerPrefs.Remove(pair.Key);

                GUI.enabled = true;

                GUI.color = StaticColor.DangerColor;
                if (GUILayout.Button("Delete", actionWidthOpt))
                {
                    inputPlayerPrefs.Remove(pair.Key);
                    PlayerPrefs.DeleteKey(pair.Key);
                    PlayerPrefs.Save();
                }

                GUI.color = origColor;
                EditorGUILayout.EndHorizontal();
            }

            if (visibleCount == 0)
            {
                string hint = currentPairs.Count == 0
                    ? "No PlayerPrefs found for this project."
                    : "No entries match the search filter.";
                EditorGUILayout.HelpBox(hint, MessageType.Info);
            }

            GUI.color = origColor;
            GUI.backgroundColor = origBg;
            EditorGUILayout.EndScrollView();
        }

        private void DrawStatusBar()
        {
            int total = currentPairs.Count;
            int modified = inputPlayerPrefs.Count;
            bool hasSearch = searchField.Length > 0;

            if (total != lastStatusTotal || visibleCount != lastStatusVisible
                || modified != lastStatusModified || hasSearch != lastStatusHasSearch)
            {
                lastStatusTotal = total;
                lastStatusVisible = visibleCount;
                lastStatusModified = modified;
                lastStatusHasSearch = hasSearch;

                statusContent.text = hasSearch
                    ? $"{visibleCount} / {total} entries"
                    : $"{total} entries";
                if (modified > 0)
                    statusContent.text += $"  |  {modified} modified";
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(statusContent, EditorStyles.miniLabel);
            GUILayout.EndHorizontal();
        }

        private void OnValueChanged(string key, string editedValue, string originalValueStr)
        {
            if (editedValue != originalValueStr)
                inputPlayerPrefs[key] = editedValue;
            else
                inputPlayerPrefs.Remove(key);
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
                    Debug.LogWarning($"[PlayerPrefsEditor] Cannot parse \"{ppNewValue}\" as int for key \"{ppKey}\"");
                    break;
                case float:
                    if (float.TryParse(ppNewValue, out float floatVal))
                    {
                        PlayerPrefs.SetFloat(ppKey, floatVal);
                        return true;
                    }
                    Debug.LogWarning($"[PlayerPrefsEditor] Cannot parse \"{ppNewValue}\" as float for key \"{ppKey}\"");
                    break;
                case string:
                    PlayerPrefs.SetString(ppKey, ppNewValue);
                    return true;
            }
            return false;
        }

        private void SaveAll()
        {
            foreach (var input in inputPlayerPrefs)
            {
                int index = FindPairIndex(input.Key);
                if (index >= 0 && Save(input.Key, currentPairs[index].Value, input.Value))
                    savedKey.Add(input.Key);
            }

            if (savedKey.Count > 0)
            {
                foreach (var key in savedKey)
                    inputPlayerPrefs.Remove(key);
                savedKey.Clear();
                PlayerPrefs.Save();
            }
        }

        private void RevertAll() => inputPlayerPrefs.Clear();

        private void DeleteAll()
        {
            for (int i = 0; i < currentPairs.Count; i++)
                PlayerPrefs.DeleteKey(currentPairs[i].Key);
            inputPlayerPrefs.Clear();
            PlayerPrefs.Save();
        }

        private int FindPairIndex(string key)
        {
            for (int i = 0; i < currentPairs.Count; i++)
            {
                if (currentPairs[i].Key == key)
                    return i;
            }
            return -1;
        }

        private void PurgeStaleDirtyEntries()
        {
            if (inputPlayerPrefs.Count == 0) return;

            savedKey.Clear();
            foreach (var key in inputPlayerPrefs.Keys)
            {
                if (FindPairIndex(key) < 0)
                    savedKey.Add(key);
            }

            foreach (var key in savedKey)
                inputPlayerPrefs.Remove(key);
            savedKey.Clear();
        }
    }
}
