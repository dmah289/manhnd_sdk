using System.Collections.Generic;

namespace manhnd_sdk.Editor.PlayerPrefsEditor
{
    public interface IPlayerPrefsProvider
    {
        public List<PlayerPrefsPair> PlayerPrefsPairs { get; }
    }
}