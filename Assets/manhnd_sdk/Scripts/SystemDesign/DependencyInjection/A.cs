using System;
using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public class A : MonoBehaviour
    {
        [Inject] ServiceA _serviceA;
        [Inject] EnvironmentSystem _environmentSystem;

        private void OnEnable()
        {
            _serviceA.Init();
            _environmentSystem.Init();
        }
    }
}