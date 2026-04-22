using System;
using System.Collections.Generic;
using manhnd_sdk.Serializables;
using UnityEngine;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    [Serializable]
    public class AssetsInFolder
    {
        public string titleName;
        public bool enableLoadingAssets;
        public bool enableLoadingSubfolders;
        public FolderReference[] rootFolderReference;
        [HideInInspector] public List<Object> Assets;
        [HideInInspector] public List<Object> SubFolders;
        
        public void LoadAssets()
        {
            if (rootFolderReference == null || rootFolderReference.Length == 0)
            {
                if(Assets == null)
                    Assets = new List<Object>();
                if(SubFolders == null)
                    SubFolders = new List<Object>();
                    
                return;
            }
            
            Assets.Clear();
            SubFolders.Clear();
            
            for (int i = 0; i < rootFolderReference.Length; i++)
            {
                if (enableLoadingAssets)
                    Assets.AddRange(rootFolderReference[i].LoadAssets<Object>());

                if (enableLoadingSubfolders)
                    SubFolders.AddRange(rootFolderReference[i].GetSubFolders());
            }
        }
    }
}