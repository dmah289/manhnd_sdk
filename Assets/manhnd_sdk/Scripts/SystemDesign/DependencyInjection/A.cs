using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public class A : MonoBehaviour
    {
        ServiceA _serviceA;
        
        [Inject]
        public void SetServiceA(ServiceA serviceA)
        {
            _serviceA = serviceA;
            _serviceA.Init();
        }
    }
}