using Autofac.Builder;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.VersionedDependencies
{
    public static class AutofacVersionedExtensions
    {
        public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterVersioned<T>(this ContainerBuilder builder)
        {
            List<Parameter> parameters = CreateParameterList<T>();

            var versionAttribute = typeof(T).GetCustomAttribute<ServiceVersionAttribute>();
            if (versionAttribute != null)
            {
                return builder.RegisterType<T>().Keyed(versionAttribute.Version, versionAttribute.Type).WithParameters(parameters.ToArray());
            }
            return builder.RegisterType<T>().WithParameters(parameters.ToArray());
        }

        public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterVersioned<T, TService>(this ContainerBuilder builder)
        {
            List<Parameter> parameters = CreateParameterList<T>();
            return builder.RegisterType<T>().As<TService>().WithParameters(parameters.ToArray());
        }
        private static List<Parameter> CreateParameterList<T>()
        {
            var ctor = typeof(T).GetConstructors().Where(c => c.IsPublic).OrderByDescending(t => t.GetParameters().Count()).First();
            List<Parameter> parameters = new List<Parameter>();
            foreach (var parameter in ctor.GetParameters())
            {
                parameters.Add(new ResolvedParameter((_, __) => true,
                                                     (pi, ctx) => ResolveWithVersioning(pi, ctx)
                                              ));
            }
            return parameters;
        }

        private static object ResolveWithVersioning(ParameterInfo pi, IComponentContext ctx)
        {
            var versionService = ctx.Resolve<IVersionService>();
            string version = versionService.CurrentVersion ?? versionService.DefaultVersion;

            object result;
            if (ctx.TryResolveKeyed(version, pi.ParameterType, out result))
            {
                return result;
            }
            return ctx.Resolve(pi.ParameterType);
        }
    }
}
