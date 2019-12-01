using System;
using System.Linq;

namespace H.NET.Utilities.Plugins.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasParameterlessConstructor(this Type type)
        {
            return type
                .GetConstructors()
                .Any(c => c.IsPublic && 
                          c.GetParameters().Length == 0);
        }
    }
}
