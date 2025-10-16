using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public class B : MonoBehaviour
    {
        [Inject]
        ServiceA _serviceA;
        
        FactoryA _factoryA;
        
        [Inject]
        public void SetFactoryA(FactoryA factoryA)
        {
            _factoryA = factoryA;
        }
    }
}