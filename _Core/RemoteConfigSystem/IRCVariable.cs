namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IRCVariable
    {
        public string FirebaseKey { get;}
        public bool AllowFetching { get; set; }

        public void ApplyRemoteValue(IRemoteConfigProvider provider);
    }
}