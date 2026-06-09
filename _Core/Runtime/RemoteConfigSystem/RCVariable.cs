using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
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
        [NonSerialized] private bool enableFetching;
        [SerializeField] private T defaultValue;
        [SerializeField] private T fetchedValue;
        

        public string FirebaseKey => firebaseKey;
        public bool EnableFetching => enableFetching;

        public T Value => enableFetching ? fetchedValue : defaultValue;
        
        
        public void Init()
        {
            if (enableFetching)
            {
                string prefsVal = PlayerPrefs.GetString(firebaseKey, string.Empty);
                fetchedValue = string.IsNullOrEmpty(prefsVal) ? defaultValue : JsonUtility.FromJson<T>(prefsVal);
            }
        }

        public void FetchValue()
        {
            throw new NotImplementedException();
        }

        public T GetValueFromString(string serializedVal)
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
    }
}