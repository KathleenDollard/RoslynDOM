using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RDomBaseStemContainer<T, TSymbol> : RDomBase<T, TSymbol>, IRDomStemContainer
        where TSymbol : ISymbol
        where T : class, IDom<T>
    {
        private IList<IStemMember> _members = new List<IStemMember>();

        internal RDomBaseStemContainer(SyntaxNode rawItem, SemanticModel model)
           : base(rawItem, model)
        {}

        internal RDomBaseStemContainer(T oldIDom)
             : base(oldIDom)
        {
            // ick. but I it avoids an FxCop error, not sure which is worse
            // also, really need to keep them in order so need to iterate entire list in order
            var oldRDom = oldIDom as RDomBaseStemContainer<T,  TSymbol>;
            var newMembers = new List<IStemMember>();
            _members = new List<IStemMember>();
            foreach (var member in oldRDom.StemMembers)
            {
                //ordered in approx expectation of frequency
                var rDomClass = member as RDomClass;
                if (rDomClass != null) { AddOrMoveStemMember(new RDomClass(rDomClass)); }
                else
                {
                    var rDomStructure = member as RDomStructure;
                    if (rDomStructure != null) { AddOrMoveStemMember(new RDomStructure(rDomStructure)); }
                    else
                    {
                        var rDomInterface = member as RDomInterface;
                        if (rDomInterface != null) { AddOrMoveStemMember(new RDomInterface(rDomInterface)); }
                        else
                        {
                            var rDomEnum = member as RDomEnum;
                            if (rDomEnum != null) { AddOrMoveStemMember(new RDomEnum(rDomEnum)); }
                            else
                            {
                                var rDomNamespace = member as RDomNamespace;
                                if (rDomNamespace != null) { AddOrMoveStemMember(new RDomNamespace(rDomNamespace)); }
                                else
                                {
                                    var rDomUsing = member as RDomUsing;
                                    if (rDomUsing != null) { AddOrMoveStemMember(new RDomUsing(rDomUsing)); }
                                    else
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public string Namespace
        // Parent always works here - if its a namespace, we deliberately skip the current, otherwise, the current is never a namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }


        public string QualifiedName
        { get { return GetQualifiedName(); } }

        public void RemoveStemMember(IStemMember member)
        {
            if (member.Parent == null)
            { _members.Remove(member); }
            else
            { RoslynDomSymbolUtilities.RemoveMemberFromParent(this, member); }
        }

        public void AddOrMoveStemMember(IStemMember member)
        {
            RoslynDomSymbolUtilities.PrepMemberForAdd(this, member);
            _members.Add(member);
        }

        public void ClearStemMembers()
        { _members.Clear(); }

        public IEnumerable<INamespace> AllChildNamespaces
        { get { return RoslynDomUtilities.GetAllChildNamespaces(this); } }

        public IEnumerable<INamespace> NonemptyNamespaces
        { get { return RoslynDomUtilities.GetNonEmptyNamespaces(this); } }

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
        { get { return StemMembers.OfType<IUsing>().ToList(); } }

        public IEnumerable<IType> Types
        { get { return StemMembers.OfType<IType>(); } }


    }
}
