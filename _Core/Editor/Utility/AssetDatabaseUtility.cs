using UnityEngine;

namespace manhnd_sdk.Editor.Utility
{
    public static class AssetDatabaseUtility
    {
        public static string GetGuid(Object target)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(target);
            return UnityEditor.AssetDatabase.AssetPathToGUID(path);
        }
    }
}