using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Return existing component T on GameObject or add it if not found
        /// </summary>
        public static T GetOrAdd<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if(!component) component = go.AddComponent<T>();
            return component;
        }
    }
}