using System;

namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    public interface IEventDTO { }
    
    public interface IEventBinding<T> where T : IEventDTO
    {
        void Add(Action onEventWithoutArgs);
        void Remove(Action onEventWithoutArgs);
        void Add(Action<T> onEventWithArgs);
        void Remove(Action<T> onEventWithArgs);
        void Raise(T value);
        void Raise();
        void Clear();
    }
    
    public class EventBinding<T> : IEventBinding<T> where T : IEventDTO
    {
        event Action OnEventWithoutArgs = () => { };
        event Action<T> OnEventWithArgs = _ => { };
        
        public void Add(Action onEventWithoutArgs)
            => OnEventWithoutArgs += onEventWithoutArgs;
        
        public void Remove(Action onEventWithoutArgs)
            => OnEventWithoutArgs -= onEventWithoutArgs;
        
        public void Add(Action<T> onEventWithArgs)
            => OnEventWithArgs += onEventWithArgs;
        
        public void Remove(Action<T> onEventWithArgs)
            => OnEventWithArgs -= onEventWithArgs;
        
        public void Raise()
            => OnEventWithoutArgs?.Invoke();

        public void Raise(T dto)
            => OnEventWithArgs?.Invoke(dto);
        
        public void Clear()
        {
            OnEventWithoutArgs = null;
            OnEventWithArgs = null;
        }
    }
}