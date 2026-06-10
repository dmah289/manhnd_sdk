using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using manhnd_sdk.Runtime.SystemDesign;
using Sirenix.OdinInspector;
using UnityEngine;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RegisteredRCVar : Attribute { }
    
    [CreateAssetMenu(menuName = "manhnd_sdk/RCVariableCollection", fileName = "RCVariableCollection")]
    public partial class RCVariableCollection : SingletonSO<RCVariableCollection>
    {
        List<IRCVariable> rcVariables = new();

        public void Initialize()
        {
            List<FieldInfo> rcFields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.IsDefined(typeof(RegisteredRCVar), false)).ToList();

            for (int i = 0; i < rcFields.Count; i++)
                rcVariables.Add((IRCVariable)rcFields[i].GetValue(this));
            
            IRemoteConfigProvider.Service.OnFetching += OnRemoteConfigFetching;
        }

        private void OnRemoteConfigFetching()
        {
            for(int i = 0; i < rcVariables.Count; i++)
                rcVariables[i].FetchValue();
        }
        
        #if UNITY_EDITOR
        [Button]
        [GUIColor("cyan")]
        public void SearchFirebaseKeyUsage(string firebaseKey)
        {
            List<FieldInfo> rcFields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.IsDefined(typeof(RegisteredRCVar), false)).ToList();
            
            for(int i = 0; i < rcFields.Count; i++)
            {
                IRCVariable variable = (IRCVariable)rcFields[i].GetValue(this);
                if (variable.FirebaseKey == firebaseKey)
                {
                    Debug.Log($"Found usage of firebase key {firebaseKey} in field {rcFields[i].Name}");
                    break;
                }
            }
        }
        #endif
    }
}