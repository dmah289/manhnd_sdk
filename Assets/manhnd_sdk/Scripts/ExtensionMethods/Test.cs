using System;
using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public class Test : MonoBehaviour
    {
        private void Awake()
        {
            Transform t = transform;
            foreach (Transform child in t.Children())
            {
                Color a = Color.black;
                a.a = 1;
                
            }
        }
    }
}