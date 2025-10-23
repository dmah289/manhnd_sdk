using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class GameObjectExtensions
    {
        public static T GetOrAdd<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if(!component) component = go.AddComponent<T>();
            return component;
        }
        
        /// <summary>
        /// Return real null in C# (not fake null of UnityEngine.Object)
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
    }
}