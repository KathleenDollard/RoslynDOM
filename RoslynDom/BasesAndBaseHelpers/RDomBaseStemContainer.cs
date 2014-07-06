using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RDomBaseStemContainer<T, TSyntax, TSymbol> : RDomBase<T, TSyntax, TSymbol>, IRDomStemContainer
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
        where T : class, IDom<T>
    {
        private IList<IStemMember> _members = new List<IStemMember>();

        internal RDomBaseStemContainer(TSyntax rawItem,
                        IEnumerable<IStemMember> members,
                        IEnumerable<IUsing> usings,
                        params PublicAnnotation[] publicAnnotations)
        : base(rawItem, publicAnnotations)
        {
            foreach (var member in members)
            { AddOrMoveMember(member); }
            foreach (var member in usings)
            { AddOrMoveMember(member); }
            Initialize();
        }

        internal RDomBaseStemContainer(T oldIDom)
             : base(oldIDom)
        {
            // ick. but I it avoids an FxCop error, not sure which is worse
            var oldRDom = oldIDom as RDomBaseStemContainer<T, TSyntax, TSymbol>;
            Namespace = oldRDom.Namespace;
            var newMembers = new List<IStemMember>();
            foreach (var member in oldRDom.StemMembers)
            {
                //ordered in approx expectation of frequency
                var rDomClass = member as RDomClass;
                if (rDomClass != null) { newMembers.Add(new RDomClass(rDomClass)); }
                else
                {
                    var rDomStructure = member as RDomStructure;
                    if (rDomStructure != null) { newMembers.Add(new RDomStructure(rDomStructure)); }
                    else
                    {
                        var rDomInterface = member as RDomInterface;
                        if (rDomInterface != null) { newMembers.Add(new RDomInterface(rDomInterface)); }
                        else
                        {
                            var rDomEnum = member as RDomEnum;
                            if (rDomEnum != null) { newMembers.Add(new RDomEnum(rDomEnum)); }
                            else
                            {
                                var rDomNamespace = member as RDomNamespace;
                                if (rDomNamespace != null) { newMembers.Add(new RDomNamespace(rDomNamespace)); }
                                else
                                {
                                    throw new InvalidOperationException();
                                }
                            }
                        }
                    }
                }
            }
            _members = newMembers;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Namespace = GetNamespace();
        }

        protected override bool CheckSameIntent(T other, bool includePublicAnnotations)
        {
            var rDomOther = other as RDomBaseStemContainer<T, TSyntax, TSymbol>;
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(Classes, rDomOther.Classes)) return false;
            if (!CheckSameIntentChildList(Interfaces, rDomOther.Interfaces)) return false;
            if (!CheckSameIntentChildList(Structures, rDomOther.Structures)) return false;
            if (!CheckSameIntentChildList(Enums, rDomOther.Enums)) return false;
            return true;
        }

        public string Namespace
        { get; set; }

        public string QualifiedName
        { get { return GetQualifiedName(); } }

        public void RemoveMember(IStemMember member)
        { RoslynUtilities.RemoveMemberFromParent(this, member); }

        public void AddOrMoveMember(IStemMember member)
        {
            RoslynUtilities.PrepMemberForAdd(this, member);
            _members.Add(member);
        }

        public IEnumerable<IStemMember> StemMembers
        { get { return _members; } }

        public IEnumerable<INamespace> Namespaces
        { get { return StemMembers.OfType<INamespace>(); } }

        public IEnumerable<IClass> Classes
        { get { return StemMembers.OfType<IClass>(); } }

        public IEnumerable<IInterface> Interfaces
        { get { return StemMembers.OfType<IInterface>(); } }

        public IEnumerable<IStructure> Structures
        { get { return StemMembers.OfType<IStructure>(); } }

        public IEnumerable<IEnum> Enums
        { get { return StemMembers.OfType<IEnum>(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Usings")]
        public IEnumerable<IUsing> Usings
        { get { return StemMembers.OfType<IUsing>(); } }

        public IEnumerable<IType> Types
        { get { return StemMembers.OfType<IType>(); } }


    }
}
