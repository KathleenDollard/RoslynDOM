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
          : RDomBaseSyntaxNodeFactory<RDomReferencedType, TypeSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      //public override Type[] SyntaxNodeTypes
      //{ get { return base.SyntaxNodeTypes.Union(new[] { typeof(BaseTypeSyntax) }).ToArray(); } }

      public RDomReferencedTypeMiscFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      private static WhitespaceKindLookup whitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterStartDelimiter, SyntaxKind.LessThanToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterEndDelimiter, SyntaxKind.GreaterThanToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      public override Func<SyntaxNode, IDom, SemanticModel, bool> CanCreateDelegate
      {
         get
         {
            return (syntax, parent, model) =>
            {
               if (syntax is NameSyntax) { return true; }
               if (syntax is TypeSyntax) { return true; }
               if (syntax is BaseTypeSyntax) { return true; }
               return false;
            };
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var typeParameterSyntax = syntaxNode as TypeParameterSyntax;
         if (typeParameterSyntax != null) throw new NotImplementedException("Should have called TypeParameterFactory");
         var baseTypeSyntax = syntaxNode as BaseTypeSyntax;
         TypeSyntax typeSyntax = null;
         if (baseTypeSyntax != null) { typeSyntax = baseTypeSyntax.Type; }
         else { typeSyntax = syntaxNode as TypeSyntax; }
         if (typeSyntax != null)
         {
            var newItem = new RDomReferencedType(typeSyntax, parent, model);

            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
            StoreWhitespace(typeSyntax, newItem);

            CreateTypeArgs(syntaxNode, model, newItem);

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

      private void StoreWhitespace(TypeSyntax typeSyntax, RDomReferencedType newItem)
      {
         //CreateFromWorker.StoreWhitespace(newItem, typeSyntax, LanguagePart.Current, whitespaceLookup);
         var identifierToken = typeSyntax.ChildTokens()
                        .Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken)
                        .FirstOrDefault();
         CreateFromWorker.StoreWhitespaceForToken(newItem, identifierToken, LanguagePart.Current, LanguageElement.Identifier);
         var firstToken = typeSyntax.GetFirstToken();
         var lastToken = typeSyntax.GetLastToken();
         if (identifierToken != firstToken)
         {
            CreateFromWorker.StoreWhitespaceForToken(newItem, firstToken, LanguagePart.Current, LanguageElement.FirstToken);
         }
         if (identifierToken != lastToken && lastToken.CSharpKind() != SyntaxKind.GreaterThanToken)
         {
            CreateFromWorker.StoreWhitespaceForToken(newItem, lastToken, LanguagePart.Current, LanguageElement.LastToken);
         }
         var typeArgumentList = typeSyntax.ChildNodes()
                                 .OfType<TypeArgumentListSyntax>()
                                 .FirstOrDefault();
         if (typeArgumentList != null)
         {
            CreateFromWorker.StoreWhitespaceForToken(newItem, typeArgumentList.LessThanToken,
                        LanguagePart.Current, LanguageElement.TypeParameterStartDelimiter);
            CreateFromWorker.StoreWhitespaceForToken(newItem, typeArgumentList.GreaterThanToken,
                        LanguagePart.Current, LanguageElement.TypeParameterEndDelimiter);
         }

         //// This is ugly, but there are so many uses for this class I had 
         //// trouble with the generalized StoreWhitespace doubling some
         //// whitespace.
         //var openTypeParam = typeSyntax.ChildTokens()
         //               .Where(x => x.CSharpKind() == SyntaxKind.LessThanToken)
         //               .FirstOrDefault();
         //CreateFromWorker.StoreWhitespaceForToken(newItem, openTypeParam, LanguagePart.Current, LanguageElement.TypeParameterStartDelimiter);
         //var closeTypeParam = typeSyntax.ChildTokens()
         //               .Where(x => x.CSharpKind() == SyntaxKind.GreaterThanToken)
         //               .FirstOrDefault();
         //CreateFromWorker.StoreWhitespaceForToken(newItem, closeTypeParam, LanguagePart.Current, LanguageElement.TypeParameterEndDelimiter);
      }

      private void CreateTypeArgs(SyntaxNode syntaxNode, SemanticModel model, RDomReferencedType newItem)
      {
         var typeArgumentListList = syntaxNode.ChildNodes()
                                 .OfType<TypeArgumentListSyntax>()
                                 .ToList();
         if (typeArgumentListList.Any())
         {
            foreach (var typeArgListSyntax in typeArgumentListList)
            {
               foreach (var typeArg in typeArgListSyntax.Arguments)
               {
                  var newArg = OutputContext.Corporation
                              .Create(typeArg, newItem, model)
                              .FirstOrDefault()
                              as IReferencedType;
                  newItem.TypeArguments.AddOrMove(newArg);
               }
            }
         }
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
         string name = null;
         string nSpace = null;
         var tokens = typeSyntax.ChildTokens()
                  .Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken);
         if (tokens.Any())
         { name = tokens.First().ToString(); }
         else
         {
            var nodes = typeSyntax.ChildNodes()
                        .OfType<NameSyntax>();
            name = nodes.Last().ToString();
            nSpace = string.Join(".", nodes.Take(nodes.Count() - 1).Select(x => x.ToString()));
         }
         newItem.Name = name;
         newItem.Namespace = nSpace;
         //if (name.Contains("."))
         //{
         //   // Assume no nested types as there is no way to distinguish
         //   newItem.Name = typeSyntax.ToString().SubstringAfterLast(".");
         //   newItem.Namespace = typeSyntax.ToString().SubstringBeforeLast(".");
         //}
         //else
         //{
         //   newItem.Name = name;
         //   newItem.Namespace = "";
         //}
      }

      private void InitializeFromSymbol(RDomReferencedType newItem, ISymbol symbol, TypeSyntax typeSyntax)
      {
         newItem.Name = symbol.Name;

         // TODO: Replace this hack becuase it seems to take a private dependency. Containing type seems broken in CTP3 on generics, so I have to first check that it's not
         if (!symbol.GetType().Name.Contains("TypeParameter"))
         {
            if (symbol.ContainingType != null)
            { newItem.ContainingType = symbol.ContainingType; }
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

         node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node,
                           itemAsT.Whitespace2Set.
                           Where(x => x.LanguageElement == LanguageElement.FirstToken)
                           .FirstOrDefault());
         node = BuildSyntaxHelpers.AttachWhitespaceToLast(node,
                           itemAsT.Whitespace2Set.
                           Where(x => x.LanguageElement == LanguageElement.LastToken)
                           .FirstOrDefault());
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, whitespaceLookup);

         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }

      // Not sure if this belongs here or in BuildSyntaxHelpers
      public TypeSyntax TypeSyntaxFromType(IReferencedType type)
      {
         // Type syntax is realy ugly to build, so I build it and then hack hte type arg whitespace
         var typeName = CleanName(type);
         if (type.TypeArguments.Any())
         {
            typeName += "<";
            foreach (var tArg in type.TypeArguments)
            {
               var display = BuildSyntax(tArg).FirstOrDefault().ToFullString();
               typeName += display;
               typeName += (tArg == type.TypeArguments.Last()
                              ? ""
                              : ", ");
               //typeName += tArg.QualifiedName +
               //             (tArg == type.TypeArguments.Last()
               //               ? ""
               //               : ", ");

            }
            typeName += ">";
         }
         if (type.IsArray)
         { typeName += "[]"; }
         var node = SyntaxFactory.ParseTypeName(typeName);
         var typeArgNode = node.ChildNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault();
         if (typeArgNode != null)
         {
            var newStartToken = BuildSyntaxHelpers.AttachWhitespaceToToken(typeArgNode.LessThanToken,
                        type.Whitespace2Set[LanguageElement.TypeParameterStartDelimiter]);
            var newEndToken = BuildSyntaxHelpers.AttachWhitespaceToToken(typeArgNode.GreaterThanToken,
                        type.Whitespace2Set[LanguageElement.TypeParameterEndDelimiter]);
            var newTypeArgNode = typeArgNode.WithLessThanToken(newStartToken).WithGreaterThanToken(newEndToken);
            //var newTypeArgNode = typeArgNode.WithLessThanToken(newStartToken);
            node = node.ReplaceNode(typeArgNode, newTypeArgNode);
         }
         return node;
      }

      public static string CleanName(IReferencedType type)
      {
         string typeName = type.QualifiedName;
         if (type.DisplayAlias)
         { typeName = AliasFromTypeName(typeName); }
         else
         { typeName = RemoveUsingPrefixes(type, typeName); }
         return typeName;
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

      //private static string RemoveUsingPrefixes(IReferencedType type, string qualifiedName)
      //{
      //   // C# does not support partial namespace usings right now
      //   var nsName = qualifiedName.SubstringBeforeLast(".");
      //   if (string.IsNullOrWhiteSpace(nsName)) { return qualifiedName; }
      //   var typeName = qualifiedName.SubstringAfterLast(".");
      //   IDom context = type;
      //   var list = new List<IUsingDirective>();
      //   while (context != null)
      //   {
      //      var contextAsStemContainer = context as IStemContainer;
      //      if (contextAsStemContainer != null)
      //      { list.AddRange(contextAsStemContainer.UsingDirectives); }
      //      context = context.Parent;
      //   }
      //   foreach (var usingDirective in list)
      //   {
      //      if (usingDirective.Name == nsName)
      //      { return typeName; }
      //   }
      //   return qualifiedName;
      //}

      private static string RemoveUsingPrefixes(IReferencedType type, string qualifiedName)
      {
         // C# does not support partial namespace usings right now
         var nsName = qualifiedName.SubstringBeforeLast(".");
         if (string.IsNullOrWhiteSpace(nsName)) { return qualifiedName; }
         var typeName = qualifiedName.SubstringAfterLast(".");
         var list = GetNamespaceToDrop(type);

         foreach (var usingName in list)
         {
            if (usingName == nsName)
            { return typeName; }
         }
         return qualifiedName;
      }

      private static List<string> GetNamespaceToDrop(IReferencedType type)
      {
         IDom context = type;
         var list = new List<string>();
         while (context != null)
         {
            var contextAsStemContainer = context as IStemContainer;
            if (contextAsStemContainer != null)
            {
               var asNamespace = context as INamespace;
               if (asNamespace != null)
               { list.AddRange(GetNamespaceSubParts(asNamespace.Name)); }
               list.AddRange(contextAsStemContainer.UsingDirectives.Select(x => x.Name));
            }
            context = context.Parent;
         }
         return list;
      }

      private static IEnumerable<string> GetNamespaceSubParts(string name)
      {
         var list = new List<string>();
         do
         {
            list.Add(name);
            name = name.SubstringBeforeLast(".");
         } while (!string.IsNullOrEmpty(name));
         return list;
      }
   }
}
