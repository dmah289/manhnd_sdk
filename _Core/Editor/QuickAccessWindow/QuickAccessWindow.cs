using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    public class QuickAccessWindow : EditorWindow
    {
        private static GUIStyle _titleStyle;
        private static GUIStyle _normalButtonStyle;
        private static GUIStyle _flexibleBtnStyle;
        
        private static Color flexibleBtnColor = new (80/255f, 1f, 100/255f, 1f);
        private static Color dangerBtnColor = new (1f, 50/255f, 50/255f, 1f);
        
        private Vector2 _scrollPosition;
        
        private static Object[] BuildScenes
        {
            get
            {
                var buildScenes = EditorBuildSettings.scenes;
                Object[] sceneObjects = new Object[buildScenes.Length];

                for (int i = 0; i < buildScenes.Length; i++)
                {
                    sceneObjects[i] = AssetDatabase.LoadAssetAtPath<Object>(buildScenes[i].path);
                }
                
                return sceneObjects;
            }
        }

        #region Menu Item
        
        [MenuItem("manhnd_sdk/Window/Quick Access/Show Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<QuickAccessWindow>();
            window.titleContent = new GUIContent("Quick Access");
            window.Show();
        }
        
        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            _titleStyle = new GUIStyle()
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _titleStyle.normal.textColor = Color.green;
            
            Refresh();
        }

        private void OnGUI()
        {
            HandleDragAndDropCustomReferences();
            
            if(_normalButtonStyle == null)
                _normalButtonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };

            if (_flexibleBtnStyle == null)
            {
                _flexibleBtnStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 15
                };
            }
            
            HandleMiddleMouseScroll();
            
            HandleScrollView();
        }

        #endregion

        #region Methods

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
        
        private void RegisterCustomReference(string path, Object reference)
        {
            QuickAccessConfig config = QuickAccessConfig.Instance;
            
            if(AssetDatabase.IsValidFolder(path))
                config.AddCustomFolderRef(reference);
            else
                config.AddCustomAssetRef(reference);
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
            
            DrawFlexibleContents();

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

            AssetsInFolder[] assetsInFolder = config.assetsInFolder;
            for (int i = 0; i < assetsInFolder.Length; i++)
            {
                GUILayout.Label(assetsInFolder[i].titleName, _titleStyle);

                if (assetsInFolder[i].enableLoadingAssets)
                {
                    for (int j = 0; j < assetsInFolder[i].Assets.Count; j++)
                        DrawAssetButton(assetsInFolder[i].Assets[j]);
                }

                if (assetsInFolder[i].enableLoadingSubfolders)
                {
                    for(int j = 0; j < assetsInFolder[i].SubFolders.Count; j++)
                        DrawFolderButton(assetsInFolder[i].SubFolders[j]);
                }
            }

            if(config.CustomAssetRefs.Count > 0)
            {
                GUILayout.Label("Custom Asset Refs", _titleStyle);
                for (int i = 0; i < config.CustomAssetRefs.Count; i++)
                    DrawAssetButton(config.CustomAssetRefs[i], true);
            }

            if(config.CustomFolderRefs.Count > 0)
            {
                GUILayout.Label("Custom Folder Refs", _titleStyle);
                for (int i = 0; i < config.CustomFolderRefs.Count; i++)
                    DrawFolderButton(config.CustomFolderRefs[i], true);
            }
        }

        private void DrawFlexibleContents()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Quick Access Config", _flexibleBtnStyle, GUILayout.Height(30)))
            {
                EditorGUIUtility.PingObject(QuickAccessConfig.Instance);
                AssetDatabase.OpenAsset(QuickAccessConfig.Instance);
            }
            
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = flexibleBtnColor;
            if(GUILayout.Button("Refresh",_flexibleBtnStyle, GUILayout.Height(40)))
                Refresh();
            GUI.backgroundColor = originalColor;
            
            
        }

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
        
        private void Refresh()
        {
            QuickAccessConfig.Instance.LoadAllAssets();
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
    }
}