using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynDom
{
    namespace Common.Test
    {
                public class Foo { }
    }
}


namespace RoslynDom
{
    namespace Common
    {
        namespace Test
        {
          //  public class Foo { }
        }
    }
}

namespace RoslynDom.Common.Test
{
    //public class Foo { }
}

namespace RoslynDomTests
{

    public class SomeAttr : Attribute { }

    public class SomeAttr2 : Attribute { }

    [SomeAttr, SomeAttr2]
    struct Foo<T>
    {
        public void FooBar()
        {
        }
    }

    class PlaySpace
    {
        [SomeAttr()]
        private string Foo2;

        public void Foo()
        {
        }

        public string Bar
        {
            [SomeAttr()]
            get
            {
                return "";
            }
        }
    }

    class C
    {
        public string GetName(int value)
        {
            var ret = "";
            if (value == 1)
            {
                C2.Foo();
                ret = "Fred";
            }
            else if (value == 2)
            {
                ret = "George";
            }
            else
            {
                ret = "Percy";
            }
            return ret;
        }
    }

    class C2
    {
        public static void Foo()
        {
            #region 
            try
            { }
            catch (Exception ex)
            { }
            finally
            { }
            #endregion
        }
    }

    class D
    {

        public void temp()
        {

            var x = SyntaxFactory.CompilationUnit()
   .WithMembers(
       SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
           SyntaxFactory.InterfaceDeclaration(
               @"IComment")
           .WithModifiers(
               SyntaxFactory.TokenList(
                   SyntaxFactory.Token(
                       SyntaxFactory.TriviaList(
                           SyntaxFactory.Trivia(
                               SyntaxFactory.DocumentationCommentTrivia(
                                   SyntaxKind.SingleLineDocumentationCommentTrivia,
                                   SyntaxFactory.List<XmlNodeSyntax>(
                                       new XmlNodeSyntax[]{
                                        SyntaxFactory.XmlText()
                                        .WithTextTokens(
                                            SyntaxFactory.TokenList(
                                                SyntaxFactory.XmlTextLiteral(
                                                    SyntaxFactory.TriviaList(
                                                        SyntaxFactory.DocumentationCommentExterior(
                                                            @"///")),
                                                    @" ",
                                                    @" ",
                                                    SyntaxFactory.TriviaList()))),
                                        SyntaxFactory.XmlElement(
                                            SyntaxFactory.XmlElementStartTag(
                                                SyntaxFactory.XmlName(
                                                    SyntaxFactory.Identifier(
                                                        @"summary"))),
                                            SyntaxFactory.XmlElementEndTag(
                                                SyntaxFactory.XmlName(
                                                    SyntaxFactory.Identifier(
                                                        @"summary"))))
                                        .WithContent(
                                            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                                                SyntaxFactory.XmlText()
                                                .WithTextTokens(
                                                    SyntaxFactory.TokenList(
                                                        new []{
                                                            SyntaxFactory.XmlTextNewLine(
                                                                SyntaxFactory.TriviaList(),
                                                                @"
",
                                                                @"
",
                                                                SyntaxFactory.TriviaList()),
                                                            SyntaxFactory.XmlTextLiteral(
                                                                SyntaxFactory.TriviaList(
                                                                    SyntaxFactory.DocumentationCommentExterior(
                                                                        @"    ///")),
                                                                @" THis is summary",
                                                                @" THis is summary",
                                                                SyntaxFactory.TriviaList()),
                                                            SyntaxFactory.XmlTextNewLine(
                                                                SyntaxFactory.TriviaList(),
                                                                @"
",
                                                                @"
",
                                                                SyntaxFactory.TriviaList()),
                                                            SyntaxFactory.XmlTextLiteral(
                                                                SyntaxFactory.TriviaList(
                                                                    SyntaxFactory.DocumentationCommentExterior(
                                                                        @"    ///")),
                                                                @" ",
                                                                @" ",
                                                                SyntaxFactory.TriviaList())})))),
                                        SyntaxFactory.XmlText()
                                        .WithTextTokens(
                                            SyntaxFactory.TokenList(
                                                SyntaxFactory.XmlTextNewLine(
                                                    SyntaxFactory.TriviaList(),
                                                    @"
",
                                                    @"
",
                                                    SyntaxFactory.TriviaList())))})))),
                       SyntaxKind.PublicKeyword,
                       SyntaxFactory.TriviaList())))
           .WithMembers(
               SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                   SyntaxFactory.PropertyDeclaration(
                       SyntaxFactory.PredefinedType(
                           SyntaxFactory.Token(
                               SyntaxKind.StringKeyword)),
                       SyntaxFactory.Identifier(
                           @"Text"))
                   .WithAccessorList(
                       SyntaxFactory.AccessorList(
                           SyntaxFactory.List<AccessorDeclarationSyntax>(
                               new AccessorDeclarationSyntax[]{
                                SyntaxFactory.AccessorDeclaration(
                                    SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(
                                    SyntaxFactory.Token(
                                        SyntaxKind.SemicolonToken)),
                                SyntaxFactory.AccessorDeclaration(
                                    SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(
                                    SyntaxFactory.Token(
                                        SyntaxKind.SemicolonToken))})))))));
        }
    }
}
