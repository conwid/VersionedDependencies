using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.VersionedDependencies
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceVersionAttribute : Attribute
    {
        public string Version { get; set; }
        public Type Type { get; set; }
    }
}
