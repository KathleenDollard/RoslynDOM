using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom
{
    public class RDomPropertyAccessor : RDomBase<IAccessor, ISymbol>, IAccessor
    {
        private RDomList<IStatement> _statements;
        private AttributeList _attributes = new AttributeList();
        private AccessorType _accessorType;

        public RDomPropertyAccessor(SyntaxNode rawItem, AccessorType accessorType, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
            _accessorType = accessorType;
             Initialize(); 
        }

        internal RDomPropertyAccessor(RDomPropertyAccessor oldRDom)
                : base(oldRDom)
        {
            Initialize();
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            Statements.AddOrMoveRange(newStatements);
            
            AccessModifier = oldRDom.AccessModifier;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _statements = new RDomList<IStatement>(this);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = base.Children.ToList();
                list.AddRange(_statements);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = base.Descendants.ToList();
                foreach (var statement in _statements)
                { list.AddRange(statement.DescendantsAndSelf); }
                return list;
            }
        }

        //public void RemoveStatement(IStatement statement)
        //{ _statements.Remove(statement); }

        //public void AddOrMoveStatement(IStatement statement)
        //{ _statements.Add(statement); }

        public string Name { get; set; }

        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }
        public IReferencedType ReturnType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsSealed { get; set; }
        public bool IsStatic { get; set; }
        public bool IsExtensionMethod { get; set; }

        public RDomList<IStatement> Statements
        { get { return _statements; } }

        public MemberKind MemberKind
        { get { return MemberKind.Method; } }

        public AccessorType AccessorType
        { get { return _accessorType; } }

        public bool HasBlock
        {
            get { return true; }
            set { }
        }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            {
                return ReturnType.QualifiedName;
            }
            return base.RequestValue(name);
        }
    }
}
