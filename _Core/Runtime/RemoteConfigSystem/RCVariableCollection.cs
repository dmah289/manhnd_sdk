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
        private List<IRCVariable> rcVariables = new();
        private List<FieldInfo> cachedRCFields;
        private IRemoteConfigProvider provider;

        private List<FieldInfo> GetRCFields()
        {
            return cachedRCFields ??= GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.IsDefined(typeof(RegisteredRCVar), false)).ToList();
        }

        public void Initialize()
        {
            rcVariables.Clear();
            List<FieldInfo> rcFields = GetRCFields();

            for (int i = 0; i < rcFields.Count; i++)
                rcVariables.Add((IRCVariable)rcFields[i].GetValue(this));

            if (IRemoteConfigProvider.TryGet(out provider))
            {
                provider.OnFetched -= OnRemoteConfigFetched;
                provider.OnFetched += OnRemoteConfigFetched;

                if (provider.IsFetched)
                    OnRemoteConfigFetched();
            }
        }

        private void OnRemoteConfigFetched()
        {
            for(int i = 0; i < rcVariables.Count; i++)
                rcVariables[i].ApplyRemoteValue(provider);
        }

        private void OnDestroy()
        {
            if (provider != null)
                provider.OnFetched -= OnRemoteConfigFetched;
        }

#if UNITY_EDITOR
        [Button]
        [GUIColor("cyan")]
        public void SearchFirebaseKeyUsage(string firebaseKey)
        {
            List<FieldInfo> rcFields = GetRCFields();

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

        [Button]
        [GUIColor("green")]
        private void EnableAllFetching()
        {
            List<FieldInfo> rcFields = GetRCFields();

            for (int i = 0; i < rcFields.Count; i++)
            {
                IRCVariable variable = (IRCVariable)rcFields[i].GetValue(this);
                variable.AllowFetching = true;
            }

            Debug.Log($"Enabled allowFetching on {rcFields.Count} RCVariables");
        }
#endif
    }
}