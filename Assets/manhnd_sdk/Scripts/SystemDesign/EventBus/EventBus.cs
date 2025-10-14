using System;
using System.Collections.Generic;
using manhnd_sdk.Scripts.Optimization.ConstantKey;

namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    
    public static class EventBus<T> where T : IEventDTO
    {
        private static readonly IEventBinding<T> Binding = new EventBinding<T>();

        public static void Register(Action onEventWithoutArgs = null,
                                    Action<T> onEventWithArgs = null)
        {
            Binding.Add(onEventWithoutArgs);
            
            if(onEventWithArgs != null)
                Binding.Add(onEventWithArgs);
        }
        
        public static void Deregister(Action onEventWithoutArgs = null,
                                      Action<T> onEventWithArgs = null)
        {
            Binding.Remove(onEventWithArgs);
            
            if(onEventWithoutArgs != null)
                Binding.Remove(onEventWithoutArgs);
        }
        
        public static void RaiseWithData(IEventDTO eventDTO)
            => Binding.RaiseWithData(eventDTO);
        
        public static void Raise()
            => Binding.Raise();

        public static void RaiseAll(IEventDTO eventDTO)
        {
            Binding.RaiseWithData(eventDTO);
            Binding.Raise();
        }
    }
}