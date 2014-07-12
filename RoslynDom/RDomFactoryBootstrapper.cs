using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace RoslynDom
{
    public class RDomFactoryBootstrapper
    {
        public  UnityContainer ConfigureContainer()
        {
            // TODO: Work out a mechanism for people to add configuration
            var container = new UnityContainer();
            var types = AllClasses.FromLoadedAssemblies();
            foreach (var type in types)
            {
                var name = type.FullName;
                if (typeof(IRDomFactory).IsAssignableFrom(type))
                {
                    if (container.IsRegistered<IRDomFactory>(name))
                    {
                        var oldType = container.Resolve<IRDomFactory>(name).GetType();
                        if (GetPriority(type) < GetPriority(oldType)) continue;
                    }
                    container.RegisterType(typeof(IRDomFactory), type, name);
                }
            }
            return container;
        }

        private  FactoryPriority GetPriority(Type type)
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
