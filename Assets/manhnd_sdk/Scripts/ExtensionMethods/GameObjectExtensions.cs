using System;
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
        
        /// <summary>
        /// Traverse all children of a GameObject and perform an action on each child
        /// </summary>
        /// <param name="action">Action to perform on each child</param>
        /// <param name="reverseOrder">Control application action order</param>
        public static void PerformActionOnChildren(this GameObject parent, Action<GameObject> action, bool reverseOrder = false)
        {
            if (!reverseOrder)
            {
                for (int i = 0; i < parent.transform.childCount; i++)
                    action(parent.transform.GetChild(i).gameObject);
            }
            else
            {
                for (int i = parent.transform.childCount - 1; i >= 0; i--)
                    action(parent.transform.GetChild(i).gameObject);
            }
        }
    }
}