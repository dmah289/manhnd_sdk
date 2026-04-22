using manhnd_sdk.Common;
using UnityEngine;

namespace manhnd_sdk.Modules.QuickAccessWindow
{
    [CreateAssetMenu(fileName = "QuickAccessConfig", menuName = "Scriptable Settings/Quick Access Config")]
    public class QuickAccessConfig : ScriptableSetting<QuickAccessConfig>
    {
        public string LoadingSceneName;
        public AssetsInFolder[] assetsInFolder;
        
        public void LoadAllAssets()
        {
            if (!IsExisted())
                return;
            
            foreach (var assets in assetsInFolder)
            {
                assets.LoadAssets();
            }
        }
    }
}