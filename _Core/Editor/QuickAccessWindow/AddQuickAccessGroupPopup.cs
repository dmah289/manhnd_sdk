using System.Collections.Generic;
using manhnd_sdk.Modules.QuickAccessWindow;
using UnityEditor;
using UnityEngine;

namespace manhnd_sdk.QuickAccessWindow
{
    public class AddQuickAccessGroupPopup : PopupWindowContent
    {
        private string _groupName = "New Group";
        private bool _enableLoadingAssets = true;
        private bool _enableLoadingSubfolders = true;

        public AddQuickAccessGroupPopup(string groupName, bool enableLoadingAssets, bool enableLoadingSubfolders)
        {
            _groupName = groupName;
            _enableLoadingAssets = enableLoadingAssets;
            _enableLoadingSubfolders = enableLoadingSubfolders;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 130);
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            GUILayout.Label("Add Quick Access Group", EditorStyles.boldLabel);
            _groupName = EditorGUILayout.TextField("Group Name: ", _groupName);
            _enableLoadingAssets = EditorGUILayout.Toggle("Loading Assets", _enableLoadingAssets);
            _enableLoadingSubfolders = EditorGUILayout.Toggle("Loading Subfolders", _enableLoadingSubfolders);
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Confirm", GUILayout.Height(30)))
            {
                OnConfirmButtonClicked();
                editorWindow.Close();
            }
        }
        
        private void OnConfirmButtonClicked()
        {
            QuickAccessConfig config = QuickAccessConfig.Instance;
            
            AssetsInFolder newGroup = new AssetsInFolder
            {
                titleName = _groupName,
                enableLoadingAssets = _enableLoadingAssets,
                enableLoadingSubfolders = _enableLoadingSubfolders,
                rootFolderReferences = new(),
                Assets = new List<UnityEngine.Object>(),
                SubFolders = new List<UnityEngine.Object>()
            };
            
            config.assetsInFolder.Add(newGroup);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}