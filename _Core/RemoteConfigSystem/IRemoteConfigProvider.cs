using System;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IRemoteConfigProvider : IService<IRemoteConfigProvider>
    {
        public event Action OnFetched;
        public bool IsFetched { get; }

        public bool TryGetRemoteValue(string firebaseKey, out string value);
    }
}