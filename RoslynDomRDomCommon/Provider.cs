using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                       .Where(x => x.Namespace != null
                                 && x.Namespace.StartsWith("RoslynDom"));
         // TODO: *** Load other things, at least SameIntent and IWorker
         LoadIntoContainerWithArgument<IRDomFactory, RDomCorporation>(types, corporation);
         LoadIntoContainer<IContainerCheck>(types);
         LoadIntoContainer<IWorker>(types);
         //LoadIntoContainerWithArgument<ICreateFromWorker, RDomCorporation>(types, corporation);
         //LoadIntoContainerWithArgument<IBuildSyntaxWorker, RDomCorporation>(types, corporation);
         LoadIntoContainer<ISameIntent>(types);
         isLoaded = true;
      }

      [ExcludeFromCodeCoverage]
      private void AssertLoaded()
      {
         if (!isLoaded || unityContainer == null)
         {
            Guardian.Assert.AccessedProviderBeforeInitialization(typeof(Provider));
         }
      }

      internal IEnumerable<T> GetItems<T>(
          [CallerMemberName] string callerName = "",
          [CallerLineNumber] int callerLineNumber = 0)
      {
         AssertLoaded();
         return UnityContainer.ResolveAll<T>();
      }

      internal bool CheckContainer(
          [CallerMemberName] string callerName = "",
          [CallerLineNumber] int callerLineNumber = 0)
      {
         AssertLoaded();
         var containerChecks = UnityContainer.ResolveAll<IContainerCheck>();
         foreach (var check in containerChecks)
         {
            if (!check.ContainerCheck()) { return false; }
         }
         return true;
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
            AssertLoaded();
            return unityContainer;
         }
      }

   }
}
