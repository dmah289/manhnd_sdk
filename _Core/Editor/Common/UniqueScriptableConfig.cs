#if UNITY_EDITOR
using manhnd_sdk.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace manhnd_sdk.Common
{
    /// <summary>
    /// Unique scriptable config loaded from Resources folder.
    /// The asset must be named the same as the class name.
    /// </summary>
    public abstract class UniqueScriptableConfig<T> : ScriptableObject where T : ScriptableObject
    {
        private static string folderPath = "Assets/manhnd_sdk/Resources/UniqueScriptableConfigs";
        
        protected static T _instance;
        public static T Instance
        {
            get
            {
                string configPath = $"{folderPath}/{typeof(T).Name}.asset";
                _instance = AssetDatabase.LoadAssetAtPath<T>(configPath);
                
                if (_instance == null)
                {
                    if (!AssetDatabase.IsValidFolder(folderPath))
                        FolderUtility.CreateFolderRecursively(folderPath);
                    
                    _instance = CreateInstance<T>();
                    AssetDatabase.CreateAsset(_instance, configPath);
                    AssetDatabase.SaveAssets();
                    
                    EditorGUIUtility.PingObject(_instance);
                }

                return _instance;
            }
        }
    }
}
#endif