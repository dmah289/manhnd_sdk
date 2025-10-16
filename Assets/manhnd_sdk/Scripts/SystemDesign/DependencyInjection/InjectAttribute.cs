using System;

namespace manhnd_sdk.Scripts.SystemDesign.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute
    {
        public InjectAttribute() {}
    }
}