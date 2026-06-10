using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IRCVariable
    {
        public string FirebaseKey { get;}
        public bool AllowFetching { get; }
        
        public void FetchValue();
    }
    
    /// <summary>
    /// If not allowFetching, value will be set on editor.
    /// if allowFetching is true:
    ///    + Try to fetch value from remote config provider -> cache it to PlayerPrefs.
    ///    + If not success, try to fetch value from PlayerPrefs
    ///    + If not success, use default value set on editor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RCVariable<T> : IRCVariable
    {
        [SerializeField] private string firebaseKey;
        [SerializeField] private bool allowFetching;
        [SerializeField] private T value;
        [SerializeField] private bool fetched;

        public string FirebaseKey => firebaseKey;
        public bool AllowFetching => allowFetching;
        public T Value => value;
        
        public static implicit operator T(RCVariable<T> variable) => variable.Value;

        public void FetchValue()
        {
            if (!allowFetching)
                return;

            bool exists = IRemoteConfigProvider.Service.TryGetRemoteValue(firebaseKey, out string fetchedVal);
            if (exists && !string.IsNullOrEmpty(fetchedVal))
            {
                value = GetStandardValueFromString(fetchedVal);
                PlayerPrefs.SetString(firebaseKey, fetchedVal);
                fetched = true;

                return;
            }
            
            Debug.LogError($"Can't find remote config value for key {firebaseKey}");
            
            string prefsVal = PlayerPrefs.GetString(firebaseKey, string.Empty);
            if(!string.IsNullOrEmpty(prefsVal))
                value = GetStandardValueFromString(prefsVal);
            else 
                Debug.LogError($"Can't find remote config value for key {firebaseKey} in PlayerPrefs -> Using default value st on Editor");
        }

        public T GetStandardValueFromString(string serializedVal)
        {
            try
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)serializedVal;

                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Int32:
                        return (T)(object)int.Parse(serializedVal);
                    case TypeCode.Boolean:
                        return (T)(object)bool.Parse(serializedVal);
                    case TypeCode.Single:
                        // BẮT BUỘC dùng InvariantCulture để tránh lỗi dấu chấm/phẩy ở các quốc gia khác
                        return (T)(object)float.Parse(serializedVal, CultureInfo.InvariantCulture);
                    case TypeCode.Double:
                        return (T)(object)double.Parse(serializedVal, CultureInfo.InvariantCulture);
                    default:
                        return JsonConvert.DeserializeObject<T>(serializedVal);
                }
            }
            catch
            {
                Debug.LogError($"Can't parse remote config value for key {firebaseKey} with value {serializedVal} to type {typeof(T)}");
                return default;
            }
        }
        
        #if UNITY_EDITOR
        
        [Button]
        private void CopyJsonToClipboard()
        {
            string json = JsonConvert.SerializeObject(value, Formatting.Indented);
            GUIUtility.systemCopyBuffer = json;
            Debug.Log($"Copied RCVariable with key {firebaseKey} to clipboard:\n{json}");
        }

        [Button]
        private void ImportDefaultValue(string defaultVal)
        {
            if(!string.IsNullOrEmpty(defaultVal))
                value = GetStandardValueFromString(defaultVal);
        }
        
        #endif
    }
}