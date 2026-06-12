using System.Collections.Generic;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IRCVariableCollection
    {
        public IEnumerable<IRCVariable> RCVariables { get; }
        public IRemoteConfigProvider RemoteConfigProvider { get; }
        
        public void Initialize();
    }
}