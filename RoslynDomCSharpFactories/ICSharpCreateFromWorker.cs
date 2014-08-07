using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal interface ICSharpCreateFromWorker : ICreateFromWorker
    {
        TokenWhitespaceSet GetWhitespaceSet(SyntaxNode syntaxNode, bool includeLeadingTrailing);
        string GetWhitespaceString(SyntaxTriviaList triviaList, bool removeEol);

        IEnumerable<ICommentWhite> GetCommentWhite<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
                    where TSyntax : SyntaxNode
                    where T : class, IDom;

        //IEnumerable<IAttribute> GetAttributes<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
        //    where TSyntax : SyntaxNode
        //    where T : class, IDom;

        IEnumerable<IPublicAnnotation> GetPublicAnnotations<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
            where TSyntax : SyntaxNode
            where T : class, IDom;

        IEnumerable<IStructuredDocumentation> GetStructuredDocumenation<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
              where TSyntax : SyntaxNode
              where T : class, IDom;

        void LoadStemMembers(IStemContainer newItem,
                  IEnumerable<MemberDeclarationSyntax> memberSyntaxes,
                  IEnumerable<UsingDirectiveSyntax> usingSyntaxes,
                  SemanticModel model);

        //TokenWhitespace MakeTokenWhitespace<TSyntaxNode>(SyntaxToken token,
        //           Func<TSyntaxNode, SyntaxToken, TSyntaxNode> withDelegate)
        //           where TSyntaxNode : SyntaxNode;

    }
}