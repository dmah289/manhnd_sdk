using System;
using System.Collections.Generic;
using manhnd_sdk.QuickAccessWindow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    public class QuickAccessWindow : EditorWindow, IHasCustomMenu
    {
        private static GUIStyle _titleStyle;
        private static GUIStyle _normalButtonStyle;
        private static GUIStyle _flexibleBtnStyle;
        
        private static Color dangerBtnColor = new (1f, 50/255f, 50/255f, 1f);
        
        private Vector2 _scrollPosition;
        private static Object[] _sceneObjects;
        
        
        private static Object[] BuildScenes
        {
            get
            {
                if(_sceneObjects == null)
                {
                    var buildScenes = EditorBuildSettings.scenes;
                    _sceneObjects = new Object[buildScenes.Length];

                    for (int i = 0; i < buildScenes.Length; i++)
                        _sceneObjects[i] = AssetDatabase.LoadAssetAtPath<Object>(buildScenes[i].path);
                }
                
                return _sceneObjects;
            }
        }
        
        
        #region Unity Callbacks

        private void OnEnable()
        {
            Refresh();
        }

        private void OnGUI()
        {
            InitializeGUIStyles();
            
            HandleDragAndDropCustomReferences();

            HandleMiddleMouseScroll();
            
            HandleScrollView();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Refresh"), false, Refresh);
            menu.AddItem(new GUIContent("Open Quick Access Config"), false, SelectQuickAccessConfig);
            menu.AddItem(new GUIContent("Add Quick Access Group"), false, AddQuickAccessGroup);
        }

        #endregion

        #region Menu Methods
        
        [MenuItem("manhnd_sdk/Window/Quick Access")]
        private static void ShowWindow()
        {
            var window = GetWindow<QuickAccessWindow>();
            window.titleContent = new GUIContent("Quick Access");
            window.Show();
        }
        
        private void SelectQuickAccessConfig()
        {
            QuickAccessConfig config = QuickAccessConfig.Instance;
            EditorGUIUtility.PingObject(config);
            AssetDatabase.OpenAsset(config);
        }
        
        private void Refresh()
        {
            QuickAccessConfig.Instance.LoadAllAssets();
        }

        private void AddQuickAccessGroup()
        {
            Rect btnRect = new Rect(0, 0, 1, 1);
            
            PopupWindow.Show(btnRect, new AddQuickAccessGroupPopup("New Group", true, true));
        }
        
        #endregion

        #region OnGUI Handlers
        
        private static void InitializeGUIStyles()
        {
            _titleStyle ??= new GUIStyle()
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _titleStyle.normal.textColor = Color.green;
            
            _normalButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };

            _flexibleBtnStyle ??= new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15
            };
        }
        
        private void HandleDragAndDropCustomReferences()
        {
            Event evt = Event.current;

            if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
            {
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(draggedObject);
                            RegisterCustomReference(path, draggedObject);
                        }
                    }
                    evt.Use(); 
                }
            }
        }
        
        private void HandleMiddleMouseScroll()
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                _scrollPosition += Event.current.delta * 10f;
                Event.current.Use();
            }
        }

        private void HandleScrollView()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(position.width),
                GUILayout.Height(position.height));

            DrawContents();

            GUILayout.EndScrollView();
        }

        private void DrawContents()
        {
            QuickAccessConfig config = QuickAccessConfig.Instance;
            
            GUILayout.Label("Favourite", _titleStyle);
            if (GUILayout.Button("Play Game"))
                OpenAndPlayScene(config.LoadingSceneName);
            
            GUILayout.Label("Build Scenes", _titleStyle);
            for(int i = 0 ; i < BuildScenes.Length; i++)
                DrawAssetButton(BuildScenes[i]);

            if(config.assetsInFolder != null && config.assetsInFolder.Count > 0)
            {
                List<AssetsInFolder> assetsInFolder = config.assetsInFolder;
                for (int i = 0; i < assetsInFolder.Count; i++)
                {
                    if(assetsInFolder[i].IsValid)
                    {
                        GUILayout.Label(assetsInFolder[i].titleName, _titleStyle);

                        if (assetsInFolder[i].enableLoadingAssets)
                        {
                            for (int j = 0; j < assetsInFolder[i].Assets.Count; j++)
                                DrawAssetButton(assetsInFolder[i].Assets[j]);
                        }

                        if (assetsInFolder[i].enableLoadingSubfolders)
                        {
                            for (int j = 0; j < assetsInFolder[i].SubFolders.Count; j++)
                                DrawFolderButton(assetsInFolder[i].SubFolders[j]);
                        }
                    }
                }
            }

            if(config.CustomAssetRefs != null && config.CustomAssetRefs.Count > 0)
            {
                GUILayout.Label("Custom Asset Refs", _titleStyle);
                for (int i = 0; i < config.CustomAssetRefs.Count; i++)
                    DrawAssetButton(config.CustomAssetRefs[i], true);
            }

            if (config.CustomFolderRefs != null && config.CustomFolderRefs.Count > 0)
            {
                GUILayout.Label("Custom Folder Refs", _titleStyle);
                for (int i = 0; i < config.CustomFolderRefs.Count; i++)
                    DrawFolderButton(config.CustomFolderRefs[i], true);
            }
        }

        private void DrawAssetButton(Object asset, bool isCustomRef = false)
        {
            if (asset == null)
                return;
            
            QuickAccessConfig config = QuickAccessConfig.Instance;
            
            GUILayout.BeginHorizontal();
            
            if(GUILayout.Button("Ping", GUILayout.MaxWidth(50)))
                EditorGUIUtility.PingObject(asset);
            
            if (GUILayout.Button(asset.name, _normalButtonStyle, GUILayout.MinWidth(150)))
                AssetDatabase.OpenAsset(asset);

            Color orginalColor = GUI.backgroundColor;
            GUI.backgroundColor = dangerBtnColor;
            if(GUILayout.Button("Remove", GUILayout.MinWidth(50), GUILayout.MaxWidth(60)))
                config.RemoveAssetRef(asset, isCustomRef);
            GUI.backgroundColor = orginalColor;
            
            GUILayout.EndHorizontal();
        }

        private void DrawFolderButton(Object folder, bool isCustomRef = false)
        {
            if (folder == null)
                return;
            
            QuickAccessConfig config = QuickAccessConfig.Instance;
            
            GUILayout.BeginHorizontal();
            
            if(GUILayout.Button(folder.name, _normalButtonStyle, GUILayout.MinWidth(200)))
                EditorGUIUtility.PingObject(folder);
            
            Color orginalColor = GUI.backgroundColor;
            GUI.backgroundColor = dangerBtnColor;
            if(GUILayout.Button("Remove", GUILayout.MinWidth(50), GUILayout.MaxWidth(60)))
                config.RemoveFolderRef(folder, isCustomRef);
            GUI.backgroundColor = orginalColor;
                    
            GUILayout.EndHorizontal();
        }

        #endregion

        #region OnGUI Methods
        
        private void OpenAndPlayScene(string targetSceneName)
        {
            bool found = false;
            
            Object[] sceneObjects = BuildScenes;
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i].name == targetSceneName)
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneObjects[i]));
                    found = true;
                }
            }
            
            if(found)
                EditorApplication.isPlaying = true;
            else
                throw new Exception($"Scene with name '{targetSceneName}' set up in QuickAccessConfig not found in Build Settings!");
        }

        private void RegisterCustomReference(string path, Object reference)
        {
            QuickAccessConfig config = QuickAccessConfig.Instance;
            
            if(AssetDatabase.IsValidFolder(path))
                config.AddCustomFolderRef(reference);
            else
                config.AddCustomAssetRef(reference);
        }

        #endregion

        
    }
}