using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using manhnd_sdk.Serializables;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    [Serializable]
    public class AssetsInFolder
    {
        public string titleName;
        public bool enableLoadingAssets;
        public bool enableLoadingSubfolders;
        public List<FolderReference> rootFolderReferences;
        [HideInInspector] public List<Object> Assets;
        [HideInInspector] public List<Object> SubFolders;

        public bool IsValid => !String.IsNullOrEmpty(titleName) 
                               && ((Assets != null && Assets.Count > 0)
                                   || (SubFolders != null && SubFolders.Count > 0));
        
        public void LoadAssets()
        {
            if (rootFolderReferences == null || rootFolderReferences.Count == 0)
            {
                if(Assets == null)
                    Assets = new List<Object>();
                if(SubFolders == null)
                    SubFolders = new List<Object>();
                    
                return;
            }
            
            Assets.Clear();
            SubFolders.Clear();
            
            for (int i = 0; i < rootFolderReferences.Count; i++)
            {
                if (rootFolderReferences[i].IsValid)
                {
                    if (enableLoadingAssets)
                        Assets.AddRange(rootFolderReferences[i].LoadAssets<Object>());

                    if (enableLoadingSubfolders)
                        SubFolders.AddRange(rootFolderReferences[i].GetSubFolders());
                }
            }
        }
    }
}