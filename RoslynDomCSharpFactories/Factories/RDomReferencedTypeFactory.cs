using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomReferencedTypeMiscFactory
           : RDomMiscFactory<RDomReferencedType, TypeSyntax>
    {
        private static WhitespaceKindLookup whitespaceLookup;

        public RDomReferencedTypeMiscFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var typeParameterSyntax = syntaxNode as TypeParameterSyntax;
            if (typeParameterSyntax != null) throw new NotImplementedException("Should have called TypeParameterFactory");
            var typeSyntax = syntaxNode as TypeSyntax;
            if (typeSyntax != null)
            {
                var newItem = new RDomReferencedType(syntaxNode, parent, model);

                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
                CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntaxNode, LanguagePart.Current, LanguageElement.Identifier);

                var arrayTypeSyntax = syntaxNode as ArrayTypeSyntax;
                if (arrayTypeSyntax != null)
                {
                    newItem.IsArray = true;
                    var arraySymbol = newItem.Symbol as IArrayTypeSymbol;
                    InitalizeNameAndNamespace(newItem, arraySymbol.ElementType, arrayTypeSyntax.ElementType);
                }
                else
                {
                    newItem.IsArray = false;
                    InitalizeNameAndNamespace(newItem, newItem.Symbol, typeSyntax);
                }
                return newItem;
            }
            throw new InvalidOperationException();
        }

        private void InitalizeNameAndNamespace(RDomReferencedType newItem, ISymbol symbol, TypeSyntax typeSyntax)
        {
            var predefinedSyntax = typeSyntax as PredefinedTypeSyntax;
            if (predefinedSyntax != null) { InitializeFromPredefined(newItem, symbol, predefinedSyntax); }
            else if (symbol != null) { InitializeFromSymbol(newItem, symbol, typeSyntax); }
            else { InitializeFromCode(newItem, typeSyntax); }
        }

        private void InitializeFromCode(RDomReferencedType newItem, TypeSyntax typeSyntax)
        {
            var text = typeSyntax.ToString();
            if (text.Contains("."))
            {
                // Assume no nested types as there is no way to distinguish
                newItem.Name = typeSyntax.ToString().SubstringAfterLast(".");
                newItem.Namespace = typeSyntax.ToString().SubstringBeforeLast(".");
            }
            else
            {
                newItem.Name = text;
                newItem.Namespace = "";
            }
        }

        private void InitializeFromSymbol(RDomReferencedType newItem, ISymbol symbol, TypeSyntax typeSyntax)
        {
            newItem.Name = symbol.Name;

            if (symbol.ContainingType != null)
            {
                newItem.ContainingType = symbol.ContainingType;
            }

            if (symbol.ContainingNamespace == null || symbol.ContainingNamespace.ToString() == "<global namespace>")
            { newItem.Namespace = ""; }
            else
            { newItem.Namespace = symbol.ContainingNamespace.ToString(); }
        }

        private void InitializeFromPredefined(RDomReferencedType newItem, ISymbol symbol, PredefinedTypeSyntax predefinedSyntax)
        {
            newItem.DisplayAlias = true;

            newItem.Name = symbol.Name;
            if (symbol == null) throw new NotImplementedException();
            newItem.Namespace = symbol.ContainingNamespace.ToString();
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IReferencedType;
            var node = TypeSyntaxFromType(itemAsT);

            node = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(node, itemAsT.Whitespace2Set.FirstOrDefault());

            return node.PrepareForBuildSyntaxOutput(item);
        }

        // Not sure if this belongs here or in BuildSyntaxHelpers
        public static TypeSyntax TypeSyntaxFromType(IReferencedType type)
        {
            string typeName = type.QualifiedName;
            if (type.DisplayAlias)
            { typeName = AliasFromTypeName(typeName); }
            else
            { typeName = RemoveUsingPrefixes(type, typeName); }
            if (type.IsArray)
            { typeName += "[]"; }
            return SyntaxFactory.ParseTypeName(typeName);

        }
        private static string AliasFromTypeName(string typeName)
        {
            switch (typeName)
            {
            case "Void":
            case "System.Void": { return "void"; }
            case "System.Object": { return "object"; }
            case "System.String": { return "string"; }
            case "System.Boolean": { return "bool"; }
            case "System.Decimal": { return "decimal"; }
            case "System.SByte": { return "sbyte"; }
            case "System.Byte": { return "byte"; }
            case "System.Int16": { return "short"; }
            case "System.UInt16": { return "ushort"; }
            case "System.Int32": { return "int"; }
            case "System.UInt32": { return "uint"; }
            case "System.Int64": { return "long"; }
            case "System.UInt64": { return "ulong"; }
            case "System.Char": { return "char"; }
            case "System.Single": { return "float"; }
            case "System.Double": { return "double"; }
            default:
                return null;
            }
        }

        private static string RemoveUsingPrefixes(IReferencedType type, string qualifiedName)
        {
            var nsName = qualifiedName.SubstringBeforeLast(".");
            if (string.IsNullOrWhiteSpace(nsName)) { return qualifiedName; }
            var typeName = qualifiedName.SubstringAfterLast(".");
            IDom context = type;
            var list = new List<IUsingDirective>();
            while (context !=  null)
            {
                var contextAsStemContainer = context as IStemContainer;
                if (contextAsStemContainer != null)
                { list.AddRange(contextAsStemContainer.UsingDirectives); }
                context = context.Parent ;
            }
            // C# does not support partial namespace usings right now
            foreach (var usingDirective in list)
            {
                if (usingDirective.Name == nsName)
                { return typeName; }
            }
            return qualifiedName;
        }
    }
}
