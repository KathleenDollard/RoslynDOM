using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom
{
    public class RDomConstructor : RDomBase<IConstructor, IMethodSymbol>, IConstructor
    {
        private RDomCollection<IParameter> _parameters;
        private RDomCollection<IArgument> _initializationArguments;
        private RDomCollection<IStatementCommentWhite> _statements;
        private AttributeCollection _attributes = new AttributeCollection();

        public RDomConstructor(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomConstructor(RDomConstructor oldRDom)
           : base(oldRDom)
        {
            Initialize();
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            Parameters.AddOrMoveRange(newParameters);
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            StatementsAll.AddOrMoveRange(newStatements);
            var newInitializationArguments = RoslynDomUtilities.CopyMembers(oldRDom._initializationArguments);
            InitializationArguments.AddOrMoveRange(newInitializationArguments);

            ConstructorInitializerType = oldRDom.ConstructorInitializerType;
            AccessModifier = oldRDom.AccessModifier;
            DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
            IsStatic = oldRDom.IsStatic;
        }

        protected void Initialize()
        {
            _parameters = new RDomCollection<IParameter>(this);
            _statements = new RDomCollection<IStatementCommentWhite>(this);
            _initializationArguments = new RDomCollection<IArgument>(this);
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
        public RDomCollection<IArgument> InitializationArguments
        { get { return _initializationArguments; } }

        public AttributeCollection Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }
        public AccessModifier DeclaredAccessModifier { get; set; }

        public bool IsStatic { get; set; }
        public RDomCollection<IParameter> Parameters
        { get { return _parameters; } }

        public RDomCollection<IStatementCommentWhite> StatementsAll
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
