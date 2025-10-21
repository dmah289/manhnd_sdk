using System;
using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public class A : MonoBehaviour
    {
        [Inject] ServiceA _serviceA;
        
        
        public void SetServiceA(ServiceA serviceA)
        {
            _serviceA = serviceA;
            _serviceA.Init();
        }

        private void OnEnable()
        {
            _serviceA.Init();
        }
    }
}