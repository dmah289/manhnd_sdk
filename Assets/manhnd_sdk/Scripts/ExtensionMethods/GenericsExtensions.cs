using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class GenericsExtensions
    {
        /// <summary>
        /// Return real null in C# (not fake null of UnityEngine.Object)
        /// </summary>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
    }
}