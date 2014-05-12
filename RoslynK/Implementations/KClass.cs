using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// I'm not currently supporting parameters (am supporting type parameters) because I don't understand them
    /// </remarks>
    public class KClass : KBaseClassOrStructure<ClassDeclarationSyntax>, IClass
    {
        internal KClass(ClassDeclarationSyntax rawItem,
            IEnumerable<ITypeMember> members)
            : base(rawItem, members)
        { }
        public override string Name
        {
            get {
                return this.Symbol.Name;
            }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.Identifier.QNameFrom(); }
        }

        public override string BestInContextName
        {
            get { return TypedRawItem.Identifier.BestInContextNameFrom(); }
        }

        public IEnumerable<IClass> Classes
        {
            get
            { return Members.OfType<IClass>(); }
        }

        public IEnumerable<IStemMember> Types
        {
            get
            {
                IEnumerable<IStemMember> ret = ClassesAndStructures.Concat<IStemMember>(Interfaces).Concat(Enums);
                return ret;
            }
        }

        public IEnumerable<IStructure> Structures
        {
            get
            { return Members.OfType<IStructure>(); }
        }

        public IEnumerable<IInterface> Interfaces
        {
            get
            { return Members.OfType<IInterface>(); }
        }

        public IEnumerable<IEnum> Enums
        {
            get
            { return Members.OfType<IEnum>(); }
        }

        public IEnumerable<IClassOrStruct> ClassesAndStructures
        {
            get
            { return Members.OfType<IClassOrStruct>(); }
        }
    }
}
