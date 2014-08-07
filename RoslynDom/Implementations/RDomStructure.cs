using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructure : RDomBaseType<IStructure>, IStructure
    {
        public RDomStructure(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model, MemberKind.Structure, StemMemberKind.Structure)
        { Initialize(); }

        internal RDomStructure(RDomStructure oldRDom)
            : base(oldRDom)
        { }

         public IEnumerable<IClass> Classes
        { get { return Members.OfType<IClass>().ToList(); } }

        public IEnumerable<IType> Types
        {
            get
            {
                IEnumerable<IType> ret = Classes.Concat<IType>(Structures).Concat(Interfaces).Concat(Enums);
                return ret;
            }
        }

        public IEnumerable<IStructure> Structures
        { get { return Members.OfType<IStructure>().ToList(); } }

        public IEnumerable<IInterface> Interfaces
        { get { return Members.OfType<IInterface>().ToList(); } }

        public IEnumerable<IEnum> Enums
        { get { return Members.OfType<IEnum>().ToList(); } }

        public IEnumerable<IConstructor> Constructors
        { get { return Members.OfType<IConstructor>(); } }

      }
}
