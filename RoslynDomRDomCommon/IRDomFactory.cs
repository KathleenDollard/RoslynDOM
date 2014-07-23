using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRDomFactory<TKind> : IRDomFactory
    {
        bool CanCreateFrom(SyntaxNode syntaxNode);
        IEnumerable<TKind> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model);
        FactoryPriority Priority { get; }

        IEnumerable<SyntaxNode> BuildSyntax(TKind item);
    }

    public interface IRDomFactory
    {

    }

    public interface IPublicAnnotationFactory : IRDomFactory<IPublicAnnotation>
    { }

    public interface IStructuredDocumentationFactory : IRDomFactory<IStructuredDocumentation>
    { }

    public interface IAttributeFactory : IRDomFactory<IAttribute>
    {
        //IEnumerable<IAttribute> ExtractAttributes(SyntaxNode parentNode, IDom newParent, SemanticModel model);
        //IEnumerable<SyntaxNode> BuildSyntax(AttributeList attributes);
    }

    public interface ICommentWhiteFactory : IRDomFactory<ICommentWhite>
    {    }


    /// <summary>
    /// Priority for candidate selection. These are for clarity. Please add your
    /// own in the format "Normal + 1"
    /// </summary>
    public enum FactoryPriority
    {
        None = 0,
        Fallback = 100,
        Normal = 200,
        Top = 300
    }

}
