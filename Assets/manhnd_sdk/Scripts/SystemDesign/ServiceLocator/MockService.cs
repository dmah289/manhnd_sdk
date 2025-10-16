using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.ServiceLocator
{
    public interface ISerrializer
    {
        void Serialize();
    }
    
    public class MockService : ISerrializer
    {
        public void Serialize()
        {
            Debug.Log(289);
        }
    }
}
