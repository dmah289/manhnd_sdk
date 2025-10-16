using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public class Provider : MonoBehaviour, IDependencyProvider
    {
        // Provide instances of dependencies here
        [Provide]
        public ServiceA ProvideServiceA()
        {
            return new ServiceA();
        }

        [Provide]
        public FactoryA ProvideFactoryA()
        {
            return new FactoryA();
        }
    }
}