using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
        private string _containingTypeName;

        protected RDomBase(SyntaxNode rawItem, IDom parent, SemanticModel model)
             : base(RDomFactoryHelper.GetPublicAnnotations(rawItem, parent, model))
        {
            _rawSyntax = rawItem;
            _originalRawSyntax = rawItem;
            Parent = parent;
            if (model != null)
            {
                _symbol = (TSymbol)model.GetDeclaredSymbol(rawItem);
                if (_symbol != null)
                {
                    var thisAsHasStructuredDocs = this as IHasStructuredDocumentation;
                    if (thisAsHasStructuredDocs != null)
                    {
                        var docsItem = RDomFactoryHelper.GetStructuredDocumentation(rawItem, parent, model).FirstOrDefault();
                        var docs = Symbol.GetDocumentationCommentXml();
                        if (!string.IsNullOrWhiteSpace(docs))
                        {
                            var xDocument = XDocument.Parse(docs);
                            docsItem.RawItem = xDocument;
                            var summaryNode = xDocument.DescendantNodes()
                                                .OfType<XElement>()
                                                .Where(x => x.Name == "summary")
                                                .Select(x => x.Value);
                            var description = summaryNode.FirstOrDefault().Replace("/r", "").Replace("\n", "").Trim();
                            docsItem.Description = description;
                            thisAsHasStructuredDocs.StructuredDocumentation = docsItem;
                            thisAsHasStructuredDocs.Description = description;
                        }
                    }
                }
            }
        }

        protected RDomBase(T oldIDom)
            : base(oldIDom)
        {
            var oldRDom = oldIDom as RDomBase<T, TSymbol>;
            _rawSyntax = oldRDom._rawSyntax;
            _originalRawSyntax = oldRDom._originalRawSyntax;
            _symbol = oldRDom._symbol;
            // TODO: SameIntent tests broke when I removed this, although it appears to be done in the base. 
            var thisAsHasName = this as IHasName;
            if (thisAsHasName != null)
            {
                thisAsHasName.Name = ((IHasName)oldRDom).Name;
            }
        }

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

        //public override string OuterName
        //{
        //    get
        //    {
        //        // TODO: Fix OuterName and all those broken tests
        //        var thisAsHasName = this as IHasName;
        //        if (thisAsHasName != null)
        //        {
        //            return (string.IsNullOrWhiteSpace(_containingTypeName) ? "" : _containingTypeName + ".") +
        //                  thisAsHasName.Name;
        //        }
        //        return "";
        //    }
        //}

        //protected virtual string GetQualifiedName()
        //{
        //    var item = this as IHasNamespace;
        //    if (item != null)
        //    {
        //        var outerName = (string.IsNullOrWhiteSpace(_containingTypeName) ? "" : _containingTypeName + ".") +
        //               item.Name;
        //        // There are probably slightly cleaner ways to do this, but with some override scenarios
        //        // directly calling OuterName will result in a StackOverflowException
        //        return (string.IsNullOrWhiteSpace(item.Namespace) ? "" : item.Namespace + ".")
        //                  + outerName;
        //    }
        //    // The following line should not be reachable, so is not tested
        //    throw new InvalidOperationException();
        //}


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
