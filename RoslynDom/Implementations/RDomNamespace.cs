using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{

    public class RDomNamespace : RDomBaseStemContainer<INamespace, NamespaceDeclarationSyntax, INamespaceSymbol>, INamespace
    {
        //private string _outerName;

        internal RDomNamespace(NamespaceDeclarationSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings,
            params PublicAnnotation[] publicAnnotations)
            : base(rawItem, members, usings, publicAnnotations)
        {
            Initialize();
        }

        internal RDomNamespace(RDomNamespace oldRDom)
             : base(oldRDom)
        {
            //_outerName = oldRDom.OuterName;

        }

        protected override void Initialize()
        {
            base.Initialize();
            //var namespaceName = RoslynDomUtilities.GetContainingNamespaceName(Symbol.ContainingNamespace);
            //namespaceName = string.IsNullOrWhiteSpace(namespaceName) ? "" : namespaceName + ".";
        }

        protected override bool CheckSameIntent(INamespace other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            // Base class checks classes, etc
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(NonemptyNamespaces, other.NonemptyNamespaces)) return false;
            if (!CheckSameIntentChildList(AllChildNamespaces, other.AllChildNamespaces)) return false;
            return true;
        }

        public IEnumerable<INamespace> AllChildNamespaces
        {
            get { return RoslynDomUtilities.GetAllChildNamespaces(this); }
        }

        public IEnumerable<INamespace> NonemptyNamespaces
        {
            get { return RoslynDomUtilities.GetNonEmptyNamespaces(this); }
        }

        public override string OuterName
        { get { return QualifiedName; } }

        //private string GetNamespace()
        //{ return GetNamespace(Symbol.ContainingNamespace); }

        //private string GetNamespace(INamespaceSymbol nspaceSymbol)
        //{
        //    if (nspaceSymbol == null) return "";
        //    var parentName = GetNamespace(nspaceSymbol.ContainingNamespace);
        //    if (!string.IsNullOrWhiteSpace(parentName))
        //    { parentName = parentName + "."; }
        //    return parentName + nspaceSymbol.Name;
        //}

        //public override string OuterName
        //{ get { return _outerName; } }

        public StemMemberType StemMemberType
        { get { return StemMemberType.Namespace; } }
    }
}
