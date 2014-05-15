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
    public abstract class RDomBaseClassOrStructure<T> : RDomSyntaxNodeBase<T>, IClassOrStruct
        where T : SyntaxNode
    {
        private IEnumerable<ITypeMember> _members;
        internal RDomBaseClassOrStructure(
            T rawItem,
            IEnumerable<ITypeMember> members)
            : base(rawItem)
        {
            _members = members;
        }

        public override string Name
        {
            get
            { return this.TypedRawItem.NestedNameFrom(); }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.QualifiedNameFrom(); }
        }

        public string OriginalName
        {
            get
            {
                return this.OriginalNameFrom();
            }
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
                return this.AttributesFrom();
            }
        }


    }
}
