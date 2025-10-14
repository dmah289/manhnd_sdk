using System;
using System.Collections.Generic;
using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    public class A : MonoBehaviour
    {
        private void Awake()
        {
            RegisterCallbacks();
        }

        void OnPlayerStateChanged()
        {
            //Debug.Log($"Player state changed event received in {gameObject.name}");
        }

        void OnPlayerStateChanged1(PlayerStateEventDto playerStateEventDto)
        {
            //Debug.Log($"{playerStateEventDto.health} - {gameObject.name}");
        }
        
        void OnDummyChanged1(DummyDTO playerStateEventDto)
        {
            //Debug.Log($"{playerStateEventDto.num} - {gameObject.name}");
        }

        public void RegisterCallbacks()
        {
            EventBus<PlayerStateEventDto>.Register(onEventWithoutArgs: OnPlayerStateChanged,
                onEventWithArgs: OnPlayerStateChanged1);
            EventBus<DummyDTO>.Register(OnPlayerStateChanged, OnDummyChanged1);
        }

        public void DeregisterCallbacks()
        {
            EventBus<PlayerStateEventDto>.Deregister(onEventWithoutArgs: OnPlayerStateChanged,
                onEventWithArgs: OnPlayerStateChanged1);
            
            EventBus<DummyDTO>.Deregister(OnPlayerStateChanged, OnDummyChanged1);
        }
    }
}