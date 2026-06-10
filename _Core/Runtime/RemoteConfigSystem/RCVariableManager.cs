using System;
using System.Collections.Generic;
using manhnd_sdk.Runtime.SystemDesign;
using UnityEngine;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RegisteredRCVar : Attribute { }
    
    [CreateAssetMenu(menuName = "manhnd_sdk/RCVariableManager", fileName = "RCVariableManager")]
    public partial class RCVariableManager : SingletonSO<RCVariableManager>
    {
        List<IRCVariable> rcVariables = new();

        public void Initialize()
        {
            
        }
    }
}