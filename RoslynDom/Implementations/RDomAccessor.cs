using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom
{
    public class RDomPropertyAccessor : RDomBase<IAccessor, ISymbol>, IAccessor
    {
        private RDomCollection<IStatementCommentWhite> _statements;
        private AttributeCollection _attributes = new AttributeCollection();
        private AccessorType _accessorType;

        public RDomPropertyAccessor(SyntaxNode rawItem, AccessorType accessorType, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
            _accessorType = accessorType;
            Initialize();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomPropertyAccessor(RDomPropertyAccessor oldRDom)
                : base(oldRDom)
        {
            Initialize();
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            StatementsAll.AddOrMoveRange(newStatements);

            AccessModifier = oldRDom.AccessModifier;
            DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
            //if (oldRDom.ReturnType != null)
            //{ ReturnType = oldRDom.ReturnType.Copy(); }
            HasBlock = oldRDom.HasBlock;
            _accessorType = oldRDom.AccessorType;
        }

        protected  void Initialize()
        {
            _statements = new RDomCollection<IStatementCommentWhite>(this);
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

        public AttributeCollection Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }
        public AccessModifier DeclaredAccessModifier { get; set; }
      //  public IReferencedType ReturnType { get; set; }
   
        public RDomCollection<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }

        public IEnumerable<IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }

        //public MemberKind MemberKind
        //{ get { return MemberKind.Accessor; } }

        public AccessorType AccessorType
        { get { return _accessorType; } }

        public bool HasBlock
        {
            get { return true; }
            set { }
        }

        //public string OuterName
        //{ get { return RoslynUtilities.GetOuterName(this); } }
    }
}
