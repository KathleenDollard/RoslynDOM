using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.BasesAndBaseHelpers;
using RoslynDom.Common;

namespace RoslynDom
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RDomBase<T, TSyntax, TSymbol> : RDomBase<T>, IRoslynDom<T, TSyntax, TSymbol>
          where TSyntax : SyntaxNode
          where TSymbol : ISymbol
          where T : class, IDom<T>
    {
        private TSyntax _originalRawSyntax;
        private TSyntax _rawSyntax;
        private TSymbol _symbol;
        private IEnumerable<IAttribute> _attributes;

        protected RDomBase(TSyntax rawItem, params PublicAnnotation[] publicAnnotations)
            : base(publicAnnotations)
        {
            _rawSyntax = rawItem;
            _originalRawSyntax = rawItem;
        }

        protected RDomBase(T oldIDom)
             : base(oldIDom)
        {
            var oldRDom = oldIDom as RDomBase<T, TSyntax, TSymbol>;
            _rawSyntax = oldRDom._rawSyntax;
            _originalRawSyntax = oldRDom._originalRawSyntax;
            if (oldRDom._attributes != null)
            { _attributes = RDomBase<IAttribute>.CopyMembers(oldRDom._attributes.Cast<RDomAttribute>()); }
            Name = oldRDom.Name;
            //_symbol = default(TSymbol); // this should be reset, this line is to remind us
        }

        protected override void Initialize()
        {
            if (TypedSymbol != null)
            {
                Name = TypedSymbol.Name;
            }
            if (this is IHasAttributes)
            {

            }
        }

        protected override bool CheckSameIntent(T other, bool includePublicAnnotations)
        {
            if (!base.CheckSameIntent(other, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers<T, TSyntax, TSymbol>.CheckSameIntent(this as T, other, includePublicAnnotations)) { return false; };
            return true;
        }

        public TSyntax TypedSyntax
        { get { return _rawSyntax; } }

        protected TSyntax OriginalTypedSyntax
        { get { return _originalRawSyntax; } }

        public override object RawItem
        { get { return _rawSyntax; } }

        public override object OriginalRawItem
        { get { return _originalRawSyntax; } }

        public override ISymbol Symbol
        { get { return TypedSymbol; } }

        public virtual TSymbol TypedSymbol
        {
            get
            {
                if (_symbol == null)
                { _symbol = (TSymbol)GetSymbol(TypedSyntax); }
                return _symbol;
            }
        }

        public override string OuterName
        {
            get
            {
                if (Symbol == null) { return Name; }
                var typeName = GetContainingTypeName(Symbol.ContainingType);
                return (string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".") +
                       Name;
            }
        }

        internal virtual string GetQualifiedName()
        {
            var item = this as IHasNamespace;
            if (item != null)
            {
                var typeName = GetContainingTypeName(Symbol.ContainingType);
                var outerName = (string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".") +
                       Name;
                // There are probably slightly cleaner ways to do this, but with some override scenarios
                // directly calling OuterName will result in a StackOverflowException
                return (string.IsNullOrWhiteSpace(item.Namespace) ? "" : item.Namespace + ".")
                          + outerName;
            }
            throw new InvalidOperationException();
        }

        internal virtual string GetNamespace()
        {
            if (Symbol == null) { return ""; }
            return RoslynDomUtilities.GetContainingNamespaceName(Symbol.ContainingNamespace);
        }

        private static string GetContainingTypeName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null) return "";
            var parentName = GetContainingTypeName(typeSymbol.ContainingType);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                typeSymbol.Name;
        }

        private SemanticModel GetModel()
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

        internal override ISymbol GetSymbol(SyntaxNode node)
        {
            var model = GetModel();
            var symbol = (TSymbol)model.GetDeclaredSymbol(node);
            return symbol;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected IEnumerable<IAttribute> GetAttributes()
        {
            if (_attributes == null)
            {
                _attributes = RDomAttribute.MakeAttributes(this, Symbol, TypedSyntax);
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
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public override object RequestValue(string propertyName)
        {
            if (ReflectionUtilities.CanGetProperty(this, propertyName))
            {
                var value = ReflectionUtilities.GetPropertyValue(this, propertyName);
                return value;
            }
            return null;
        }


    }


}
