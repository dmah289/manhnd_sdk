using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public class ServiceA
    {
        public void Init()
        {
            Debug.Log("ServiceA Init");
        }
    }

    public class FactoryA
    {
        ServiceA serviceA;
        
        public ServiceA CreateServiceA()
        {
            if(serviceA == null) serviceA = new ServiceA();
            return serviceA;
        }
    }
}