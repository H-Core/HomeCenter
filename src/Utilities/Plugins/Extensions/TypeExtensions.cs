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
                .Any(constructorInfo => 
                    constructorInfo.IsPublic && 
                    !constructorInfo.GetParameters().Any());
        }
    }
}
