using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class CreateFromWorker : ICSharpCreateFromWorker
    {
        private TriviaManager triviaManager = new TriviaManager();

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
            if (syntaxNode == null) return;
            if (itemAsStatement == null) return;
            var blockSyntax = syntaxNode as BlockSyntax;
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
            InitializeImplementedInterfaces(newItem as IHasImplementedInterfaces, syntaxNode, parent, model);
            InitializeTypeParameters(newItem as IHasTypeParameters, syntaxNode, parent, model);
           // InitializeWhitespace(newItem as IRoslynDom, syntaxNode, parent, model, true);
        }

        private void InitializeAccessModifiers(IHasAccessModifier itemHasAccessModifier, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemHasAccessModifier == null) { return; }
            var itemAsHasSymbol = itemHasAccessModifier as IRoslynHasSymbol;
            Guardian.Assert.IsNotNull(itemAsHasSymbol, nameof(itemAsHasSymbol));

            var accessibility = itemAsHasSymbol.Symbol.DeclaredAccessibility;
            itemHasAccessModifier.AccessModifier = Mappings.AccessModifierFromAccessibility(accessibility);
            var tokens = syntaxNode.ChildTokens();
            if (tokens.Any(x => x.CSharpKind() == SyntaxKind.PublicKeyword))
            { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Public; }
            else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.PrivateKeyword))
            { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Private; }
            else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.ProtectedKeyword && x.CSharpKind() == SyntaxKind.InternalKeyword))
            { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.ProtectedOrInternal; }
            else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.ProtectedKeyword))
            { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Protected; }
            else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.InternalKeyword))
            { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Internal; }
            else
            { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.None; }
        }

        public void InitializePublicAnnotations(IDom item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var publicAnnotations = GetPublicAnnotations(syntaxNode, item, model);
            item.PublicAnnotations.Add(publicAnnotations);
        }

        private void InitializeAttributes(IHasAttributes itemAsHasAttributes, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemAsHasAttributes == null) { return; }
            var attributes = new List<IAttribute>();
            var attributeLists = syntaxNode.ChildNodes().OfType<AttributeListSyntax>();
            foreach (var attributeList in attributeLists)
            {
                // Flatten list
                // Force whitespace
                //var tokenWS = 
                if (attributeList != null)
                {
                    var attr = Corporation.CreateFrom<IAttribute>(attributeList, itemAsHasAttributes, model);
                    attributes.AddRange(attr);
                }
            }
            itemAsHasAttributes.Attributes.AddOrMoveAttributeRange(attributes);
        }

        private void InitializeOOTypeMember(IOOTypeMember itemAsOO, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemAsOO == null) { return; }
            var itemAsDom = itemAsOO as IRoslynHasSymbol;
            itemAsOO.IsAbstract = itemAsDom.Symbol.IsAbstract;
            itemAsOO.IsVirtual = itemAsDom.Symbol.IsVirtual;
            itemAsOO.IsOverride = itemAsDom.Symbol.IsOverride;
            itemAsOO.IsSealed = itemAsDom.Symbol.IsSealed;
        }

        private void InitializeImplementedInterfaces(IHasImplementedInterfaces itemAsT, SyntaxNode node, IDom parent, SemanticModel model)
        {
            if (itemAsT == null) return;

            var symbol = ((IRoslynHasSymbol)itemAsT).Symbol as INamedTypeSymbol;
            var interfaces = symbol.Interfaces;
            var baseType = symbol.BaseType;
            var baseList = node.ChildNodes().OfType<BaseListSyntax>().SingleOrDefault();
            if (baseList != null)
            {
                IEnumerable<TypeSyntax> types = baseList.Types;
                if (node is ClassDeclarationSyntax && baseType.Name == types.First().ToString())
                {
                    types = types.Skip(1);
                }
                foreach (var b in types)
                {
                    var newBase = Corporation.CreateFrom<IMisc>(b, itemAsT, model).Single()
                                    as IReferencedType;
                    itemAsT.ImplementedInterfaces.AddOrMove(newBase);
                }
            }
        }

        private void InitializeTypeParameters(IHasTypeParameters itemAsT, SyntaxNode node, IDom parent, SemanticModel model)
        {
            if (itemAsT == null) return;

            //var symbol = ((IRoslynHasSymbol)itemAsT).Symbol as INamedTypeSymbol;
            //var interfaces = symbol.Interfaces;
            var typeParameterList = node.ChildNodes().OfType<TypeParameterListSyntax>().SingleOrDefault();
            if (typeParameterList == null) return;

            var typeParameters = typeParameterList.Parameters;
            foreach (var p in typeParameters)
            {
                var newBase = Corporation.CreateFrom<IMisc>(p, itemAsT, model).Single()
                                as ITypeParameter;
                itemAsT.TypeParameters.AddOrMove(newBase);
            }
        }

        private void InitializeStatic(ICanBeStatic item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (item == null) { return; }
            var itemAsDom = item as IRoslynHasSymbol;
            item.IsStatic = itemAsDom.Symbol.IsStatic;
        }

         private void InitializeStructuredDocumentation(IHasStructuredDocumentation item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (item == null) return;
            var structuredDocumentation = GetStructuredDocumenation(syntaxNode, item, model).FirstOrDefault();
            if (structuredDocumentation != null)
            {
                item.StructuredDocumentation = structuredDocumentation;
                item.Description = structuredDocumentation.Description;
            }
        }

         private string SpecialLeadingWhitespace(SyntaxNode syntaxNode, IEnumerable<SyntaxTrivia> nodeLeadingWS)
        {
            var leadingWS = "";
            if (nodeLeadingWS.Any())
            {
                leadingWS = String.Join("",
                                nodeLeadingWS
                                    .Select(x => x.ToFullString()));
            }
            else
            {
                // If there is a previous token, 
                var parentNodesAndTokens = syntaxNode
                                .Parent
                                .ChildNodesAndTokens();
                var prevNodeOrToken = parentNodesAndTokens
                                .PreviousSiblings(syntaxNode)
                                .LastOrDefault();
                if (prevNodeOrToken != null && IsPunctuation(prevNodeOrToken))
                {
                    leadingWS = GetWhitespaceString(
                            prevNodeOrToken.AsToken().TrailingTrivia
                            , true);
                }
            }
            return leadingWS;
        }

        private string SpecialTrailingWhitespace(SyntaxNode syntaxNode, IEnumerable<SyntaxTrivia> nodeTrailingWS)
        {
            var trailingWS = "";
            if (nodeTrailingWS.Any())
            {
                trailingWS = String.Join("",
                                nodeTrailingWS
                                    .Select(x => x.ToFullString()));
            }
            else
            {
                // If there is a previous token, 
                var parentNodesAndTokens = syntaxNode
                                .Parent
                                .ChildNodesAndTokens();
                var nextNodeOrToken = parentNodesAndTokens
                                .FollowingSiblings(syntaxNode)
                                .FirstOrDefault();
                if (nextNodeOrToken != null && IsPunctuation(nextNodeOrToken))
                {
                    trailingWS = GetWhitespaceString(
                            nextNodeOrToken.AsToken().LeadingTrivia
                            , true);
                }
            }
            return trailingWS;
        }

        private bool IsPunctuation(SyntaxNodeOrToken nextNodeOrToken)
        {
            return (",.{}]();".Contains(nextNodeOrToken.ToString().Trim()));
        }

        public string GetWhitespaceString(SyntaxTriviaList triviaList, bool removeEol)
        {
            var wsTrivia = triviaList
                            .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia
                                || x.CSharpKind() == SyntaxKind.EndOfLineTrivia);
            if (removeEol)
            {
                // There might be multiple EOLs in the leading trivia that we need to strip here
                var list = new List<SyntaxTrivia>();
                foreach (var item in wsTrivia)
                {
                    if (item.CSharpKind() == SyntaxKind.EndOfLineTrivia) // empty the list and start over, but don't record in leading
                    { list = new List<SyntaxTrivia>(); }
                    else { list.Add(item); }
                }
                wsTrivia = list;
            }
            return string.Join("", wsTrivia.Select(x => x.ToString()));
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

         public void StoreWhitespace(IDom newItem, SyntaxNode syntaxNode, LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup)
        {            triviaManager.StoreWhitespace(newItem, syntaxNode, languagePart, whitespaceLookup);        }

            public void StoreWhitespaceForToken(IDom newItem, SyntaxToken token,
                    LanguagePart languagePart, LanguageElement languageElement)
        {            triviaManager.StoreWhitespaceForToken(newItem, token, languagePart, languageElement);        }

        public void StoreWhitespaceForFirstAndLastToken(IDom newItem, SyntaxNode node,
                LanguagePart languagePart, 
                LanguageElement languageElement)
        {triviaManager.StoreWhitespaceForFirstAndLastToken(newItem, node, languagePart, languageElement);}

      // public void StoreWhitespace(IDom newItem, SyntaxNode syntaxNode, LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup)
        //{
        //    if (syntaxNode == null) return;

        //    // For now, all expressions are held as strings, so we just care about first/last
        //    var nodeAsExpressionSyntax = syntaxNode as ExpressionSyntax;
        //    if (nodeAsExpressionSyntax is ExpressionSyntax)
        //    {
        //        StoreWhitespaceForExpression(newItem, nodeAsExpressionSyntax, languagePart);
        //        return;
        //    }

        //    var lookForIdentifier = whitespaceLookup.Lookup(LanguageElement.Identifier) != SyntaxKind.None;
        //    lookForIdentifier = StoreWhitespaceForChildren(newItem, syntaxNode,
        //                            languagePart, whitespaceLookup, lookForIdentifier);
        //    if (lookForIdentifier)
        //    { StoreWhitespaceForIdentifierNode(newItem, syntaxNode, languagePart); }
        //}

        //private void StoreWhitespaceForExpression(IDom newItem, ExpressionSyntax syntaxNode,
        //       LanguagePart languagePart)
        //{
        //        StoreWhitespaceForFirstAndLastToken(newItem, syntaxNode,
        //                languagePart, LanguageElement.Expression );
        //}

        //private void StoreWhitespaceForIdentifierNode(IDom newItem, SyntaxNode syntaxNode,
        //            LanguagePart languagePart)
        //{
        //    // assume if it was a token we already found it
        //    var idNode = syntaxNode.ChildNodes().OfType<NameSyntax>().FirstOrDefault();
        //    if (idNode != null)
        //    {
        //        StoreWhitespaceForFirstAndLastToken(newItem, idNode,
        //                languagePart, LanguageElement.Identifier);
        //    }
        //}

        //private bool StoreWhitespaceForChildren(IDom newItem, SyntaxNode syntaxNode,
        //            LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup, bool lookForIdentifier)
        //{
        //    foreach (var token in syntaxNode.ChildTokens())
        //    {
        //        var kind = token.CSharpKind();
        //        var languageElement = whitespaceLookup.Lookup(kind);
        //        if (languageElement == LanguageElement.Identifier)
        //        { lookForIdentifier = false; }
        //        if (languageElement != LanguageElement.NotApplicable)
        //        { StoreWhitespaceForToken(newItem, token, languagePart, languageElement); }
        //    }

        //    return lookForIdentifier;
        //}

        //public void StoreWhitespaceForToken(IDom newItem, SyntaxToken token,
        //            LanguagePart languagePart, LanguageElement languageElement)
        //{
        //    var newWS = new Whitespace2(LanguagePart.Current, languageElement);
        //    var region = token.LeadingTrivia.Where(x => x.CSharpKind() == SyntaxKind.RegionDirectiveTrivia).FirstOrDefault();
        //    var structure = region.GetStructure();
        //    newWS.LeadingWhitespace = token.LeadingTrivia
        //                                .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia)
        //                                .Select(x => x.ToString())
        //                                .JoinString();
        //    newWS.TrailingWhitespace = token.TrailingTrivia
        //                                .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia
        //                                        || x.CSharpKind() == SyntaxKind.EndOfLineTrivia)
        //                                .Select(x => x.ToString())
        //                                .JoinString();
        //    // TODO: Add EOL comments here
        //    newItem.Whitespace2Set[languageElement] = newWS;
        //}

        //public void StoreWhitespaceForFirstAndLastToken(IDom newItem, SyntaxNode node,
        //        LanguagePart languagePart, 
        //        LanguageElement languageElement)


        //{
        //    StoreWhitespaceForToken(newItem, node.GetFirstToken(), languagePart, languageElement);
        //    StoreWhitespaceForToken(newItem, node.GetLastToken(), languagePart, languageElement);
        //}


    }
}
