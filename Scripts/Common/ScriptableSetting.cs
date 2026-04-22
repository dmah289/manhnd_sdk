using System;
using UnityEngine;

namespace manhnd_sdk.Common
{
    /// <summary>
    /// Unique scriptable config loaded from Resources folder.
    /// The asset must be named the same as the class name.
    /// </summary>
    public abstract class ScriptableSetting<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                if (!IsExisted())
                {
                    throw new Exception(
                        $"Scriptable setting for {typeof(T).Name} can not be found! Please create an asset of type {typeof(T).Name} with name {typeof(T).Name} in the Resources folder.");
                }

                return Resources.Load<T>(typeof(T).Name);
            }
        }
        
        public static bool IsExisted() => Resources.Load<T>(typeof(T).Name) != null;
    }
}