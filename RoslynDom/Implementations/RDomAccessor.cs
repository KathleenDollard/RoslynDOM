using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom
{
    public class RDomPropertyAccessor : RDomBase<IAccessor, ISymbol>, IAccessor
    {
        private RDomList<IStatementCommentWhite> _statements;
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
            StatementsAll.AddOrMoveRange(newStatements);

            AccessModifier = oldRDom.AccessModifier;
            if (oldRDom.ReturnType != null)
            { ReturnType = oldRDom.ReturnType.Copy(); }
            HasBlock = oldRDom.HasBlock;
            _accessorType = oldRDom.AccessorType;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _statements = new RDomList<IStatementCommentWhite>(this);
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

        public string Name { get; set; }

        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }
        public IReferencedType ReturnType { get; set; }
   
        public RDomList<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }

        public IEnumerable<IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }

        public MemberKind MemberKind
        { get { return MemberKind.Accessor; } }

        public AccessorType AccessorType
        { get { return _accessorType; } }

        public bool HasBlock
        {
            get { return true; }
            set { }
        }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }

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
