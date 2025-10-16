using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    // Only 1 injector
    public class DependencyInjector : MonoBehaviour
    {
        private DependencyInjector Instance;
        
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private readonly Dictionary<Type, object> registry = new();

        private void Awake()
        {
            Instance = this;

            var providers = FindAllMonoBehavioursInScene().OfType<IDependencyProvider>();
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }
        }

        private void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(Flags)
                .Where(m => m.GetCustomAttribute<ProvideAttribute>() != null);

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                if (returnType == typeof(void))
                {
                    Debug.LogWarning($"Provider method {method.Name} in {provider.GetType().Name} has void return type. Skipping.");
                    continue;
                }

                if (registry.ContainsKey(returnType))
                {
                    Debug.LogWarning($"Dependency of type {returnType.Name} is already registered. Skipping duplicate from {provider.GetType().Name}.{method.Name}.");
                    continue;
                }

                var providedInstance = method.Invoke(provider, null);
                if (providedInstance != null)
                {
                    registry[returnType] = providedInstance;
                    Debug.Log($"Registered dependency of type {returnType.Name} from {provider.GetType().Name}.{method.Name}");
                }
                else
                {
                    Debug.LogWarning($"Provider method {method.Name} in {provider.GetType().Name} returned null. Skipping.");
                }
            }
        }

        static MonoBehaviour[] FindAllMonoBehavioursInScene()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }

        
    }
}