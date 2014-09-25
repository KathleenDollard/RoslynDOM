using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary></summary>
    /// <remarks>
    /// I'm not currently supporting parameters (am supporting type parameters) because I don't understand them
    /// </remarks>
    public class RDomClass : RDomBaseType<IClass>, IClass
    {
        public RDomClass(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model, MemberKind.Class, StemMemberKind.Class)
        {  }

        internal RDomClass(RDomClass oldRDom)
           : base(oldRDom)
        {
            if (oldRDom.BaseType != null) { BaseType = oldRDom.BaseType.Copy(); }
            IsAbstract = oldRDom.IsAbstract;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
        }

        public IEnumerable<IField> Fields
        { get { return Members.OfType<IField>().ToList(); } }

        public IEnumerable<IOperator> Operators
        { get { return Members.OfType<IOperator>().ToList(); } }

        public IEnumerable<IConversionOperator> ConversionOperators
        { get { return Members.OfType<IConversionOperator>().ToList(); } }

        public IDestructor Destructor
        { get { return Members.OfType<IDestructor>().SingleOrDefault(); } }

        public IEnumerable<IClass> Classes
        { get { return Members.OfType<IClass>().ToList(); } }

        public IEnumerable<IType> Types
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

        public IEnumerable<IConstructor> Constructors
        { get { return Members.OfType<IConstructor>(); } }

        public string ClassName { get { return this.Name; } }
    }
}
