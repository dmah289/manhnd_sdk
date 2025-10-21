using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    [DefaultExecutionOrder(-10000)]
    public class DependencyInjector : MonoBehaviour
    {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private readonly Dictionary<Type, object> registry = new();

        private void Awake()
        {
            var providers = FindAllMonoBehaviours().OfType<IDependencyProvider>();
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }
            
            var injectables = FindAllMonoBehaviours().Where(IsInjectable);
            foreach (var injectable in injectables)
            {
                HandleInjecting(injectable);
            }
        }

        #region Handle Injecting
        private bool IsInjectable(MonoBehaviour instance)
        {
            MemberInfo[] members = instance.GetType().GetMembers(Flags);
            return members.Any(m => Attribute.IsDefined(m, typeof(InjectAttribute)));
        }

        public object Resolve(Type type)
        {
            registry.TryGetValue(type, out var resolvedInstance);
            return resolvedInstance;
        }

        public void HandleInjecting(MonoBehaviour instance)
        {
            Type type = instance.GetType();
            
            HandleInjectFields(instance, type);
            HandleInjectMethods(instance, type);
        }

        private void HandleInjectFields(MonoBehaviour instance, Type type)
        {
            var injectableFields = type.GetFields(Flags).Where( m => Attribute.IsDefined(m, typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields)
            {
                object resolvedInstance = Resolve(injectableField.FieldType);
                if (resolvedInstance == null)
                    throw new Exception($"No registered dependency for type {injectableField.FieldType.Name} found for injection into field {injectableField.Name} of {type.Name}.");
                
                injectableField.SetValue(instance, resolvedInstance);
            }
        }
        
        private void HandleInjectMethods(MonoBehaviour instance, Type type)
        {
            var injectableMethods = type.GetMethods(Flags).Where( m => Attribute.IsDefined(m, typeof(InjectAttribute)));

            foreach (MethodInfo injectableMethod in injectableMethods)
            {
                Type[] requiredParameters = injectableMethod.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToArray();

                object[] resolvedInstances = requiredParameters.Select(Resolve).ToArray();

                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
                {
                    throw new Exception($"Failed to inject {type.Name}.{injectableMethod.Name}");
                }
                
                injectableMethod.Invoke(instance, resolvedInstances);
            }
        }
        #endregion

        #region Handle Registering

        private MonoBehaviour[] FindAllMonoBehaviours()
            => FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);

        private void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(Flags)
                .Where(m => m.GetCustomAttribute<ProvideAttribute>() != null);

            foreach (MethodInfo method in methods)
            {
                Type returnType = method.ReturnType;

                if (registry.ContainsKey(returnType))
                {
                    Debug.LogWarning($"Dependency of type {returnType.Name} is already registered. Skipping duplicate from {provider.GetType().Name}.{method.Name}.");
                    continue;
                }

                object providedInstance = method.Invoke(provider, null);
                if (providedInstance != null) registry[returnType] = providedInstance;
                else
                {
                    Debug.LogWarning($"Provider method {method.Name} in {provider.GetType().Name} returned null. Skipping.");
                }
            }
        }

        #endregion
    }
}