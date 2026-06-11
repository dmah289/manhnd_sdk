using System;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IRemoteConfigProvider : IService<IRemoteConfigProvider>
    {
        public event Action OnFetching;

        public bool TryGetRemoteValue(string firebaseKey, out string value);
    }
}