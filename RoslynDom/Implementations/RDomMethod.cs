using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System.Linq;
using System;

namespace RoslynDom
{
    public class RDomMethod : RDomBase<IMethod, IMethodSymbol>, IMethod
    {
        private RDomList<IParameter> _parameters;
        private RDomList<ITypeParameter> _typeParameters;
        private RDomList<IStatementCommentWhite> _statements;
        private AttributeList _attributes = new AttributeList();

        public RDomMethod(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomMethod(RDomMethod oldRDom)
             : base(oldRDom)
        {
            Initialize();
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            Parameters.AddOrMoveRange(newParameters);
            var newTypeParameters = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
            TypeParameters.AddOrMoveRange(newTypeParameters);
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            StatementsAll.AddOrMoveRange(newStatements);

            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsAbstract = oldRDom.IsAbstract;
            IsVirtual = oldRDom.IsVirtual;
            IsOverride = oldRDom.IsOverride;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
            IsExtensionMethod = oldRDom.IsExtensionMethod;

        }

        protected override void Initialize()
        {
            base.Initialize();
            _typeParameters = new RDomList<ITypeParameter>(this);
            _parameters = new RDomList<IParameter>(this);
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
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsSealed { get; set; }
        public bool IsStatic { get; set; }

        public bool IsExtensionMethod { get; set; }

        public RDomList<ITypeParameter> TypeParameters
        { get { return _typeParameters; } }

        public RDomList<IParameter> Parameters
        { get { return _parameters; } }

        public RDomList<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }

        public IEnumerable <IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }

        public bool HasBlock
        {
            get { return true; }
            set { }
        }

        public MemberKind MemberKind
        { get { return MemberKind.Method; } }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }

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
