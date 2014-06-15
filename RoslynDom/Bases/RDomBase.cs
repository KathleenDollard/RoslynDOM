using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary>
    /// Base class for Roslyn Dom navigation tree
    /// </summary>
    public abstract class RDomBase : IRoslynDom
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Return type is object, not SyntaxNode to match interface
        /// </remarks>
        public abstract object RawItem { get; }

        /// <summary>
        /// For a discussion of names <see cref="OuterName"/>
        /// </summary>
        /// <returns>The string name, same as Roslyn symbol's name</returns>
        public abstract string Name { get; }

        /// <summary>
        /// <para>
        /// The name is the local name. The best name 
        /// is the name you are most likely to use inside the assembly and 
        /// outside the current type. This name is also legal inside the type.
        ///  </para><para>
        ///    - namespace: The full namespace, regardless of declaration nesting
        ///  </para><para>
        ///    - nested types: The type name plus the outer type name
        ///  </para><para>
        ///    - static members: The member name plus the containing type name
        ///  </para><para>
        ///    - root and usings: empty string
        ///  </para><para>
        ///    - other symbols: The symbol name
        /// </para>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///  <para>
        /// Names are complicated. These are the distinctions for the RoslynDom
        ///  <para>
        ///    - Name: The local name, and same as the semantic tree symbol name
        ///  </para><para>
        ///    - OuterName: Described above
        ///  </para><para>
        ///    - QualifiedName: The name, nesting types, and namespace
        ///  </para><para>
        /// Part of the driver for this is removing differences based on namespace
        /// nesting and other layout details that are irrelevant to meaning. However, 
        /// it seemed unfriendly to have a name that differed from the Roslyn symbol name, 
        /// so namespaces are the immediate name, not the current namespace name. Thus
        /// Name on namespaces should be used with caution. Either use the Namespace property 
        /// on another item, or use the OuterName in almost all cases.
        ///  </para><para>
        /// NOTE: Naming for generics is not yet included. 
        ///  </para> 
        /// </remarks>
        public abstract string OuterName { get; }

        /// <summary>
        /// For a discussion of names <see cref="OuterName"/>
        /// </summary>
        /// <returns>The qualified name, containing namespaces, containing types, and name</returns>
        public abstract string QualifiedName { get; }

        /// <summary>
        /// The contaning namespace.
        /// </summary>
        /// <returns>The current namespace, empty string for the root, and 
        /// containing namespace for namespaces (not including the current namespace)
        /// </returns>
        public abstract string Namespace { get; }

        public abstract ISymbol Symbol { get; }

        public abstract object RequestValue( string name);
    }

    public abstract class RDomBase<T, TSymbol> : RDomBase, IRoslynDom<T, TSymbol>
        where T : SyntaxNode
        where TSymbol : ISymbol
    {
        private T _rawSyntax;
        private TSymbol _symbol;
        private IEnumerable<IAttribute> _attributes;

        protected RDomBase(T rawItem)
        {
            _rawSyntax = rawItem;
        }

        public T TypedSyntax
        {
            get
            { return _rawSyntax; }
        }

        public override object RawItem
        {
            get
            { return _rawSyntax; }
        }

        public override ISymbol Symbol
        {
            get
            {
                return TypedSymbol;
            }
        }

        public virtual TSymbol TypedSymbol
        {
            get
            {
                if (_symbol == null)
                { _symbol = GetSymbol(TypedSyntax); }
                return _symbol;
            }
        }

        public override string Name
        {
            get
            {
                return Symbol.Name;
            }
        }

        public override string OuterName
        {
            get
            {
                // namespace overrides this
                var typeName = GetContainingTypeName(Symbol.ContainingType);
                return (string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".") +
                       Name;
            }
        }


        public override string QualifiedName
        {
            get
            {
                var namespaceName = GetContainingNamespaceName(Symbol.ContainingNamespace);
                var typeName = GetContainingTypeName(Symbol.ContainingType);
                namespaceName = string.IsNullOrWhiteSpace(namespaceName) ? "" : namespaceName + ".";
                typeName = string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".";
                return namespaceName + typeName + Name;
            }
        }

        public override string Namespace
        {
            get
            {
                return GetContainingNamespaceName(Symbol.ContainingNamespace);
                //var namespaceName = GetContainingNamespaceName(Symbol.ContainingNamespace);
                //var typeName = GetContainingTypeName(Symbol.ContainingType);
                //return (string.IsNullOrWhiteSpace(namespaceName) ? "" : namespaceName + ".") +
                //       (string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".") +
                //       Name;
            }
        }

        internal string GetContainingNamespaceName(INamespaceSymbol nspaceSymbol)
        // TODO: Change to assembly protected when it is available
        {
            if (nspaceSymbol == null) return "";
            var parentName = GetContainingNamespaceName(nspaceSymbol.ContainingNamespace);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                nspaceSymbol.Name;
        }

        private string GetContainingTypeName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null) return "";
            var parentName = GetContainingTypeName(typeSymbol.ContainingType);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                typeSymbol.Name;
        }

        private SemanticModel GetModel()
        // TODO: Change this scope to assembly protected as soon as it is available
        {
            var tree = TypedSyntax.SyntaxTree;
            var compilation = CSharpCompilation.Create("MyCompilation",
                                           options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                           syntaxTrees: new[] { tree },
                                           references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);
            return model;

        }

        private MetadataReference mscorlib;
        private MetadataReference Mscorlib
        {
            get
            {
                if (mscorlib == null)
                {
                    mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
                }

                return mscorlib;
            }
        }


        protected TSymbol GetSymbol(SyntaxNode node)
        {
            var model = GetModel();
            var symbol =  (TSymbol)model.GetDeclaredSymbol(node);
            return symbol;
        }

        protected IEnumerable<IAttribute > GetAttributes()
        {
            if (_attributes == null)
            {
                _attributes = RDomAttribute.MakeAttributes(Symbol, TypedSyntax);
            }
            return _attributes;
        }

        protected Microsoft.CodeAnalysis.TypeInfo GetTypeInfo(SyntaxNode node)
        {
            var model = GetModel();
            var tyypeInfo = model.GetTypeInfo(node);
            return tyypeInfo;
        }

        /// <summary>
        /// Fallback for getting requested values. 
        /// <br/>
        /// For special values (those that don't just return a property) override
        /// this method, return the approparite value, and olny call this base method when needed
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object RequestValue( string name)
        {
            if (ReflectionUtilities.CanGetProperty(this, name))
            {
                var value = ReflectionUtilities.GetPropertyValue(this, name);
                return value;
            }
            return null;
        }
    }

    public abstract class RDomSyntaxNodeBase<T, TSymbol> : RDomBase<T, TSymbol>
        where T : SyntaxNode
        where TSymbol : ISymbol
    {
        // TODO: Consider why this isn't collapsed into the RDomBase<T>
        private T _rawItem;

        protected RDomSyntaxNodeBase(T rawItem) : base(rawItem)
        {
            _rawItem = rawItem;
        }

    }
}
