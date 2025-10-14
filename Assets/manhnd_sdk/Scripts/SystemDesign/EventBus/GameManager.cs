using System;
using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    public class GameManager : MonoBehaviour
    {
        private void Update()
        {
            // Notify();

            for (int i = 0; i < 100; i++)
            {
                EventBus<PlayerStateEventDto>.Raise();
                EventBus<PlayerStateEventDto>.RaiseWithData(new PlayerStateEventDto(28));
                EventBus<DummyDTO>.Raise();
                EventBus<DummyDTO>.RaiseWithData(new DummyDTO(31));
            }
        }

        public void Notify()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                EventBus<PlayerStateEventDto>.RaiseAll(new PlayerStateEventDto(28));
            }
        }
    }
}