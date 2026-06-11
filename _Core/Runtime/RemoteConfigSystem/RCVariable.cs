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
        public bool AllowFetching { get; set; }

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
        public bool AllowFetching
        {
            get => allowFetching;
            set => allowFetching = value;
        }
        public T Value => value;
        
        public static implicit operator T(RCVariable<T> variable) => variable != null ? variable.Value : default;

        public void FetchValue()
        {
            if (!allowFetching)
                return;

            bool exists = IRemoteConfigProvider.Service.TryGetRemoteValue(firebaseKey, out string fetchedVal);
            if (exists && !string.IsNullOrEmpty(fetchedVal) && GetStandardValueFromString(fetchedVal, out T parsed))
            {
                value = parsed;
                PlayerPrefs.SetString(firebaseKey, fetchedVal);
                fetched = true;

                return;
            }

            Debug.LogError($"Can't find remote config value for key {firebaseKey}");

            string prefsVal = PlayerPrefs.GetString(firebaseKey, string.Empty);
            if(!string.IsNullOrEmpty(prefsVal) && GetStandardValueFromString(prefsVal, out T cachedParsed))
                value = cachedParsed;
            else
                Debug.LogError($"Can't find remote config value for key {firebaseKey} in PlayerPrefs -> Using default value set on Editor");
        }

        private bool GetStandardValueFromString(string serializedVal, out T result)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    result = (T)(object)serializedVal;
                    return true;
                }

                if (typeof(T).IsEnum)
                {
                    result = (T)Enum.Parse(typeof(T), serializedVal);
                    return true;
                }

                result = Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.Int32 => (T)(object)int.Parse(serializedVal),
                    TypeCode.Boolean => (T)(object)bool.Parse(serializedVal),
                    // BẮT BUỘC dùng InvariantCulture để tránh lỗi dấu chấm/phẩy ở các quốc gia khác
                    TypeCode.Single => (T)(object)float.Parse(serializedVal, CultureInfo.InvariantCulture),
                    TypeCode.Double => (T)(object)double.Parse(serializedVal, CultureInfo.InvariantCulture),
                    TypeCode.Int64 => (T)(object)long.Parse(serializedVal),
                    _ => JsonConvert.DeserializeObject<T>(serializedVal),
                };
                return true;
            }
            catch
            {
                Debug.LogError($"Can't parse remote config value for key {firebaseKey} with value {serializedVal} to type {typeof(T)}");
                result = default;
                return false;
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
            if(!string.IsNullOrEmpty(defaultVal) && GetStandardValueFromString(defaultVal, out T parsed))
                value = parsed;
        }
        
        #endif
    }
}