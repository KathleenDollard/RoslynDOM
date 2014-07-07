using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomPropertyAccessor : RDomBase<IAccessor , AccessorDeclarationSyntax, ISymbol>, IAccessor 
    {
        private IList<IStatement> _statements = new List<IStatement>();

        internal RDomPropertyAccessor(
            AccessorDeclarationSyntax rawItem,
            IEnumerable<IStatement> statements,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            foreach (var statement in statements)
            { AddStatement(statement); }
            Initialize();
        }

        internal RDomPropertyAccessor(RDomPropertyAccessor oldRDom)
             : base(oldRDom)
        {
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            foreach (var statement in newStatements)
            { AddStatement(statement); }

            AccessModifier = oldRDom.AccessModifier;
        }

        protected override void Initialize()
        {
            base.Initialize();

            AccessModifier = GetAccessibility();
        }

        public override AccessorDeclarationSyntax BuildSyntax()
        {
            return null;
        }

          public void RemoveStatement(IStatement  statement)
        { _statements .Remove(statement); }

        public void AddStatement(IStatement statement)
        { _statements.Add(statement); }

         public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }
        public IReferencedType ReturnType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsSealed { get; set; }
        public bool IsStatic { get; set; }

        public bool IsExtensionMethod { get; set; }

         public IEnumerable<IStatement> Statements
        { get { return _statements ; } }

        public MemberKind MemberKind
        { get { return MemberKind.Method; } }

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
