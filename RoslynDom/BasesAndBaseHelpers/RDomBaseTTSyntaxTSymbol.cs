using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RDomBase<T, TSymbol> : RDomBase<T>, IRoslynDom<T, TSymbol>
          where TSymbol : ISymbol
          where T : class, IDom<T>
    {
        private SyntaxNode _originalRawSyntax;
        private SyntaxNode _rawSyntax;
        private TSymbol _symbol;
        //private AttributeList _attributes = new AttributeList();
        private string _containingTypeName;

        protected RDomBase(SyntaxNode rawItem, IDom parent, SemanticModel model)
             : base(RDomFactoryHelper.GetPublicAnnotations(rawItem, parent))
        {
            _rawSyntax = rawItem;
            _originalRawSyntax = rawItem;
            Parent = parent;
            if (model != null)
            {
                _symbol = (TSymbol)model.GetDeclaredSymbol(rawItem);
                //if (_symbol != null)
                //{
                //    _attributes = RDomAttribute.MakeAttributes(this, Symbol, _rawSyntax, model);
                //}
            }
        }

        protected RDomBase(T oldIDom)
            : base(oldIDom)
        {
            var oldRDom = oldIDom as RDomBase<T, TSymbol>;
            _rawSyntax = oldRDom._rawSyntax;
            _originalRawSyntax = oldRDom._originalRawSyntax;
            _symbol = oldRDom._symbol;
            //if (oldRDom._attributes != null)
            //{ _attributes = RoslynDomUtilities.CopyMembers(oldRDom._attributes); }
            Name = oldRDom.Name;
            //_symbol = default(TSymbol); // this should be reset, this line is to remind us
        }

        //protected SyntaxList<AttributeListSyntax> BuildAttributeListSyntax()
        //{
        //    var list = SyntaxFactory.List<AttributeListSyntax>();
        //    if (GetAttributes().Any())
        //    {
        //        var attribList = SyntaxFactory.AttributeList();
        //        var attributes = _attributes.Select(x => ((RDomAttribute)x).BuildSyntax());
        //        attribList = attribList.AddAttributes(attributes.ToArray());
        //        list = list.Add(attribList);
        //    }
        //    return list;
        //}


        public SyntaxNode TypedSyntax
        { get { return _rawSyntax; } }

        protected SyntaxNode OriginalTypedSyntax
        { get { return _originalRawSyntax; } }

        public override object RawItem
        { get { return _rawSyntax; } }

        public override object OriginalRawItem
        { get { return _originalRawSyntax; } }

        public override ISymbol Symbol
        { get { return TypedSymbol; } }

        public virtual TSymbol TypedSymbol
        { get { return _symbol; } }

        public override string OuterName
        {
            get
            {
                return (string.IsNullOrWhiteSpace(_containingTypeName) ? "" : _containingTypeName + ".") +
                       Name;
            }
        }

        protected virtual string GetQualifiedName()
        {
            var item = this as IHasNamespace;
            if (item != null)
            {
                var outerName = (string.IsNullOrWhiteSpace(_containingTypeName) ? "" : _containingTypeName + ".") +
                       Name;
                // There are probably slightly cleaner ways to do this, but with some override scenarios
                // directly calling OuterName will result in a StackOverflowException
                return (string.IsNullOrWhiteSpace(item.Namespace) ? "" : item.Namespace + ".")
                          + outerName;
            }
            // The following line should not be reachable, so is not tested
            throw new InvalidOperationException();
        }


        protected virtual AccessModifier GetAccessibility()
        {
            if (Symbol == null) { return AccessModifier.NotApplicable; }
            return (AccessModifier)Symbol.DeclaredAccessibility;
        }

        private static string GetContainingTypeName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null) return "";
            var parentName = GetContainingTypeName(typeSymbol.ContainingType);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                typeSymbol.Name;
        }

        //private SemanticModel GetModel()
        //{
        //    var tree = _rawSyntax.SyntaxTree;
        //    var compilation = CSharpCompilation.Create("MyCompilation",
        //                                   options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
        //                                   syntaxTrees: new[] { tree },
        //                                   references: new[] { Mscorlib });
        //    var model = compilation.GetSemanticModel(tree);
        //    return model;

        //}

        //private MetadataReference mscorlib;
        //private MetadataReference Mscorlib
        //{
        //    get
        //    {
        //        if (mscorlib == null)
        //        {
        //            mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
        //        }

        //        return mscorlib;
        //    }
        //}

        //protected override ISymbol GetSymbol(SyntaxNode node)
        //{
        //    var model = GetModel();
        //    var symbol = (TSymbol)model.GetDeclaredSymbol(node);
        //    return symbol;
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        //protected IEnumerable<IAttribute> GetAttributes()
        //{ return _attributes; }

        //protected Microsoft.CodeAnalysis.TypeInfo GetTypeInfo(SyntaxNode node)
        //{
        //    var model = GetModel();
        //    var tyypeInfo = model.GetTypeInfo(node);
        //    return tyypeInfo;
        //}

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
