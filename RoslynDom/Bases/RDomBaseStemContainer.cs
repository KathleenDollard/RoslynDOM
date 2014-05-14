using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBaseStemContainer<T> : RDomSyntaxNodeBase<T>
    {
        private IEnumerable<IStemMember> _members;
        private IEnumerable<IUsing> _usings;

        internal RDomBaseStemContainer(T rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings)
        : base(rawItem)
        {
            _members = members;
            _usings = usings;
        }

        public IEnumerable<IClass> Classes
        {
            get { return Members.OfType<IClass>(); }
        }

        public IEnumerable<IClassOrStruct> ClassesAndStructures
        {
            get { return Members.OfType<IClassOrStruct>(); }
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

           public IEnumerable<INamespace> Namespaces
        {
            get { return Members.OfType<INamespace>(); }
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
            get { return Members.OfType<INestableType>(); }
        }
    }
}
