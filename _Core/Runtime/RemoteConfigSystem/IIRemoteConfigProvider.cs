using System;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IIRemoteConfigProvider : IService<IIRemoteConfigProvider>
    {
        public Action OnAllValueFetched { get; set; }
        
        public bool TryGetRemoteValue(string firebaseKey, out string value);
    }
}