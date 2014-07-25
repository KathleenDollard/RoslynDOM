using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RDomBaseStemContainer<T, TSymbol> : RDomBase<T, TSymbol>, IStemContainer
        where TSymbol : ISymbol
        where T : class, IDom<T>
    {
        private RDomList<IStemMemberCommentWhite> _members;

        internal RDomBaseStemContainer(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomBaseStemContainer(T oldIDom)
             : base(oldIDom)
        {
            Initialize();
            // Really need to keep them in order so need to iterate entire list in order
            var oldRDom = oldIDom as RDomBaseStemContainer<T, TSymbol>;
            var newMembers = new List<IStemMember>();
            foreach (var member in oldRDom.StemMembersAll)
            {
                //ordered in approx expectation of frequency
                if (TryCopyMember<RDomClass>(member, m => new RDomClass(m))) continue;
                if (TryCopyMember<RDomStructure>(member, m => new RDomStructure(m))) continue;
                if (TryCopyMember<RDomInterface>(member, m => new RDomInterface(m))) continue;
                if (TryCopyMember<RDomEnum>(member, m => new RDomEnum(m))) continue;
                if (TryCopyMember<RDomNamespace>(member, m => new RDomNamespace(m))) continue;
                if (TryCopyMember<RDomUsingDirective>(member, m => new RDomUsingDirective(m))) continue;
                if (TryCopyMember<RDomVerticalWhitespace >(member, m => new RDomVerticalWhitespace(m))) continue;
                if (TryCopyMember<RDomComment>(member, m => new RDomComment(m))) continue;
                throw new InvalidOperationException();
            }
        }

        private bool TryCopyMember<TLocal>(IStemMemberCommentWhite member, Func<TLocal, TLocal> constructDelegate)
            where TLocal : class, IStemMemberCommentWhite
        {
            var memberAsT = member as TLocal;
            if (memberAsT != null)
            {
                var newMember = constructDelegate(memberAsT);
                StemMembersAll.AddOrMove(newMember);
                return true;
            }
            return false;
        }

        protected override void Initialize()
        {
            _members = new RDomList<IStemMemberCommentWhite>(this);
            base.Initialize();
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = base.Children.ToList();
                list.AddRange(_members);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = base.Descendants.ToList();
                foreach (var member in _members)
                { list.AddRange(member.DescendantsAndSelf); }
                return list;
            }
        }

        public string Namespace
        // Parent always works here - if its a namespace, we deliberately skip the current, otherwise, the current is never a namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }


        public string QualifiedName
        { get { return RoslynUtilities.GetQualifiedName(this); } }

        public void ClearStemMembers()
        { _members.Clear(); }

        public RDomList<IStemMemberCommentWhite > StemMembersAll
        { get { return _members; } }

        public IEnumerable<INamespace> AllChildNamespaces
        { get { return RoslynDomUtilities.GetAllChildNamespaces(this); } }

        public IEnumerable<INamespace> NonemptyNamespaces
        { get { return RoslynDomUtilities.GetNonEmptyNamespaces(this); } }

        public IEnumerable<IStemMember> StemMembers
        { get { return _members.OfType<IStemMember>().ToList(); } }

        public IEnumerable<INamespace> Namespaces
        { get { return StemMembers.OfType<INamespace>().ToList(); } }

        public IEnumerable<IClass> Classes
        { get { return StemMembers.OfType<IClass>().ToList(); } }

        public IEnumerable<IInterface> Interfaces
        { get { return StemMembers.OfType<IInterface>().ToList(); } }

        public IEnumerable<IStructure> Structures
        { get { return StemMembers.OfType<IStructure>().ToList(); } }

        public IEnumerable<IEnum> Enums
        { get { return StemMembers.OfType<IEnum>().ToList().ToList(); } }

        public IEnumerable<IUsingDirective> UsingDirectives
        { get { return StemMembers.OfType<IUsingDirective>().ToList().ToList(); } }

        public IEnumerable<IType> Types
        { get { return StemMembers.OfType<IType>().ToList().ToList(); } }


    }
}
