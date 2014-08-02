using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom
{
    public class RDomConstructor : RDomBase<IConstructor, IMethodSymbol>, IConstructor
    {
        private RDomList<IParameter> _parameters;
        private RDomList<IArgument> _initializationArguments;
        private RDomList<IStatementCommentWhite> _statements;
        private AttributeList _attributes = new AttributeList();

        public RDomConstructor(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomConstructor(RDomConstructor oldRDom)
             : base(oldRDom)
        {
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            Parameters.AddOrMoveRange(newParameters);
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            StatementsAll.AddOrMoveRange(newStatements);
            var newInitializationArguments = RoslynDomUtilities.CopyMembers(oldRDom._initializationArguments );
            InitializationArguments .AddOrMoveRange(newInitializationArguments);

            ConstructorInitializerType = oldRDom.ConstructorInitializerType;
            AccessModifier = oldRDom.AccessModifier;
            IsStatic = oldRDom.IsStatic;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _parameters = new RDomList<IParameter>(this);
            _statements = new RDomList<IStatementCommentWhite>(this);
            _initializationArguments = new RDomList<IArgument >(this);
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
        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }
        public ConstructorInitializerType ConstructorInitializerType { get; set; }
        public RDomList<IArgument> InitializationArguments
        { get { return _initializationArguments; } }

        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }

        public bool IsStatic { get; set; }
        public RDomList<IParameter> Parameters
        { get { return _parameters; } }

        public RDomList<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }

        public IEnumerable<IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }

        public bool HasBlock
        {
            get { return true; }
            set { }
        }

        public MemberKind MemberKind
        { get { return MemberKind.Constructor; } }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }

    }
}
