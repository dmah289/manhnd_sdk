using System;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    public interface IDependencyProvider { }
    
    // why don't use for class instead of method and interface-based searching?
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute { }
}