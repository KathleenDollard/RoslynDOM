using System;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary>
    /// Base class for Roslyn Dom navigation tree
    /// </summary>
    /// <remarks>
    /// Initialize must be called near end of the constructor. Existing RDom impelementations all do this.
    /// </remarks>
    public abstract class RDomBase : IRoslynDom
    {
        private PublicAnnotationList _publicAnnotations = new PublicAnnotationList();

        protected RDomBase(params PublicAnnotation[] publicAnnotations)
        {
            _publicAnnotations.Add(publicAnnotations);
        }

        protected RDomBase(IDom oldIDom)
        {
            var oldRDom = (RDomBase)oldIDom;
            Name = oldIDom.Name;
            _publicAnnotations.AddCopy(oldRDom._publicAnnotations);
        }

        /// <summary>
        /// Must at least set the Name property
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Return type is object, not SyntaxNode to match interface
        /// </remarks>
        public abstract object RawItem { get; }

        public abstract object OriginalRawItem { get; }

        public IDom Parent { get; internal set; }

        public void RemoveFromParent()
        {
            var parentAsStemContainer = this.Parent as IRDomStemContainer;
            if (parentAsStemContainer != null)
            {
                var thisAsStemMember = this as IStemMember;
                if (thisAsStemMember == null) { throw new InvalidOperationException(); }
                parentAsStemContainer.RemoveMember(thisAsStemMember);
            }
            var parentAsTypeContainer = this.Parent as IRDomTypeContainer;
            if (parentAsTypeContainer != null)
            {
                var thisAsTypeMember = this as ITypeMember;
                if (thisAsTypeMember == null) { throw new InvalidOperationException(); }
                parentAsTypeContainer.RemoveMember(thisAsTypeMember);
            }
            var parentAsCodeContainer = this.Parent as IRDomCodeContainer;
            if (parentAsCodeContainer != null)
            {
                var thisAsCodeMember = this as ICodeMember;
                if (thisAsCodeMember == null) { throw new InvalidOperationException(); }
                parentAsCodeContainer.RemoveMember(thisAsCodeMember);
            }
            Parent = null;
        }

        /// <summary>
        /// NOTE: This documentation has not been updated to reflect changes due to @beefarino's input
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

        public abstract object RequestValue(string propertyName);

        internal abstract ISymbol GetSymbol(SyntaxNode node);

        /// <summary>
        /// For a discussion of names <see cref="OuterName"/>
        /// </summary>
        /// <returns>The string name, same as Roslyn symbol's name</returns>
        public string Name { get; set; }

        public virtual bool Matches(IDom other)
        { return this.Name == other.Name; }

        public bool SameIntent<TLocal>(TLocal other)
               where TLocal : class
        {
            return SameIntent(other, true);
        }

        public bool SameIntent<TLocal>(TLocal other, bool includePublicAnnotations)
         where TLocal : class
        {
            return SameIntentInternal(other, includePublicAnnotations);
        }

        internal abstract bool SameIntentInternal<TLocal>(TLocal other, bool includePublicAnnotations)
                         where TLocal : class;


        public PublicAnnotationList PublicAnnotations
        { get { return _publicAnnotations; } }

        //protected bool CheckPublicAnnotations(IDom other, bool includePublicAnnotations)
        //{
        //    if (includePublicAnnotations)
        //    { if (!this.PublicAnnotations.SameIntent(other.PublicAnnotations)) return false; }
        //    return true;
        //}


    }

}
