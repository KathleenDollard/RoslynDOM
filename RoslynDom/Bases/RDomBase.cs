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
        private List<PublicAnnotation> _publicAnnotations = new List<PublicAnnotation>();

        protected RDomBase(params PublicAnnotation[] publicAnnotations)
        {
            AddPublicAnnotations(publicAnnotations);
        }

        protected RDomBase(IDom oldIDom)
        {
            // PublicAnnotation is a structure, so this copies
            var oldRDom = (RDomBase)oldIDom;
            AddPublicAnnotations(oldRDom._publicAnnotations);
        }

        private void AddPublicAnnotations(IEnumerable<PublicAnnotation> publicAnnotations)
        {
            if (publicAnnotations == null) return;
            foreach (var publicAnnotation in publicAnnotations)
            { this._publicAnnotations.Add(publicAnnotation); }
        }

        private IEnumerable<PublicAnnotation> PublicAnnotations
        {
            get { return _publicAnnotations; }
        }

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

        public abstract ISymbol Symbol { get; }

        public abstract object RequestValue(string name);

        internal abstract ISymbol GetSymbol(SyntaxNode node);

        public object GetPublicAnnotationValue(string name, string key)
        {
            var annotation = GetAnnotation(name);
            if (annotation == default(PublicAnnotation)) return null;
            return annotation[key];
        }

        public T GetPublicAnnotationValue<T>(string name, string key)
        {
            var annotation = GetAnnotation(name);
            if (annotation == default(PublicAnnotation)) return default(T);
            return annotation.GetValue<T>(key);
        }

        public void AddPublicAnnotationValue(string name, string key, object value)
        {
            var publicAnnotation = GetAnnotation(name);
            if (publicAnnotation == default(PublicAnnotation))
            {
                publicAnnotation = new PublicAnnotation(name);
                _publicAnnotations.Add(publicAnnotation);
            }
            publicAnnotation.AddItem(key, value);
        }

        public void AddPublicAnnotationValue(string name, object value)
        {
            AddPublicAnnotationValue(name, name, value);
        }

        private PublicAnnotation GetAnnotation(string name)
        {
            return _publicAnnotations
                                .Where(x => x.Name == name)
                                .FirstOrDefault();
        }

        public bool HasPublicAnnotation(string name)
        {
            return (GetAnnotation(name) != default(PublicAnnotation));
        }

        public object GetPublicAnnotationValue(string name)
        {
            return GetPublicAnnotationValue(name, name);
        }

        public T GetPublicAnnotationValue<T>(string name)
        {
            return GetPublicAnnotationValue<T>(name, name);
        }

        protected bool CheckSameIntent(IDom other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (includePublicAnnotations)
            { if (!this.AnnotationsMatch(other)) return false; }
            return true;
        }
        internal bool AnnotationsMatch(IDom other)
        {
            var rDomOther = other as RDomBase;
            foreach (var annotation in _publicAnnotations)
            {
                var otherAnnotation = rDomOther.GetAnnotation(annotation.Name);
                if (otherAnnotation != annotation) return false;
            }
            return true;
        }
    }

    public abstract class RDomBase<T> : RDomBase, IDom<T>
            where T : IDom<T>
    {
        protected RDomBase(params PublicAnnotation[] publicAnnotations)
           : base(publicAnnotations)
        { }

        protected RDomBase(T oldRDom)
             : base(oldRDom)
        { }

        protected static IEnumerable<T> CopyMembers(IEnumerable<T> members)
        {
            var ret = new List<T>();
            if (members != null)
            {
                foreach (var member in members)
                {
                    ret.Add(member.Copy());
                }
            }
            return ret;
        }

        public virtual T Copy()
        {
            var type = this.GetType();
            var constructor = type.GetTypeInfo()
                .DeclaredConstructors
                .Where(x => x.GetParameters().Count() == 1
                && typeof(T).IsAssignableFrom(x.GetParameters().First().ParameterType))
                .FirstOrDefault();
            if (constructor == null) throw new InvalidOperationException("Missing constructor for clone");
            var newItem = constructor.Invoke(new object[] { this });
            return (T)newItem;
        }

        public bool SameIntent(T other)
        {
            return SameIntent(other, true);
        }

        /// <summary>
        /// Derived classes should override this to determine intent
        /// </summary>
        /// <param name="other"></param>
        /// <param name="includePublicAnnotations"></param>
        /// <returns></returns>
        public virtual bool SameIntent(T other, bool includePublicAnnotations)
        {
            var otherItem = other as RDomBase;
            if (! base.CheckSameIntent(otherItem, includePublicAnnotations)) return false;
            return true;
        }

        protected bool CheckSameIntentChildList<TChild>(IEnumerable<TChild> thisList,
                IEnumerable<TChild> otherList)
             where TChild : IDom<TChild>
        {
            return CheckSameIntentChildList(thisList, otherList, null);
        }
        protected bool CheckSameIntentChildList<TChild>(IEnumerable<TChild> thisList,
             IEnumerable<TChild> otherList, Func<TChild, TChild, bool> compareDelegate)
                where TChild : IDom<TChild>
        {
            if (thisList == null) return (otherList == null);
            if (otherList == null) return false;
            if (thisList.Count() != otherList.Count()) return false;
            compareDelegate = compareDelegate ?? ((x, y) => x.Name == y.Name);
            foreach (var item in thisList)
            {
                var otherItem = otherList.Where(x => compareDelegate(x, item)).FirstOrDefault();
                if (otherItem == null) return false;
                if (!item.SameIntent(otherItem)) return false;
            }
            return true;
        }

    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RDomBase<T, TSyntax, TSymbol> : RDomBase<T>, IRoslynDom<T, TSyntax, TSymbol>
            where TSyntax : SyntaxNode
            where TSymbol : ISymbol
            where T : IDom<T>
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
            _attributes = RDomBase<IAttribute>.CopyMembers(oldRDom._attributes.Cast<RDomAttribute>());
            _symbol = default(TSymbol); // this should be reset, this line is to remind us
        }

        public override bool SameIntent(T other, bool includePublicAnnotations)
        {
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            var rDomOther = other as RDomBase<T, TSyntax, TSymbol>;
            if (rDomOther == null) return false;
            if (Name != rDomOther.Name) return false;
            if (!CheckSameIntentNamespaceNames(rDomOther)) return false;
            if (!CheckSameIntentAccessModifier(other)) return false;
            if (!CheckSameIntentAttributes(rDomOther)) return false;
            if (!CheckSameIntentAccessModifier(other)) return false;
            if (!CheckSameIntentStaticModfier(other)) return false;
            if (!CheckSameIntentReturnType(other)) return false;
            if (!CheckSameIntentPropertyOrMethod(other)) return false;

            return true;
        }

        private bool CheckSameIntentNamespaceNames(RDomBase<T, TSyntax, TSymbol> rDomOther)
        {
            if (this is IHasNamespace)
            {
                if (OuterName != rDomOther.OuterName) return false;
                if (GetNamespace() != rDomOther.GetNamespace()) return false;
                if (GetQualifiedName() != rDomOther.GetQualifiedName()) return false;
            }
            return true;
        }

        private bool CheckSameIntentAttributes(RDomBase<T, TSyntax, TSymbol> rDomOther)
        {
            if (this is IHasAttributes)
            {
                var attributes = this.GetAttributes();
                var otherAttributes = rDomOther.GetAttributes();
                if (attributes != null || otherAttributes != null)
                {
                    if (attributes == null && otherAttributes != null) return false;
                    if (attributes != null && otherAttributes == null) return false;
                    if (attributes.Count() != otherAttributes.Count()) return false;
                    foreach (var attribute in attributes)
                    {
                        // TODO: Consider multiple attributes of the same name and values/attribute type
                        var otherAttribute = otherAttributes.Where(x => x.Name == attribute.Name).FirstOrDefault();
                        if (otherAttribute == null) return false;
                        if (!attribute.SameIntent(otherAttribute)) return false;
                    }
                }
            }
            return true;
        }

        private bool CheckSameIntentAccessModifier(T other)
        {
            var item = this as IHasAccessModifier;
            if (item != null)
            {
                var otherItem = other as IHasAccessModifier;
                if (item.AccessModifier != otherItem.AccessModifier) return false;
            }
            return true;
        }

        private bool CheckSameIntentStaticModfier(T other)
        {
            var item = this as ICanBeStatic;
            if (item != null)
            {
                var otherItem = other as ICanBeStatic;
                if (item.IsStatic != otherItem.IsStatic) return false;
            }
            return true;
        }

        private bool CheckSameIntentReturnType(T other)
        {
            var item = this as IHasReturnType ;
            if (item != null)
            {
                var otherItem = other as IHasReturnType;
                if (!item.ReturnType.SameIntent(otherItem.ReturnType)) return false;
            }
            return true;
        }

        private bool CheckSameIntentPropertyOrMethod(T other)
        {
            var item = this as IPropertyOrMethod;
            if (item != null)
            {
                var otherItem = other as IPropertyOrMethod;
                if ( item.IsAbstract != otherItem.IsAbstract) return false;
                if ( item.IsOverride != otherItem.IsOverride) return false;
                if ( item.IsSealed   != otherItem.IsSealed  ) return false;
                if (item.IsVirtual != otherItem.IsVirtual) return false;
            }
            return true;
        }

        public TSyntax TypedSyntax
        { get { return _rawSyntax; } }

        protected TSyntax OriginalTypedSyntax
        { get { return _originalRawSyntax; } }

        public override object RawItem
        { get { return _rawSyntax; } }

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

        public override string Name
        { get { return Symbol.Name; } }

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

        internal virtual string GetQualifiedName()
        {
            var namespaceName = RoslynDomUtilities.GetContainingNamespaceName(Symbol.ContainingNamespace);
            var typeName = GetContainingTypeName(Symbol.ContainingType);
            namespaceName = string.IsNullOrWhiteSpace(namespaceName) ? "" : namespaceName + ".";
            typeName = string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".";
            return namespaceName + typeName + Name;
        }

        internal virtual string GetNamespace()
        { return RoslynDomUtilities.GetContainingNamespaceName(Symbol.ContainingNamespace); }

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
        /// <param name="name"></param>
        /// <returns></returns>
        public override object RequestValue(string name)
        {
            if (ReflectionUtilities.CanGetProperty(this, name))
            {
                var value = ReflectionUtilities.GetPropertyValue(this, name);
                return value;
            }
            return null;
        }


    }

    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    //public abstract class RDomSyntaxNodeBase<T, TSyntax, TSymbol> : RDomBase<T, TSyntax, TSymbol>
    //    where TSyntax : SyntaxNode
    //    where TSymbol : ISymbol
    //    where T : IDom<T>
    //{
    //    // TODO: Consider why this isn't collapsed into the RDomBase<T>
    //    //private TSyntax _rawItem;

    //    internal RDomSyntaxNodeBase(
    //        T oldRDom)
    //        : base(oldRDom)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected RDomSyntaxNodeBase(TSyntax rawItem,
    //                   params PublicAnnotation[] publicAnnotations)
    //             : base(rawItem, publicAnnotations)
    //    {
    //        //_rawItem = rawItem;
    //    }
    //}
}
