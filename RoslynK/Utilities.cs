using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{
    public static class Utilities
    {
 
        internal static string NameFrom(this SyntaxToken token)
         {
            if (token.CSharpKind() != SyntaxKind.IdentifierToken )
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
            //var qName = nameSyntax as QualifiedNameSyntax;
            //if (qName == null)
            //{ return nameSyntax.ChildTokens().First().NameFrom(); }
            //var childNodesAndTokens = nameSyntax.ChildNodesAndTokens();
            //var retName = ""; // StringBuilder not used because rarely more than 5
            //foreach (var child in childNodesAndTokens)
            //{
            //    if (child is SyntaxToken)
            //    { retName += ((SyntaxToken)child).NameFrom(); }
            //    else
            //    { retName += ((NameSyntax)child).NameFrom(); }
            //}
            //return retName;
        }

   
        internal static string QNameFrom(this SyntaxToken token)
        {
            //if (token.CSharpKind() != SyntaxKind)
            //{
            //}
            throw new NotImplementedException();
        }

        internal static string QNameFrom(this NameSyntax nameSyntax)
        {
            //if (token.CSharpKind() != SyntaxKind)
            //{
            //}
            throw new NotImplementedException();
        }

        internal static string BestInContextNameFrom(this SyntaxToken token)
        {
            //if (token.CSharpKind() != SyntaxKind)
            //{
            //}
            throw new NotImplementedException();
        }

        internal static string BestInContextNameFrom(this NameSyntax nameSyntax)
        {
            //if (token.CSharpKind() != SyntaxKind)
            //{
            //}
            throw new NotImplementedException();
        }

    }
}
