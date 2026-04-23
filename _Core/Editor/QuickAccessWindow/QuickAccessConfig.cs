using System.Collections.Generic;
using manhnd_sdk.Common;
using manhnd_sdk.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    [CreateAssetMenu(fileName = "QuickAccessConfig", menuName = "Scriptable Settings/Quick Access Config")]
    public class QuickAccessConfig : UniqueScriptableConfig<QuickAccessConfig>
    {
        public string LoadingSceneName;
        public AssetsInFolder[] assetsInFolder;
        
        [HideInInspector] public List<Object> CustomAssetRefs;
        [HideInInspector] public List<Object> CustomFolderRefs;
        
        
        public void LoadAllAssets()
        {
            if(assetsInFolder == null || assetsInFolder.Length == 0)
                return;
            
            foreach (var assets in assetsInFolder)
            {
                assets.LoadAssets();
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public void AddCustomAssetRef(Object target)
        {
            if (CustomAssetRefs == null)
                CustomAssetRefs = new List<Object>();
                
            if(!CustomFolderRefs.Contains(target))
                CustomAssetRefs.Add(target);
        }

        public void AddCustomFolderRef(Object target)
        {
            if (CustomFolderRefs == null)
                CustomFolderRefs = new List<Object>();
            
            if(!CustomFolderRefs.Contains(target))
                CustomFolderRefs.Add(target);
        }

        public void RemoveAssetRef(Object target, bool isCustomRef)
        {
            string targetGuid = AssetDatabaseUtility.GetGuid(target);

            if(isCustomRef)
            {
                for (int i = 0; i < CustomAssetRefs.Count; i++)
                {
                    if (AssetDatabaseUtility.GetGuid(CustomAssetRefs[i]).Equals(targetGuid))
                        CustomAssetRefs.Remove(CustomAssetRefs[i]);
                }
            }
            else
            {
                for (int i = 0; i < assetsInFolder.Length; i++)
                {
                    for(int j = 0; j < assetsInFolder[i].Assets.Count; j++)
                    {
                        if (AssetDatabaseUtility.GetGuid(assetsInFolder[i].Assets[j]).Equals(targetGuid))
                            assetsInFolder[i].Assets.Remove(assetsInFolder[i].Assets[j]);
                    }
                }
            }
        }

        public void RemoveFolderRef(Object folder, bool isCustomRef = false)
        {
            string targetGuid = AssetDatabaseUtility.GetGuid(folder);

            if (isCustomRef)
            {
                for (int i = 0; i < CustomFolderRefs.Count; i++)
                {
                    if (AssetDatabaseUtility.GetGuid(CustomFolderRefs[i]).Equals(targetGuid))
                        CustomFolderRefs.Remove(CustomFolderRefs[i]);
                }
            }
            else
            {
                for (int i = 0; i < assetsInFolder.Length; i++)
                {
                    for(int j = 0; j < assetsInFolder[i].SubFolders.Count; j++)
                    {
                        if (AssetDatabaseUtility.GetGuid(assetsInFolder[i].SubFolders[j]).Equals(targetGuid))
                            assetsInFolder[i].SubFolders.Remove(assetsInFolder[i].SubFolders[j]);
                    }
                }
            }
        }
    }
}