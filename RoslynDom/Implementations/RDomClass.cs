using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// I'm not currently supporting parameters (am supporting type parameters) because I don't understand them
    /// </remarks>
    public class RDomClass : RDomBaseType<IClass>, IClass
    {
        public RDomClass(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model, MemberKind.Class, StemMemberKind.Class)
        { Initialize(); }

        internal RDomClass(RDomClass oldRDom)
           : base(oldRDom)
        {
            BaseType = oldRDom.BaseType.Copy();
            IsAbstract = oldRDom.IsAbstract;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
        }

        public IEnumerable<IClass> Classes
        { get { return Members.OfType<IClass>().ToList(); } }

        public IEnumerable<IType> Types
        //{ get { return Classes.Concat<IStemMember>(Structures).Concat(Interfaces).Concat(Enums); } }
        { get { return Members.OfType<IType>().ToList(); } }


        public IEnumerable<IStructure> Structures
        { get { return Members.OfType<IStructure>().ToList(); } }

        public IEnumerable<IInterface> Interfaces
        {
            get
            { return Members.OfType<IInterface>().ToList(); }
        }

        public IEnumerable<IEnum> Enums
        {
            get
            { return Members.OfType<IEnum>().ToList(); }
        }

        public bool IsAbstract { get; set; }

        public bool IsSealed { get; set; }

        public bool IsStatic { get; set; }

        public IReferencedType BaseType { get; set; }

        public IEnumerable<IReferencedType> ImplementedInterfaces
        { get { return this.ImpementedInterfacesFrom(false); } }

        public IEnumerable<IReferencedType> AllImplementedInterfaces
        { get { return this.ImpementedInterfacesFrom(true); } }

        public string ClassName { get { return this.Name; } }
    }
}
