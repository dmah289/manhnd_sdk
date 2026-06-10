using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace manhnd_sdk.Runtime.RemoteConfigSystem
{
    public interface IRCVariable
    {
        public string FirebaseKey { get;}
        public bool EnableFetching { get; }
        
        public void Init();
        public void FetchValue();
    }
    
    [Serializable]
    public class RCVariable<T> : IRCVariable
    {
        [SerializeField] private string firebaseKey;
        [SerializeField] private bool enableFetching;
        [SerializeField] private T defaultValue;
        [SerializeField] private T fetchedValue;
        [SerializeField] private bool fetched;

        public string FirebaseKey => firebaseKey;
        public bool EnableFetching => enableFetching;
        public T Value => fetched ? fetchedValue : defaultValue;
        
        
        public void Init()
        {
            if (!enableFetching)
                return;
            
            string prefsVal = PlayerPrefs.GetString(firebaseKey, string.Empty);
            fetchedValue = string.IsNullOrEmpty(prefsVal) ? defaultValue : GetStandardValueFromString(prefsVal);
        }

        public void FetchValue()
        {
            if (!enableFetching)
                return;

            bool exists = IIRemoteConfigProvider.Service.TryGetRemoteValue(firebaseKey, out string fetchedVal);
            if (exists)
            {
                fetchedValue = GetStandardValueFromString(fetchedVal);
                PlayerPrefs.SetString(firebaseKey, fetchedVal);
                fetched = true;
            }
            // else
            // {
            //     string prefsVal = PlayerPrefs.GetString(firebaseKey, string.Empty);
            //     fetchedValue = string.IsNullOrEmpty(prefsVal) ? defaultValue : GetStandardValueFromString(prefsVal);
            // }
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
            string json = JsonConvert.SerializeObject(Value, Formatting.Indented);
            GUIUtility.systemCopyBuffer = json;
            Debug.Log($"Copied RCVariable with key {firebaseKey} to clipboard:\n{json}");
        }

        [Button]
        private void ImportDefaultValue(string defaultVal)
        {
            defaultValue = GetStandardValueFromString(defaultVal);
        }
        
        #endif
    }
}