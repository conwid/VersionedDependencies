using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.VersionedDependencies
{
    public interface IVersionService
    {
        string DefaultVersion { get; }
        string CurrentVersion { get; }
    }
}
