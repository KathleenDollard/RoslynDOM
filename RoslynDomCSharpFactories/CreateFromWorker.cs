using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class CreateFromWorker : ICSharpCreateFromWorker
    {
        [ExcludeFromCodeCoverage]
        private static string nameof<T>(T value) { return ""; }

        public CreateFromWorker(RDomCorporation corporation)
        {
            Corporation = corporation;
        }

        protected RDomCorporation Corporation { get; private set; }

        public RDomPriority Priority
        { get { return RDomPriority.Normal; } }

        public void InitializeStatements(IStatementBlock itemAsStatement, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemAsStatement == null) return;
            var blockSyntax = syntaxNode as BlockSyntax;
            if (syntaxNode == null) return;
            if (blockSyntax != null)
            {
                var statements = ListUtilities.CreateFromList(blockSyntax.Statements, x => Corporation.CreateFrom<IStatementCommentWhite>(x, parent, model));
                itemAsStatement.StatementsAll.AddOrMoveRange(statements);
                itemAsStatement.HasBlock = true;
                return;
            }
            var statementSyntax = syntaxNode as StatementSyntax;
            if (statementSyntax != null)
            {
                var statements = Corporation.CreateFrom<IStatementCommentWhite>(statementSyntax, parent, model);
                if (statements.Count() > 1) throw new NotImplementedException();
                var statement = statements.First();
                var statementAsBlockStatement = statement as IBlockStatement;
                if (statementAsBlockStatement != null)
                {
                    itemAsStatement.HasBlock = true;
                    foreach (var st in statementAsBlockStatement.Statements)
                    { itemAsStatement.StatementsAll.AddOrMove(st); }
                }
                else
                { itemAsStatement.StatementsAll.AddOrMove(statement); }
            }
        }

        public void StandardInitialize<T>(T newItem, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
                where T : class, IDom
        {
            InitializePublicAnnotations(newItem, syntaxNode, parent, model);
            InitializeAttributes(newItem as IHasAttributes, syntaxNode, parent, model);
            InitializeAccessModifiers(newItem as IHasAccessModifier, syntaxNode, parent, model);
            InitializeOOTypeMember(newItem as IOOTypeMember, syntaxNode, parent, model);
            InitializeStatic(newItem as ICanBeStatic, syntaxNode, parent, model);
            InitializeStructuredDocumentation(newItem as IHasStructuredDocumentation, syntaxNode, parent, model);
        }

        public void InitializeAccessModifiers(IHasAccessModifier itemHasAccessModifier, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemHasAccessModifier == null) { return; }
            var itemAsHasSymbol = itemHasAccessModifier as IRoslynHasSymbol;
            Guardian.Assert.IsNotNull(itemAsHasSymbol, nameof(itemAsHasSymbol));

            var accessibility = itemAsHasSymbol.Symbol.DeclaredAccessibility;
            itemHasAccessModifier.AccessModifier = Mappings.AccessModifierFromAccessibility(accessibility);
        }

        public void InitializeAttributes(IHasAttributes itemAsHasAttributes, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemAsHasAttributes == null) { return; }
            var attributes = new List<IAttribute>();
            var attributeLists = syntaxNode.ChildNodes().OfType<AttributeListSyntax>();
            foreach (var attributeList in attributeLists)
            {
                if (attributeList != null)
                {
                    var attr = Corporation.CreateFrom<IAttribute>(attributeList, itemAsHasAttributes, model);
                    attributes.AddRange(attr);
                }
            }
            itemAsHasAttributes.Attributes.AddOrMoveAttributeRange(attributes);
        }

        public void InitializeOOTypeMember(IOOTypeMember itemAsOO, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemAsOO == null) { return; }
            var itemAsDom = itemAsOO as IRoslynHasSymbol;
            itemAsOO.IsAbstract = itemAsDom.Symbol.IsAbstract;
            itemAsOO.IsVirtual = itemAsDom.Symbol.IsVirtual;
            itemAsOO.IsOverride = itemAsDom.Symbol.IsOverride;
            itemAsOO.IsSealed = itemAsDom.Symbol.IsSealed;
        }

        public void InitializeStatic(ICanBeStatic item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (item == null) { return; }
            var itemAsDom = item as IRoslynHasSymbol;
            item.IsStatic = itemAsDom.Symbol.IsStatic;
        }

        public void InitializePublicAnnotations(IDom item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var publicAnnotations = GetPublicAnnotations(syntaxNode, item, model);
            item.PublicAnnotations.Add(publicAnnotations);
        }

        public void InitializeStructuredDocumentation(IHasStructuredDocumentation item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (item == null) return;
            var structuredDocumentation = GetStructuredDocumenation(syntaxNode, item, model).FirstOrDefault();
            if (structuredDocumentation != null)
            {
                item.StructuredDocumentation = structuredDocumentation;
                item.Description = structuredDocumentation.Description;
            }
        }

        public void LoadStemMembers(IStemContainer newItem,
                    IEnumerable<MemberDeclarationSyntax> memberSyntaxes,
                    IEnumerable<UsingDirectiveSyntax> usingSyntaxes,
                    SemanticModel model)
        {
            var usings = ListUtilities.CreateFromList(usingSyntaxes, x => Corporation.CreateFrom<IStemMemberCommentWhite>(x, newItem, model));
            var members = ListUtilities.CreateFromList(memberSyntaxes, x => Corporation.CreateFrom<IStemMemberCommentWhite>(x, newItem, model));
            newItem.StemMembersAll.AddOrMoveRange(usings);
            newItem.StemMembersAll.AddOrMoveRange(members);
        }

        public IEnumerable<ICommentWhite> GetCommentWhite<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
            where T : class, IDom
            where TSyntax : SyntaxNode
        {
            return Corporation.CreateFrom<ICommentWhite>(syntaxNode, newItem, model);
        }

        public IEnumerable<IPublicAnnotation> GetPublicAnnotations<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
           where T : class, IDom
           where TSyntax : SyntaxNode
        {
            return Corporation.CreateFrom<IPublicAnnotation>(syntaxNode, newItem, model);
        }

        public IEnumerable<IStructuredDocumentation> GetStructuredDocumenation<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
            where T : class, IDom
            where TSyntax : SyntaxNode
        {
            return Corporation.CreateFrom<IStructuredDocumentation>(syntaxNode, newItem, model);
        }

        public IEnumerable<TKind> CreateInvalidMembers<TKind>(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
            where TKind : class
        {
            var ret = new RDomInvalidMember(syntaxNode, parent, model) as TKind;
            Guardian.Assert.IsNotNull(ret, nameof(ret));
            return new List<TKind>() { };
        }


    }
}
