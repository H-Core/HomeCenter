using System.Collections.Generic;

namespace H.NET.Plugins
{
    public class InstanceNotFoundException : KeyNotFoundException
    {
        public InstanceNotFoundException(string name) : base($"Instance {name} is not found in the instances file")
        {
        }
    }
}