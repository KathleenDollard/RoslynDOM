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
    public abstract class RDomBaseStemContainer<T, TSyntax, TSymbol> : RDomSyntaxNodeBase<T, TSyntax, TSymbol>
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
        where T : IDom<T>
    {
        private IEnumerable<IStemMember> _members;
        private IEnumerable<IUsing> _usings;

        internal RDomBaseStemContainer(TSyntax rawItem,
                        IEnumerable<IStemMember> members,
                        IEnumerable<IUsing> usings,
                        params PublicAnnotation[] publicAnnotations)
        : base(rawItem, publicAnnotations)
        {
            _members = members;
            _usings = usings;
        }

        internal RDomBaseStemContainer(T oldRDom)
             : base(oldRDom)
        {
            // ick. but I want to keep members in order
            var newMembers = new List<IStemMember>();
            foreach (var member in Members)
            {
                if (member is RDomClass)
                { newMembers.Add(new RDomClass((RDomClass)member)); }
                if (member is RDomStructure)
                { newMembers.Add(new RDomStructure((RDomStructure)member)); }
                if (member is RDomInterface)
                { newMembers.Add(new RDomInterface((RDomInterface)member)); }
                if (member is RDomEnum)
                { newMembers.Add(new RDomEnum((RDomEnum)member)); }
            }
        }

        public override bool SameIntent(T other, bool includePublicAnnotations)
        {
            var rDomOther = other as RDomBaseStemContainer<T, TSyntax, TSymbol>;
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(Classes, rDomOther.Classes)) return false;
            if (!CheckSameIntentChildList(Interfaces, rDomOther.Interfaces)) return false;
            if (!CheckSameIntentChildList(Structures, rDomOther.Structures)) return false;
            if (!CheckSameIntentChildList(Enums, rDomOther.Enums)) return false;
            return true;
        }

        public string Namespace
        {
            get { return GetNamespace(); }
        }

        public string QualifiedName
        {
            get { return GetQualifiedName(); }
        }

        public IEnumerable<INamespace> Namespaces
        {
            get { return Members.OfType<INamespace>(); }
        }

        public IEnumerable<IClass> Classes
        {
            get { return Members.OfType<IClass>(); }
        }

        public IEnumerable<IInterface> Interfaces
        {
            get
            { return Members.OfType<IInterface>(); }
        }

        public IEnumerable<IStemMember> Members
        {
            get { return _members; }
        }


        public IEnumerable<IStructure> Structures
        {
            get { return Members.OfType<IStructure>(); }
        }

        public IEnumerable<IEnum> Enums
        {
            get { return Members.OfType<IEnum>(); }
        }

        public IEnumerable<IUsing> Usings
        {
            get
            { return _usings; }
        }

        public IEnumerable<IStemMember> Types
        {
            get { return Members.OfType<IType>(); }
        }
    }
}
