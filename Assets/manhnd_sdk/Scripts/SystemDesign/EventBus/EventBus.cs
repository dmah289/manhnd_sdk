using System;
using System.Collections.Generic;

namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    public interface IEventBusListener
    {
        public void RegisterCallbacks();
        public void DeregisterCallbacks();
    }
    
    public static class EventBus<T> where T : IEventDTO
    {
        private static readonly IEventBinding<T> Binding = new EventBinding<T>();

        /// <summary>
        /// Register event listeners
        /// </summary>
        public static void Register(Action onEventWithoutArgs = null,
                                    Action<T> onEventWithArgs = null)
        {
            Binding.Add(onEventWithoutArgs);
            
            if(onEventWithArgs != null)
                Binding.Add(onEventWithArgs);
        }
        
        /// <summary>
        /// Deregister event listeners
        /// </summary>
        public static void Deregister(Action onEventWithoutArgs = null,
                                      Action<T> onEventWithArgs = null)
        {
            Binding.Remove(onEventWithArgs);
            
            if(onEventWithoutArgs != null)
                Binding.Remove(onEventWithoutArgs);
        }
        
        /// <summary>
        /// Raise event with args
        /// </summary>
        public static void Raise(T eventDTO)
            => Binding.Raise(eventDTO);
        
        /// <summary>
        /// Raise event without args
        /// </summary>
        public static void Raise()
            => Binding.Raise(); 

        /// <summary>
        /// Raise both event with args and without args
        /// </summary>
        public static void RaiseBoth(T eventDto)
        {
            Binding.Raise(eventDto);
            Binding.Raise();
        }
        
        /// <summary>
        /// Clear all registered listeners
        /// </summary>
        public static void Clear()
            => Binding.Clear();
    }
}