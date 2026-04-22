using System;
using manhnd_sdk.Serializables;
using UnityEngine;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    [Serializable]
    public class AssetsInFolder : MonoBehaviour
    {
        [SerializeField] private FolderReference folderReference;
        [SerializeField] private Object[] assets;
        
        public void LoadAssets()
        {
            if (folderReference == null || !folderReference.IsValid)
            {
                assets = Array.Empty<Object>();
                return;
            }
            
            assets = folderReference.LoadAll<Object>();
        }
    }
}