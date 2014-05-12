using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{
    public abstract class KBaseClassOrStructure<T> : KSyntaxNodeBase<T>, IClassOrStruct
    {
        private IEnumerable<ITypeMember> _members;
        internal KBaseClassOrStructure(
            T rawItem,
            IEnumerable<ITypeMember> members)
            : base(rawItem)
        {
            _members = members;
        }

        public IEnumerable<ITypeMember> Members
        {
            get
            { return _members; }
        }
        public IEnumerable<IMethod> Methods
        {
            get
            { return Members.OfType<IMethod>(); }
        }
        public IEnumerable<IProperty> Properties
        {
            get
            { return Members.OfType<IProperty>(); }
        }

        public IEnumerable<IField> Fields
        {
            get
            { return Members.OfType<IField>(); }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }
}
