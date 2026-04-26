using System;
using System.Collections.Generic;
using manhnd_sdk.Serializables;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    /// <summary>
    /// Editor window providing fast access to scenes, configurable groups of folders, and pinned shortcuts.
    /// Drag-drop into a group:
    ///   • all folders, no Shift → added as roots (auto-loaded into group)
    ///   • Shift held, or any non-folder → pinned (reference-only)
    /// Drops outside any group are rejected.
    /// </summary>
    public class QuickAccessWindow : EditorWindow
    {
        // ──────────────── GUI cache (allocated once per editor session) ────────────────

        private static GUIStyle _titleStyle;
        private static GUIStyle _groupTitleStyle;
        private static GUIStyle _normalButtonStyle;
        private static GUIStyle _flexibleBtnStyle;
        private static GUIStyle _groupBoxStyle;
        private static GUIStyle _dropHintStyle;

        private static readonly Color _dangerColor = new(1f, 50f / 255f, 50f / 255f, 1f);
        private static readonly Color _rootHoverColor = new(0.4f, 0.8f, 1f, 0.6f);
        private static readonly Color _pinHoverColor  = new(1f, 0.85f, 0.4f, 0.7f);

        private static readonly GUIContent C_Refresh       = new("Refresh", "Reload all groups and rescan build scenes");
        private static readonly GUIContent C_AddGroup      = new("+ Group", "Append a new group");
        private static readonly GUIContent C_OpenConfig    = new("⚙", "Open the Quick Access config asset");
        private static readonly GUIContent C_LoadingScene  = new("Loading Scene:");
        private static readonly GUIContent C_Edit          = new("Edit", "Toggle inline editor");
        private static readonly GUIContent C_Up            = new("▲", "Move group up");
        private static readonly GUIContent C_Down          = new("▼", "Move group down");
        private static readonly GUIContent C_X             = new("X", "Remove");
        private static readonly GUIContent C_Ping          = new("Ping");
        private static readonly GUIContent C_Remove        = new("Remove");
        private static readonly GUIContent C_PlayGame      = new("Play Game");
        private static readonly GUIContent C_Favourite     = new("Favourite");
        private static readonly GUIContent C_BuildScenes   = new("Build Scenes");
        private static readonly GUIContent C_Title         = new("Title");
        private static readonly GUIContent C_LoadAssets    = new("Load Assets");
        private static readonly GUIContent C_LoadSubfolders = new("Load Subfolders");
        private static readonly GUIContent C_RootFolders   = new("Root Folders");
        private static readonly GUIContent C_PinnedSection = new("Pinned", "Items pinned directly to this group (folders here are reference-only).");
        private static readonly GUIContent C_LoadedSection = new("From root folders");
        private static readonly GUIContent C_DropRootHint  = new("Drop folders here to add as roots.");
        private static readonly GUIContent C_EmptyHint     = new("Drop a folder to add as root, or hold Shift while dropping to pin.");

        private static readonly GUILayoutOption[] L_TbRefresh    = { GUILayout.Width(70) };
        private static readonly GUILayoutOption[] L_TbAddGroup   = { GUILayout.Width(80) };
        private static readonly GUILayoutOption[] L_TbOpenConfig = { GUILayout.Width(28) };
        private static readonly GUILayoutOption[] L_TbScene      = { GUILayout.Width(160) };
        private static readonly GUILayoutOption[] L_TbLabel      = { GUILayout.Width(95) };
        private static readonly GUILayoutOption[] L_Mini24       = { GUILayout.Width(24) };
        private static readonly GUILayoutOption[] L_Mini40       = { GUILayout.Width(40) };
        private static readonly GUILayoutOption[] L_PlayBtn      = { GUILayout.Height(28) };
        private static readonly GUILayoutOption[] L_Ping         = { GUILayout.MaxWidth(50) };
        private static readonly GUILayoutOption[] L_AssetName    = { GUILayout.MinWidth(150) };
        private static readonly GUILayoutOption[] L_FolderName   = { GUILayout.MinWidth(200) };
        private static readonly GUILayoutOption[] L_Remove       = { GUILayout.MinWidth(50), GUILayout.MaxWidth(60) };

        // Build-scene cache. Static so multiple windows share one. Invalidated by sceneListChanged.
        private static Object[] _buildScenes      = Array.Empty<Object>();
        private static string[] _buildSceneNames  = Array.Empty<string>();
        private static string[] _buildScenePaths  = Array.Empty<string>();
        private static bool     _buildScenesDirty = true;

        // ──────────────── Per-instance state ────────────────

        private enum ItemSource { Loaded, Pinned }

        private QuickAccessConfig _config;
        private Vector2           _scroll;
        private readonly List<Rect> _groupRects = new();
        private int  _hoveredGroup = -1;
        private bool _hoverPinMode;
        private int  _pendingRemoveGroup = -1;

        // ──────────────── Lifecycle ────────────────

        [MenuItem("manhnd_sdk/Window/Quick Access")]
        private static void ShowWindow()
        {
            var window = GetWindow<QuickAccessWindow>();
            window.titleContent = new GUIContent("Quick Access");
            window.Show();
        }

        private void OnEnable()
        {
            EditorBuildSettings.sceneListChanged += MarkBuildScenesDirty;
            Refresh();
        }

        private void OnDisable()
        {
            EditorBuildSettings.sceneListChanged -= MarkBuildScenesDirty;
        }

        private void OnGUI()
        {
            EnsureStyles();
            _config = QuickAccessConfig.Instance;
            EnsureBuildScenes();

            if (Event.current.type == EventType.Repaint)
                _groupRects.Clear();

            HandleScrollWheelBoost();

            DrawToolbar();

            _scroll = GUILayout.BeginScrollView(_scroll);
            DrawFavouriteSection();
            DrawBuildScenesSection();
            DrawGroupsSection();
            HandleDragAndDrop();
            GUILayout.EndScrollView();

            ConsumeDeferredRemovals();
        }

        // ──────────────── One-shot caches ────────────────

        private static void EnsureStyles()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                _titleStyle.normal.textColor = Color.green;
            }
            _groupTitleStyle   ??= new GUIStyle(EditorStyles.foldout) { fontSize = 14, fontStyle = FontStyle.Bold };
            _normalButtonStyle ??= new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };
            _flexibleBtnStyle  ??= new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fontSize = 15 };
            _groupBoxStyle     ??= new GUIStyle(GUI.skin.box) { padding = new(8, 8, 6, 6), margin = new(2, 2, 4, 4) };
            _dropHintStyle     ??= new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Italic };
        }

        private static void EnsureBuildScenes()
        {
            if (!_buildScenesDirty) return;

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int n = scenes.Length;
            if (_buildScenes.Length      != n) _buildScenes      = new Object[n];
            if (_buildSceneNames.Length  != n) _buildSceneNames  = new string[n];
            if (_buildScenePaths.Length  != n) _buildScenePaths  = new string[n];

            for (int i = 0; i < n; i++)
            {
                string path = scenes[i].path;
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                _buildScenes[i]     = asset;
                _buildScenePaths[i] = path;
                _buildSceneNames[i] = asset != null ? asset.name : "<missing>";
            }

            _buildScenesDirty = false;
        }

        private static void MarkBuildScenesDirty() => _buildScenesDirty = true;

        // ──────────────── Toolbar ────────────────

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button(C_Refresh, EditorStyles.toolbarButton, L_TbRefresh)) Refresh();
            if (GUILayout.Button(C_AddGroup, EditorStyles.toolbarButton, L_TbAddGroup))
                _config.AddGroup("New Group", true, true);

            GUILayout.FlexibleSpace();

            GUILayout.Label(C_LoadingScene, EditorStyles.miniLabel, L_TbLabel);
            int currentIdx = Array.IndexOf(_buildSceneNames, _config.loadingSceneName);
            int newIdx = EditorGUILayout.Popup(currentIdx, _buildSceneNames, EditorStyles.toolbarPopup, L_TbScene);
            if (newIdx != currentIdx && newIdx >= 0 && newIdx < _buildSceneNames.Length)
                _config.SetLoadingSceneName(_buildSceneNames[newIdx]);

            if (GUILayout.Button(C_OpenConfig, EditorStyles.toolbarButton, L_TbOpenConfig))
                OpenConfigAsset();

            EditorGUILayout.EndHorizontal();
        }

        private void Refresh()
        {
            QuickAccessConfig.Instance.RebuildAllLoaded();
            MarkBuildScenesDirty();
            Repaint();
        }

        private static void OpenConfigAsset()
        {
            QuickAccessConfig config = QuickAccessConfig.Instance;
            EditorGUIUtility.PingObject(config);
            AssetDatabase.OpenAsset(config);
        }

        // ──────────────── Top-level sections ────────────────

        private void DrawFavouriteSection()
        {
            GUILayout.Label(C_Favourite, _titleStyle);
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_config.loadingSceneName)))
            {
                if (GUILayout.Button(C_PlayGame, _flexibleBtnStyle, L_PlayBtn))
                    PlayLoadingScene();
            }
        }

        private void DrawBuildScenesSection()
        {
            GUILayout.Label(C_BuildScenes, _titleStyle);
            for (int i = 0; i < _buildScenes.Length; i++)
                DrawAssetRow(_buildScenes[i]);
        }

        private void DrawGroupsSection()
        {
            if (_config.groups == null) return;
            for (int i = 0; i < _config.groups.Count; i++)
                DrawGroup(i, _config.groups[i]);
        }

        // ──────────────── Group rendering ────────────────

        private void DrawGroup(int index, QuickAccessGroup group)
        {
            bool hovered = _hoveredGroup == index;
            Color savedBg = default;

            if (hovered)
            {
                savedBg = GUI.backgroundColor;
                GUI.backgroundColor = _hoverPinMode ? _pinHoverColor : _rootHoverColor;
            }

            EditorGUILayout.BeginVertical(_groupBoxStyle);
            if (hovered) GUI.backgroundColor = savedBg;

            DrawGroupHeader(index, group);
            if (group.editExpanded)     DrawGroupEditor(index, group);
            if (group.foldoutExpanded)  DrawGroupItems(index, group);

            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
                _groupRects.Add(GUILayoutUtility.GetLastRect());
        }

        private void DrawGroupHeader(int index, QuickAccessGroup group)
        {
            EditorGUILayout.BeginHorizontal();

            string title = string.IsNullOrEmpty(group.title) ? "unnamed" : group.title;
            group.foldoutExpanded = EditorGUILayout.Foldout(group.foldoutExpanded, title, true, _groupTitleStyle);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(C_Edit, EditorStyles.miniButton, L_Mini40))
                group.editExpanded = !group.editExpanded;

            int last = _config.groups.Count - 1;
            using (new EditorGUI.DisabledScope(index == 0))
                if (GUILayout.Button(C_Up,   EditorStyles.miniButton, L_Mini24)) _config.MoveGroup(index, -1);
            using (new EditorGUI.DisabledScope(index >= last))
                if (GUILayout.Button(C_Down, EditorStyles.miniButton, L_Mini24)) _config.MoveGroup(index, +1);

            if (DangerButton(C_X, EditorStyles.miniButton, L_Mini24))
                ConfirmRemoveGroup(index, group);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGroupEditor(int index, QuickAccessGroup group)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();
            string title          = EditorGUILayout.TextField(C_Title, group.title);
            bool   loadAssets     = EditorGUILayout.Toggle(C_LoadAssets, group.loadAssets);
            bool   loadSubfolders = EditorGUILayout.Toggle(C_LoadSubfolders, group.loadSubfolders);
            if (EditorGUI.EndChangeCheck())
            {
                group.title          = title;
                group.loadAssets     = loadAssets;
                group.loadSubfolders = loadSubfolders;
                group.RebuildLoaded();
                EditorUtility.SetDirty(_config);
            }

            GUILayout.Space(4);
            EditorGUILayout.LabelField(C_RootFolders, EditorStyles.boldLabel);
            DrawRootFoldersEditor(index, group);

            EditorGUILayout.EndVertical();
        }

        private void DrawRootFoldersEditor(int index, QuickAccessGroup group)
        {
            List<FolderReference> roots = group.roots;
            if (roots == null || roots.Count == 0)
            {
                GUILayout.Label(C_DropRootHint, _dropHintStyle);
                return;
            }

            int removeAt = -1;
            for (int j = 0; j < roots.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();

                FolderReference fr = roots[j];
                string path  = fr != null ? fr.Path : null; // only one GUIDToAssetPath call
                bool   valid = !string.IsNullOrEmpty(path) && AssetDatabase.IsValidFolder(path);

                using (new EditorGUI.DisabledScope(!valid))
                {
                    if (GUILayout.Button(valid ? path : "<invalid>", EditorStyles.objectField))
                    {
                        Object folder = AssetDatabase.LoadAssetAtPath<Object>(path);
                        if (folder != null) EditorGUIUtility.PingObject(folder);
                    }
                }

                if (DangerButton(C_X, L_Mini24)) removeAt = j;

                EditorGUILayout.EndHorizontal();
            }

            if (removeAt >= 0) _config.RemoveRootFolderFromGroup(index, removeAt);
        }

        private void DrawGroupItems(int index, QuickAccessGroup group)
        {
            bool hasPinned = HasItems(group.pinnedAssets) || HasItems(group.pinnedFolders);
            bool hasLoaded = (group.loadAssets     && HasItems(group.loadedAssets))
                          || (group.loadSubfolders && HasItems(group.loadedSubfolders));

            if (hasPinned)
            {
                GUILayout.Label(C_PinnedSection, EditorStyles.miniBoldLabel);
                DrawAssetRows(group.pinnedAssets,  ItemSource.Pinned, index);
                DrawFolderRows(group.pinnedFolders, ItemSource.Pinned, index);
            }

            if (hasLoaded)
            {
                if (hasPinned) GUILayout.Label(C_LoadedSection, EditorStyles.miniBoldLabel);
                if (group.loadAssets)     DrawAssetRows(group.loadedAssets,     ItemSource.Loaded, index);
                if (group.loadSubfolders) DrawFolderRows(group.loadedSubfolders, ItemSource.Loaded, index);
            }

            if (!hasPinned && !hasLoaded)
                GUILayout.Label(C_EmptyHint, _dropHintStyle);
        }

        private void ConfirmRemoveGroup(int index, QuickAccessGroup group)
        {
            if (EditorUtility.DisplayDialog("Remove group",
                    $"Remove group '{group.title}'? This cannot be undone.", "Remove", "Cancel"))
                _pendingRemoveGroup = index;
        }

        // ──────────────── Item rendering ────────────────

        private void DrawAssetRows(List<Object> list, ItemSource source, int groupIndex)
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
                DrawAssetRow(list[i], source, groupIndex);
        }

        private void DrawFolderRows(List<Object> list, ItemSource source, int groupIndex)
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
                DrawFolderRow(list[i], source, groupIndex);
        }

        private void DrawAssetRow(Object asset, ItemSource source = ItemSource.Loaded, int groupIndex = -1)
        {
            if (asset == null) return;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(C_Ping, L_Ping))                                  EditorGUIUtility.PingObject(asset);
            if (GUILayout.Button(asset.name, _normalButtonStyle, L_AssetName))     AssetDatabase.OpenAsset(asset);
            if (DangerButton(C_Remove, L_Remove))                                  RemoveAsset(asset, source, groupIndex);
            GUILayout.EndHorizontal();
        }

        private void DrawFolderRow(Object folder, ItemSource source = ItemSource.Loaded, int groupIndex = -1)
        {
            if (folder == null) return;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(folder.name, _normalButtonStyle, L_FolderName)) EditorGUIUtility.PingObject(folder);
            if (DangerButton(C_Remove, L_Remove))                                RemoveFolder(folder, source, groupIndex);
            GUILayout.EndHorizontal();
        }

        private void RemoveAsset(Object asset, ItemSource source, int groupIndex)
        {
            if (source == ItemSource.Pinned) _config.RemovePinnedFromGroup(groupIndex, asset);
            else                             _config.RemoveLoadedAsset(asset);
        }

        private void RemoveFolder(Object folder, ItemSource source, int groupIndex)
        {
            if (source == ItemSource.Pinned) _config.RemovePinnedFromGroup(groupIndex, folder);
            else                             _config.RemoveLoadedFolder(folder);
        }

        // ──────────────── Drag-drop ────────────────

        private void HandleDragAndDrop()
        {
            Event evt = Event.current;
            EventType et = evt.type;

            if (et == EventType.DragExited)
            {
                ClearHover();
                return;
            }
            if (et != EventType.DragUpdated && et != EventType.DragPerform) return;

            Object[] dragged = DragAndDrop.objectReferences;
            if (dragged.Length == 0) return;

            int targetGroup = FindGroupAt(evt.mousePosition);
            if (targetGroup < 0)
            {
                ClearHover();
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                evt.Use();
                return;
            }

            // Shift forces pin mode. Without Shift: all-folders → root, otherwise → pin (files cannot be roots).
            bool pinMode = evt.shift || !AreAllFolders(dragged);

            UpdateHover(targetGroup, pinMode);
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (et == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (pinMode) _config.AddPinnedToGroup(targetGroup, dragged);
                else         _config.AddRootFoldersToGroup(targetGroup, dragged);
                ClearHover();
            }
            evt.Use();
        }

        private static bool AreAllFolders(Object[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                string p = AssetDatabase.GetAssetPath(items[i]);
                if (string.IsNullOrEmpty(p) || !AssetDatabase.IsValidFolder(p)) return false;
            }
            return true;
        }

        private int FindGroupAt(Vector2 pos)
        {
            for (int i = 0; i < _groupRects.Count; i++)
                if (_groupRects[i].Contains(pos)) return i;
            return -1;
        }

        private void UpdateHover(int group, bool pinMode)
        {
            if (_hoveredGroup == group && _hoverPinMode == pinMode) return;
            _hoveredGroup = group;
            _hoverPinMode = pinMode;
            Repaint();
        }

        private void ClearHover()
        {
            if (_hoveredGroup == -1 && !_hoverPinMode) return;
            _hoveredGroup = -1;
            _hoverPinMode = false;
            Repaint();
        }

        // ──────────────── Misc ────────────────

        private void HandleScrollWheelBoost()
        {
            if (Event.current.type != EventType.ScrollWheel) return;
            _scroll += Event.current.delta * 10f;
            Event.current.Use();
        }

        private void ConsumeDeferredRemovals()
        {
            if (_pendingRemoveGroup < 0) return;
            _config.RemoveGroup(_pendingRemoveGroup);
            _pendingRemoveGroup = -1;
            Repaint();
        }

        private void PlayLoadingScene()
        {
            string sceneName = _config.loadingSceneName;
            int idx = Array.IndexOf(_buildSceneNames, sceneName);
            if (idx < 0)
                throw new Exception($"Scene '{sceneName}' set in QuickAccessConfig is not in Build Settings.");

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            EditorSceneManager.OpenScene(_buildScenePaths[idx]);
            EditorApplication.isPlaying = true;
        }

        // ──────────────── Helpers ────────────────

        private static bool HasItems(List<Object> list) => list != null && list.Count > 0;

        private static bool DangerButton(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            Color bg = GUI.backgroundColor;
            GUI.backgroundColor = _dangerColor;
            bool clicked = GUILayout.Button(content, style, options);
            GUI.backgroundColor = bg;
            return clicked;
        }

        private static bool DangerButton(GUIContent content, GUILayoutOption[] options)
        {
            Color bg = GUI.backgroundColor;
            GUI.backgroundColor = _dangerColor;
            bool clicked = GUILayout.Button(content, options);
            GUI.backgroundColor = bg;
            return clicked;
        }
    }
}
