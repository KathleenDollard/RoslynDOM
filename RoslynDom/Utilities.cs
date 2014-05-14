using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynDom
{
    public static class Utilities
    {

        internal static string NameFrom(this SyntaxNode node)
        {
            var nameNode = node.ChildNodes()
                                      .OfType<NameSyntax>()
                                      .SingleOrDefault();
            if (nameNode != null)
            {
                return NameFrom(nameNode);
            }
            var nameToken = node.ChildTokens()
                                      .Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken)
                                      .SingleOrDefault();
            return nameToken.ValueText;
            var token = node.ChildTokens().Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken).First();
            return token.NameFrom();
        }

        internal static string NameFrom(this SyntaxToken token)
        {
            if (token.CSharpKind() != SyntaxKind.IdentifierToken)
            {
                throw new InvalidOperationException();
            }
            return token.ValueText;
        }

        /// <summary>
        /// Return the simple name without any qualifiers
        /// </summary>
        /// <param name="nameSyntax"></param>
        /// <returns></returns>
        internal static string NameFrom(this NameSyntax nameSyntax)
        {
            return nameSyntax.ToString().Replace("@", "");
        }

        internal static string NestedNameFrom(this SyntaxNode node)
        {
            var realParent = node.Parent;
            if (realParent is CompilationUnitSyntax || realParent is NamespaceDeclarationSyntax)
            { return node.NameFrom(); }
            return realParent.NestedNameFrom() + "." + node.NameFrom();
        }

        internal static string QualifiedNameFrom(this SyntaxNode node)
        {
            var realParent = node.Parent;
            if (realParent is CompilationUnitSyntax)
            { return node.NameFrom(); }
            return realParent.QualifiedNameFrom() + "." + node.NameFrom();
        }

        //internal static string QualifiedNameFrom(this SyntaxToken token)
        //{
        //    //if (token.CSharpKind() != SyntaxKind)
        //    //{
        //    //}
        //    throw new NotImplementedException();
        //}

        //internal static string QualifiedNameFrom(this NameSyntax nameSyntax)
        //{
        //    var realParent = nameSyntax.Parent.Parent;
        //    if (realParent is CompilationUnitSyntax )
        //    { return nameSyntax.NameFrom(); }
        //    return realParent.QualifiedNameFrom() + "." + nameSyntax.NameFrom();
        //}

    }
}
