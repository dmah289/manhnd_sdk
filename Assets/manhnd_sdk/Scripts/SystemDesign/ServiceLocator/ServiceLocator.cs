using System;
using System.Collections.Generic;

namespace manhnd_sdk.Scripts.SystemDesign.ServiceLocator
{
    public class ServiceLocator
    {
        readonly Dictionary<Type, object> _services = new();
    }
}