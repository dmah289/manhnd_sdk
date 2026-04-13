using System;
using System.Collections.Generic;

namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    /// <summary>
    /// Create a concrete event "thread" respectively to data transfer object type
    /// </summary>
    /// <typeparam name="T">Data transfer object for this "thread"</typeparam>
    public static class EventBus<T> where T : IEventDTO
    {
        private static readonly IEventBinding<T> Binding = new EventBinding<T>();

        /// <summary>
        /// Event listeners register
        /// </summary>
        public static void Register(Action onEventWithoutArgs = null,
                                    Action<T> onEventWithArgs = null)
        {
            Binding.Add(onEventWithoutArgs);
            
            if(onEventWithArgs != null)
                Binding.Add(onEventWithArgs);
        }
        
        /// <summary>
        /// Event listeners deregister
        /// </summary>
        public static void Deregister(Action onEventWithoutArgs = null,
                                      Action<T> onEventWithArgs = null)
        {
            Binding.Remove(onEventWithArgs);
            
            if(onEventWithoutArgs != null)
                Binding.Remove(onEventWithoutArgs);
        }
        
        /// <summary>
        /// Subject raise event with args
        /// </summary>
        public static void Raise(T eventDTO)
            => Binding.Raise(eventDTO);
        
        /// <summary>
        /// Subject raise event without args
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