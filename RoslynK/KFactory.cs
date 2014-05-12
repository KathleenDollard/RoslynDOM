using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynK.Implementations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace RoslynK
{
    public class KFactory
    {
        //private  Dictionary<Type, Type> _typeLookup;

        //private  Dictionary<Type, Type> TypeLookup
        //{
        //    get
        //    {
        //        if (_typeLookup == null)
        //        {
        //            _typeLookup = new Dictionary<Type, Type>();
        //            var thisAssembly = typeof(KBase).Assembly;
        //            var allTypeInfo = thisAssembly.GetTypes().Select(x => x.GetTypeInfo());
        //            var matchingTypesT = allTypeInfo
        //                        .Where(t => typeof(KBase).IsAssignableFrom(t));
        //            var matchingTypes = matchingTypesT.Where(t => !t.ContainsGenericParameters);
        //            foreach (var typeInfo in matchingTypes)
        //            {
        //                var rawItemType = GetRawItemType(typeInfo);
        //                if (rawItemType != null)
        //                {
        //                    _typeLookup.Add(rawItemType, typeInfo);
        //                }
        //            }

        //        }
        //        return _typeLookup;
        //    }
        //    }

        //private  Type GetRawItemType(System.Reflection.TypeInfo typeInfo)
        //{
        //    var bType = typeInfo.BaseType;
        //    while (bType != null)
        //    {
        //        var args = bType.GetGenericArguments();
        //        if (args.Count() > 0)
        //        { return args.First(); }
        //        bType = bType.BaseType;
        //    }
        //    return null;
        //}

        //private  KBase<T> Create<T>(T rawItem, params object[] moreParameters)
        //    where T : class
        //{
        //    Type type;
        //    if (TypeLookup.TryGetValue(typeof(T), out type))
        //    {
        //        // Expect this to be the only contructor because nothing else should construct these
        //        var constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();
        //        var newK = constructor.Invoke(new object[] { rawItem , moreParameters });
        //        return newK as KBase<T>;
        //    }
        //    throw new InvalidOperationException(type.FullName + "This type not supported");
        //}

        private IEnumerable<T> MakeList<T, TInput, TRaw>(TInput input,
             Func<TInput, IEnumerable<TRaw>> getItemsDeleg,
             Func<TRaw, T> makeNewItem)
        {
            var ret = new List<T>();
            foreach (var rawItem in getItemsDeleg(input))
            {
                var kItem = makeNewItem(rawItem);
                ret.Add(kItem);
            }
            return ret;
        }


        public ITreeWrapper CreateTreeWrapper(SyntaxTree tree)
        {
            var root = MakeRoot(tree);
            return new KTreeWrapper(tree, root);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="tree"></param>
       /// <returns></returns>
       /// <remarks>
       /// I'm not currently supporting attributes because I don't understand what attributes
       /// are on the compilation unit. Probably assembly attributes, but I want to get it first <br/>
       /// <br/>
       /// I currnelty think I will NOT support externs. They feel rare to me and something that the
       /// people who understand them can drop to the raw item to work with <br/>
       /// </remarks>
       private  KRoot MakeRoot(SyntaxTree tree)
        {
            // why are there attributes on a compilatoin unit?
            var kRoot = tree.GetRoot() as CompilationUnitSyntax;
            var members = MakeList(kRoot, x => x.Members, x => MakeStemMember(x));
            var usings = MakeList(kRoot, x => x.Usings, x => MakeUsingDirective(x));
            return new KRoot(kRoot, members, usings);
        }

        private IUsing MakeUsingDirective(UsingDirectiveSyntax x)
        {
            return new KUsingDirective(x);
        }

        private bool DoMember<T>(MemberDeclarationSyntax  val, Func<T, 
                IMember> doAction, out  IMember retValue)
            where T : class
        {
            var item = val as T;
            retValue = item != null ? doAction(item) : null;
            return (retValue != null);
        }

        /// <summary>
        /// Creates namespace and class
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private IStemMember MakeStemMember(MemberDeclarationSyntax rawMember)
        {
            var type = rawMember.GetType();
            IMember ret;
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMember<NamespaceDeclarationSyntax>(rawMember, MakeNamespace, out ret)){ }
            else if (DoMember<ClassDeclarationSyntax>(rawMember, MakeClass, out ret)) { }
            else if (DoMember<InterfaceDeclarationSyntax>(rawMember, MakeInterface, out ret)) { }
            else if (DoMember<StructDeclarationSyntax>(rawMember, MakeStructure, out ret)) { }
            else if (DoMember<EnumDeclarationSyntax>(rawMember, MakeEnum, out ret)) { }
            return ret as IStemMember;
        }

        /// <summary>
        /// Creates methods, properties and nested types
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private ITypeMember MakeTypeMember(MemberDeclarationSyntax rawMember)
        {
            var type = rawMember.GetType();
            IMember ret;
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMember<MethodDeclarationSyntax >(rawMember, MakeMethod, out ret)) { }
            else if (DoMember<FieldDeclarationSyntax>(rawMember, MakeField, out ret)) { }
            else if (DoMember<PropertyDeclarationSyntax>(rawMember, MakeProperty, out ret)) { }
            else if (DoMember<ClassDeclarationSyntax>(rawMember, MakeClass, out ret)) { }
            else if (DoMember<StructDeclarationSyntax>(rawMember, MakeStructure, out ret)) { }
            else if (DoMember<EnumDeclarationSyntax>(rawMember, MakeEnum, out ret)) { }
            return ret as ITypeMember;
        }
        private IMember MakeNamespace(NamespaceDeclarationSyntax rawNamespace)
        {
            var members = MakeList(rawNamespace, x => x.Members, x => MakeStemMember(x));
            var usings = MakeList(rawNamespace, x => x.Usings, x => MakeUsingDirective(x));
            return new KNamespace(rawNamespace, members, usings);
        }

        private IMember MakeClass(ClassDeclarationSyntax rawClass)
        {
            var members = MakeList(rawClass, x => x.Members, x => MakeTypeMember(x));
            return new KClass(rawClass, members);
        }

        private IMember MakeStructure(StructDeclarationSyntax rawStruct)
        {
            var members = MakeList(rawStruct, x => x.Members, x => MakeTypeMember(x));
            return new KStructure (rawStruct, members);
        }

        private IMember MakeInterface(InterfaceDeclarationSyntax rawInterface)
        {
            var members = MakeList(rawInterface, x => x.Members, x => MakeTypeMember(x));
            return new KInterface(rawInterface, members);
        }

        private IMember MakeEnum(EnumDeclarationSyntax rawEnum)
        {
            return new KEnum(rawEnum);
        }
        private IMember MakeMethod(MethodDeclarationSyntax rawMethod)
        {
            return new KMethod(rawMethod);
        }

        private IMember MakeProperty(PropertyDeclarationSyntax rawProperty)
        {
            return new KProperty(rawProperty );
        }

        private IMember MakeField(FieldDeclarationSyntax rawField)
        {
            return new KField(rawField);
        }
    }
}
