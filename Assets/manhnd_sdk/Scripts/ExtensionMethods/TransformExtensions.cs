using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Return existing component T on Transform or add it if not found
        /// </summary>
        public static T GetOrAdd<T>(this Transform t) where T : Component
        {
            T component = t.GetComponent<T>();
            if(!component) component = t.gameObject.AddComponent<T>();
            return component;
        }
    }
}