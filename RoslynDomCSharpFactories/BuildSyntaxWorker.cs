using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class BuildSyntaxWorker : ICSharpBuildSyntaxWorker
    {
      private RDomCorporation corporation;
      public RDomCorporation Corporation
      {
         get { return corporation; }
         set
         {
            if (corporation != null) throw new InvalidOperationException("Can't reset corporation");
            corporation = value;
         }
      }

      public RDomPriority Priority
        { get { return RDomPriority.Normal; } }

        public SyntaxList<AttributeListSyntax> BuildAttributeSyntax(AttributeCollection attributes)
        {
            var ret = new List<SyntaxNode>();
            foreach (var attr in attributes)
            {
                // TODO: Regroup attributes on group here
                var nodes = Corporation.GetSyntaxNodes(attr);
                if (nodes.Any())
                { ret.AddRange(nodes); }
            }
            if (!ret.Any()) { SyntaxFactory.List<AttributeListSyntax>(); }
            var attributeSyntaxes = ret.OfType<AttributeListSyntax>();
            return SyntaxFactory.List<AttributeListSyntax>( attributeSyntaxes);
        }

        public BlockSyntax GetStatementBlock(IEnumerable<IStatementAndDetail> statements)
        {
            var statementSyntaxList = statements
                              .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                              .ToList();
            return SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
        }

        public TypeSyntax GetVariableTypeSyntax(bool isImplicitlyTyped, IReferencedType type)
        {
            TypeSyntax typeSyntax;
            if (isImplicitlyTyped)
            {
                typeSyntax = SyntaxFactory.ParseTypeName("var");
                typeSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(typeSyntax, type.Whitespace2Set.First());
            }
            else
            { typeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(type).First(); }
            return typeSyntax;

            //if (itemAsVariable.IsImplicitlyTyped)
            //{ return SyntaxFactory.IdentifierName("var"); }

            //var type = itemAsVariable.Type;
            //if (itemAsVariable.IsAliased)
            //{
            //    var typeName = Mappings.AliasFromSystemType(type.Name);
            //    return SyntaxFactory.IdentifierName(typeName);
            //}
            //return (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(type));
        }
    }
}
