using System;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public interface IDependencyProvider { }
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute { }
}