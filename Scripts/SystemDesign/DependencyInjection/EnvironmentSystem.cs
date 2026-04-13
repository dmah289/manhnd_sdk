using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public class EnvironmentSystem : MonoBehaviour, IDependencyProvider
    {
        [Provide]
        public EnvironmentSystem ProvideES()
        {
            return this;
        }
        
        public void Init()
        {
            Debug.Log("EnvironmentSystem Init");
        }
    } 
}