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
    public class FactoryProvider
    {
        private IUnityContainer unityContainer;
        internal bool isLoaded;

        internal void Initialize(IEnumerable<Tuple<Type, RDomFactoryHelper>> registrations)
        {
            isLoaded = true;
            ConfigureContainer(registrations);
        }

        public IEnumerable<IRDomFactory<TKind>> GetFactories<TKind>()
        {
            if (!isLoaded) throw new InvalidOperationException();
            return UnityContainer.ResolveAll<IRDomFactory<TKind>>();
        }

        private IUnityContainer UnityContainer
        {
            get
            {
                if (unityContainer == null) throw new InvalidOperationException();
                return unityContainer;
            }
        }

        private UnityContainer ConfigureContainer(IEnumerable<Tuple<Type, RDomFactoryHelper>> registrations)
        {
            // TODO: Work out a mechanism for people to add configuration
            var container = new UnityContainer();
            var types = AllClasses.FromAssembliesInBasePath();
            foreach (var registration in registrations)
            {
                var methodInfo = ReflectionUtilities.MakeGenericMethod(this.GetType(),
                    "LoadFactoriesIntoContainer", registration.Item1);
                methodInfo.Invoke(this, new object[] { types, container, null, registration.Item2 });
            }
            unityContainer = container;
            CheckContainer();
            return container;
        }

        //private UnityContainer ConfigureContainer()
        //{
        //    // TODO: Work out a mechanism for people to add configuration
        //    var container = new UnityContainer();
        //    var types = AllClasses.FromAssembliesInBasePath();
        //    LoadFactoriesIntoContainer<PublicAnnotation>(types, container, null, new RDomPublicAnnotationFactoryHelper());
        //    LoadFactoriesIntoContainer<IMisc>(types, container, typeof(RDomMiscFactory<,>), RDomFactoryHelper.MiscFactoryHelper);
        //    LoadFactoriesIntoContainer<IExpression>(types, container, typeof(RDomExpressionFactory<,>), RDomFactoryHelper.ExpressionFactoryHelper);
        //    LoadFactoriesIntoContainer<IStatement>(types, container, typeof(RDomStatementFactory<,>), RDomFactoryHelper.StatementFactoryHelper);
        //    LoadFactoriesIntoContainer<ITypeMember>(types, container, typeof(RDomTypeMemberFactory<,>), RDomFactoryHelper.TypeMemberFactoryHelper);
        //    LoadFactoriesIntoContainer<IStemMember>(types, container, typeof(RDomStemMemberFactory<,>), RDomFactoryHelper.StemMemberFactoryHelper);
        //    LoadFactoriesIntoContainer<IRoot>(types, container, typeof(RDomRootContainerFactory<,>), RDomFactoryHelper.RootFactoryHelper);
        //    unityContainer = container;
        //    CheckContainer();
        //    return container;
        //}
        private void CheckContainer()
        {
            Contract.Assert(2 <= GetFactories<IMisc>().Count());
            Contract.Assert(0 <= GetFactories<IExpression>().Count());
            Contract.Assert(3 <= GetFactories<IStatement>().Count());
            Contract.Assert(6 <= GetFactories<ITypeMember>().Count());
            Contract.Assert(6 <= GetFactories<IStemMember>().Count());
            Contract.Assert(1 <= GetFactories<IRoot>().Count());

        }
        private void LoadFactoriesIntoContainer<TKind>(IEnumerable<Type> types,
                    UnityContainer container,
                    Type rDomFactoryBase,
                    RDomFactoryHelper helper)
        {
            var factoryType = typeof(IRDomFactory<TKind>);
            var handledTypes = new List<Type>();
            var typesToHandle = new List<Type>();
            RegisterExplicitFactories<TKind>(types, container, factoryType, handledTypes, typesToHandle);
            RegisterDefaultFactories<TKind>(container, rDomFactoryBase, handledTypes, typesToHandle);
        }

        private void RegisterExplicitFactories<TKind>(IEnumerable<Type> types, UnityContainer container, Type factoryType, List<Type> handledTypes, List<Type> typesToHandle)
        {
            foreach (var type in types)
            {
                if (factoryType.IsAssignableFrom(type))
                {
                    var name = type.FullName;
                    if (ShouldRegister<TKind>(container, type, name))
                    {
                        RegisterFactory<TKind>(container, type, name);
                        RecordAsHandled(handledTypes, type);
                    }
                }
                else if (typeof(TKind).IsAssignableFrom(type))
                { typesToHandle.Add(type); }
            }
        }

        private void RegisterDefaultFactories<TKind>(UnityContainer container, Type rDomFactoryBase, List<Type> handledTypes, List<Type> typesToHandle)
        {
            // TODO: Not currently handling public annotations, but need to do this so manage alternate public annotation styles
            if (rDomFactoryBase != null)
            {
                var typesNotHandled = typesToHandle.Except(handledTypes);
                foreach (var type in typesNotHandled)
                {
                    var name = type.FullName;
                    var syntaxType = RoslynDomUtilities.FindFirstSyntaxNodeType(type);
                    if (syntaxType == null) { continue; } // this happens with referenced types
                    var newType = ReflectionUtilities.MakeGenericType(rDomFactoryBase, type, syntaxType);
                    RegisterFactory<TKind>(container, newType, name);
                }
            }
        }


        private void RecordAsHandled(List<Type> handledTypes, Type type)
        {
            if (type.BaseType.IsConstructedGenericType)
            {
                var handledType = type.BaseType.GenericTypeArguments.First();
                handledTypes.Add(handledType);
            }
        }

        private bool ShouldRegister<TKind>(UnityContainer container, Type type, string name)
        {
            if (type.IsGenericTypeDefinition) { return false; }
            if (container.IsRegistered<IRDomFactory<TKind>>(name))
            {
                var oldType = container.Resolve<IRDomFactory<TKind>>(name).GetType();
                if (GetPriority(type) < GetPriority(oldType)) { return false; }
                return true;
            }
            return true;
        }

        private void RegisterFactory<TKind>(UnityContainer container, Type type, string name)
        {
            container.RegisterType(typeof(IRDomFactory<TKind>), type, name,
                     new ContainerControlledLifetimeManager(),
                     new InjectionMember[] { });
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
