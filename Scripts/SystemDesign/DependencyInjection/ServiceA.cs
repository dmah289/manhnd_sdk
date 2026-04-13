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
        public void Init()
        {
            Debug.Log("FactoryA Init");
        }
    }
}