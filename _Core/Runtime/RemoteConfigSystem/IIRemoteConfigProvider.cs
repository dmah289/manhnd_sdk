namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IIRemoteConfigProvider : IService<IIRemoteConfigProvider>
    {
        public bool TryGetStringValue(string firebaseKey, out string value);
    }
}