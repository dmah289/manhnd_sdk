using System;
using System.Collections.Generic;
using manhnd_sdk.Serializables;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    /// <summary>
    /// One section in the Quick Access window.
    ///   • <c>roots</c> — folders scanned on Refresh to populate the loaded cache.
    ///   • <c>pinnedAssets</c> / <c>pinnedFolders</c> — direct shortcuts; folders here are reference-only.
    ///   • <c>loadedAssets</c> / <c>loadedSubfolders</c> — derived cache rebuilt by <see cref="RebuildLoaded"/>.
    /// </summary>
    [Serializable]
    [MovedFrom(false, sourceClassName: "AssetsInFolder")]
    public class QuickAccessGroup
    {
        [FormerlySerializedAs("titleName")]                public string title;
        [FormerlySerializedAs("enableLoadingAssets")]      public bool loadAssets;
        [FormerlySerializedAs("enableLoadingSubfolders")] public bool loadSubfolders;
        [FormerlySerializedAs("rootFolderReferences")]    public List<FolderReference> roots;

        [FormerlySerializedAs("PinnedAssets")]  public List<Object> pinnedAssets;
        [FormerlySerializedAs("PinnedFolders")] public List<Object> pinnedFolders;

        [HideInInspector, FormerlySerializedAs("Assets")]     public List<Object> loadedAssets;
        [HideInInspector, FormerlySerializedAs("SubFolders")] public List<Object> loadedSubfolders;

        [NonSerialized] public bool foldoutExpanded = true;
        [NonSerialized] public bool editExpanded;

        public void RebuildLoaded()
        {
            loadedAssets ??= new List<Object>();
            loadedSubfolders ??= new List<Object>();

            loadedAssets.Clear();
            loadedSubfolders.Clear();

            if (roots == null) return;

            for (int i = 0; i < roots.Count; i++)
            {
                FolderReference root = roots[i];
                if (root == null || !root.IsValid) continue;

                if (loadAssets) loadedAssets.AddRange(root.LoadAssets<Object>());
                if (loadSubfolders) loadedSubfolders.AddRange(root.GetSubFolders());
            }
        }
    }
}
