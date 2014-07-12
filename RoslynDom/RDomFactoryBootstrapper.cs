using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomFactoryBootstrapper
    {
        public UnityContainer ConfigureContainer()
        {
            // TODO: Work out a mechanism for people to add configuration
            var container = new UnityContainer();
            var types = AllClasses.FromLoadedAssemblies();
            LoadContainer<IPublicAnnotationFactory>(types, container, new RDomPublicAnnotationFactoryHelper(container));
            LoadContainer<IRDomFactory<IStatement>>(types, container, new RDomStatementFactoryHelper(container));
            return container;
        }

        private void LoadContainer<T>(IEnumerable<Type> types, UnityContainer container, RDomFactoryHelper helper)
        {
            foreach (var type in types)
            {
                var name = type.FullName;
                if (typeof(T).IsAssignableFrom(type))
                {
                    if (container.IsRegistered<T>(name))
                    {
                        var oldType = container.Resolve<T>(name).GetType();
                        if (GetPriority(type) < GetPriority(oldType)) continue;
                    }
                    container.RegisterType(typeof(T), type, name,
                        new ContainerControlledLifetimeManager(),
                        new InjectionMember[] { new InjectionConstructor(helper) });
                }
            }
        }

        private FactoryPriority GetPriority(Type type)
        {
            var propInfo = type
                .GetTypeInfo()
                .DeclaredProperties
                .Where(prop => prop.Name == "Priority")
                .FirstOrDefault();
            if (propInfo == null) return FactoryPriority.None;
            return (FactoryPriority)propInfo.GetValue(null);
        }

    }
}
