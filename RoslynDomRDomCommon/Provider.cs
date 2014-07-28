using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class Provider
    {
        private IUnityContainer unityContainer = new UnityContainer();
        internal bool isLoaded;

        internal void ConfigureContainer(RDomCorporation corporation)
        {
            var types = AllClasses.FromAssembliesInBasePath()
                          .Where(x => x.Namespace.StartsWith("RoslynDom"));
            // TODO: *** Load other things, at least SameIntent and IWorker
            LoadIntoContainerWithArgument<IRDomFactory, RDomCorporation >(types, corporation);
            LoadIntoContainer<IContainerCheck>(types);
            LoadIntoContainerWithArgument<ICreateFromWorker, RDomCorporation>(types, corporation);
            LoadIntoContainerWithArgument<IBuildSyntaxWorker, RDomCorporation>(types, corporation);
            LoadIntoContainer<ISameIntent>(types);
            isLoaded = true;
        }

        internal IEnumerable<IRDomFactory> GetFactories()
        {
            if (!isLoaded) throw new InvalidOperationException();
            return UnityContainer.ResolveAll<IRDomFactory>();
        }

        internal ICreateFromWorker GetCreateFromWorker()
        {
            if (!isLoaded) throw new InvalidOperationException();
            return UnityContainer.ResolveAll<ICreateFromWorker>()
                        .OrderByDescending(x => x.Priority)
                        .First();
        }

        internal IBuildSyntaxWorker GetBuildSyntaxWorker()
        {
            if (!isLoaded) throw new InvalidOperationException();
            return UnityContainer.ResolveAll<IBuildSyntaxWorker>()
                        .OrderByDescending(x => x.Priority)
                        .First();
        }

        internal void CheckContainer()
        {
            if (!isLoaded) throw new InvalidOperationException();
            var containerChecks = UnityContainer.ResolveAll<IContainerCheck>();
            foreach (var check in containerChecks)
            {
                if (!check.ContainerCheck()) { throw new InvalidOperationException(); }
            }
        }

        private void LoadIntoContainer<T>(
                      IEnumerable<Type> types)
        {
            var factoryType = typeof(T);
            foreach (var type in types)
            {
                if (factoryType.IsAssignableFrom(type))
                {
                    unityContainer.RegisterType(factoryType, type, type.FullName,
                               new ContainerControlledLifetimeManager(),
                               new InjectionMember[] { });
                }
            }
        }

        private void LoadIntoContainerWithArgument<T, TArg>(
                      IEnumerable<Type> types,
                      TArg argument)
        {
            var factoryType = typeof(T);
            foreach (var type in types)
            {
                if (factoryType.IsAssignableFrom(type))
                {
                    unityContainer.RegisterType(factoryType, type, type.FullName,
                               new ContainerControlledLifetimeManager(),
                               new InjectionMember[] { new InjectionConstructor(argument) });
                }
            }
        }

        private IUnityContainer UnityContainer
        {
            get
            {
                if (unityContainer == null) throw new InvalidOperationException();
                return unityContainer;
            }
        }

    }
}
