using System;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IRemoteConfigProvider : IService<IRemoteConfigProvider>
    {
        public Action OnFetching { get; set; }
        
        public bool TryGetRemoteValue(string firebaseKey, out string value);
    }
}