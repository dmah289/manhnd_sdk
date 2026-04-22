#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    public class QuickAccessWindow : EditorWindow
    {
        private static GUIStyle _titleStyle;
        private static GUIStyle _buttonStyle;
        
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
        

        [MenuItem("manhnd_sdk/Window/Quick Access")]
        private static void ShowWindow()
        {
            var window = GetWindow<QuickAccessWindow>();
            window.titleContent = new GUIContent("Quick Access");
            window.Show();
        }

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
            
            QuickAccessConfig.Instance.LoadAllAssets();
        }

        private void OnGUI()
        {
            if (!QuickAccessConfig.IsExisted())
                return;
            
            HandleMiddleMouseScroll();
            
            HandleScrollView();
        }

        #endregion

        #region Methods
        
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
            GUILayout.Label("Main");
            if (GUILayout.Button("Play game from build settings"))
            {
                OpenAndPlayScene(QuickAccessConfig.Instance.LoadingSceneName);
            }
                
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
                Debug.LogError($"Scene '{targetSceneName}' not found in Build Settings!");
        }
        
        private void Refresh()
        {
            QuickAccessConfig.Instance.LoadAllAssets();
        }

        #endregion
    }
}

#endif