using System;
using System.Reflection;
using H.NET.Core.Attributes;

namespace H.NET.Core.Extensions
{
    public static class ModuleExtensions
    {
        public static bool AllowMultipleInstance(this Type type) => 
            type.GetTypeInfo().GetCustomAttribute<AllowMultipleInstanceAttribute>() != null;

        public static bool AllowMultipleInstance(this IModule module) => 
            module.GetType().AllowMultipleInstance();

        public static bool AutoCreateInstance(this Type type)
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute<AllowMultipleInstanceAttribute>();

            return attribute?.AutoCreateInstance ?? true;
        }

        public static bool AutoCreateInstance(this IModule module) =>
            module.GetType().AutoCreateInstance();

    }
}
