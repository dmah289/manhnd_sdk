using UnityEditor;
using UnityEngine;

namespace manhnd_sdk.EditorTool.Common
{
    public static class StaticGUIContent
        {
            // Stable text — eager init.
            public static readonly GUIContent AddGroup       = new("Add Group", "Append a new group");
            public static readonly GUIContent Edit           = new("Edit", "Toggle inline editor");
            public static readonly GUIContent MoveUp         = new("▲", "Move group up");
            public static readonly GUIContent MoveDown       = new("▼", "Move group down");
            public static readonly GUIContent Ping           = new("Ping", "Highlight in Project window");
            public static readonly GUIContent Remove         = new("Remove");
            public static readonly GUIContent PlayGame       = new("▶ Play Game", "Open the loading scene and enter Play mode");
            public static readonly GUIContent BuildScenes    = new("Build Scenes");
            public static readonly GUIContent TitleField     = new("Title");
            public static readonly GUIContent LoadFilesFromRoots      = new("Enable Loading Files From Root Folders");
            public static readonly GUIContent LoadSubfoldersFromRoots = new("Enable Loading Subfolders From Root Folders");
            public static readonly GUIContent LoadRecursively         = new("Enable Loading Recursively");
            public static readonly GUIContent RootFolders    = new("Root Folders");
            public static readonly GUIContent PinnedSection  = new("📌 Pinned", "Items pinned directly to this group (folders here are reference-only).");
            public static readonly GUIContent LoadedSection  = new("From root folders");
            public static readonly GUIContent EmptyRoots     = new("Hold Shift + drop folders here to load all below references.");
            public static readonly GUIContent EmptyGroup     = new("Drop to pin, or hold Shift while dropping folders to add as roots.");

            // Icon-bearing content — lazy-init from EditorGUIUtility (icons require editor skin).
            public static GUIContent Refresh;
            public static GUIContent Trash;     // group/header X
            public static GUIContent TrashSmall; // root folder X (smaller scale)

            private static bool _iconsLoaded;

            public static void EnsureIcons()
            {
                if (_iconsLoaded) return;
                _iconsLoaded = true;

                Refresh    = WithIcon("Refresh",            "Reload all groups and rescan build scenes", "↻");
                Trash      = WithIcon("d_TreeEditor.Trash", "Remove",                                    "X");
                TrashSmall = WithIcon("d_TreeEditor.Trash", "Remove",                                    "×");
            }

            private static GUIContent WithIcon(string iconName, string tooltip, string textFallback)
            {
                Texture image = EditorGUIUtility.IconContent(iconName)?.image;
                return image != null ? new GUIContent(image, tooltip) : new GUIContent(textFallback, tooltip);
            }

            // Dynamic label "Scene: <name>" with cached invalidation by current value.
            private static GUIContent _loadingButton;
            private static string     _loadingButtonCachedFor;

            public static GUIContent LoadingButton(string sceneName)
            {
                if (_loadingButton == null)
                    _loadingButton = new GUIContent { tooltip = "Loading scene played by Play Game" };

                if (_loadingButtonCachedFor != sceneName)
                {
                    _loadingButton.text = string.IsNullOrEmpty(sceneName) ? "Scene: <none>" : "Scene: " + sceneName;
                    _loadingButtonCachedFor = sceneName;
                }
                return _loadingButton;
            }
        }
}