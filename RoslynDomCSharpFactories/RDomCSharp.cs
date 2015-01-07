using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDom : IFactoryAccess
   {
      [ExcludeFromCodeCoverage]
      // until move to C# 6 - I want to support name of as soon as possible
      protected static string nameof<T>(T value) { return ""; }

      private static RDom csharp = new RDom();
      private RDomCorporation corporation;

      public RDom()
      {
         corporation = new RDomCorporation(LanguageNames.CSharp, this);
      }

      public static RDom CSharp
      { get { return csharp; } }

      public IRoot LoadFromFile(string fileName)
      {
         var code = File.ReadAllText(fileName);
         SyntaxTree tree = CSharpSyntaxTree.ParseText(code, path: fileName);
         // TODO: Consider whether to expand the filename to full path
         return LoadFromInternal(tree, fileName);
      }

      public IRootGroup LoadFromFiles(params string[] fileNames)
      {
         var trees = new List<SyntaxTree>();
         foreach (var fileName in fileNames)
         {
            var code = File.ReadAllText(fileName);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code, path: fileName);
            trees.Add(tree);
         }
         return LoadGroupFromInternal(null, trees.ToArray());
      }

      public IRoot Load(string code)
      {
         SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
         return LoadFromInternal(tree, null);
      }

      public IRoot Load(Document document)
      {
         Guardian.Assert.IsNotNull(document, nameof(document));
         SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
         return LoadFromInternal(tree, document.FilePath);
      }

      public IRoot Load(SyntaxTree tree)
      {
         return LoadFromInternal(tree, tree.FilePath);
      }

      public IRootGroup LoadGroup(Project project)
      {
         var compilation = project.GetCompilationAsync().Result;
         return LoadGroupFromInternal(compilation, null);
      }

      public IRootGroup LoadGroup(params string[] codeStrings)
      {
         var trees = codeStrings
                        .Select(x => CSharpSyntaxTree.ParseText(x))
                        .ToArray();
         return LoadGroupFromInternal(null, trees);
      }

      public IRootGroup LoadGroup(params SyntaxTree[] trees)
      {
         return LoadGroupFromInternal(null, trees);
      }

      public IRootGroup Load(Compilation compilation)
      {
         return LoadGroupFromInternal(compilation, null);
      }

      public SyntaxNode GetSyntaxNode(IDom item)
      {
         var syntaxGroup = GetSyntaxGroup(item);
         if (syntaxGroup == null || !syntaxGroup.Any()) return null;
         return syntaxGroup.Single();
      }

      public SyntaxTree GetSyntaxTree(IRoot root)
      {
         var rootNode = GetSyntaxNode(root);
         return SyntaxFactory.SyntaxTree(rootNode);
      }

      public string GetSourceCode(IDom item)
      { return GetSyntaxNode(item).ToFullString(); }

      public string GetFormattedSourceCode(IDom item)
      {
         var node = GetSyntaxNode(item);
         var ret = Format(node);
         return ret.ToFullString();
      }

      public IExpression ParseExpression(string expressionAsString)
      {
         var expressionSyntax = SyntaxFactory.ParseExpression(expressionAsString);
         expressionSyntax = RDom.CSharp.Format(expressionSyntax) as ExpressionSyntax;
         var expression = corporation.CreateSpecial<IExpression>(expressionSyntax, null, null).FirstOrDefault();
         return expression;
      }

      public SyntaxTree Format(SyntaxTree tree)
      {
         var root = tree.GetCompilationUnitRoot();
         var span = root.FullSpan;
         root = Formatter.Format(root, span, new CustomWorkspace()) as CompilationUnitSyntax;
         return SyntaxFactory.SyntaxTree(root);
      }

      public SyntaxNode Format(SyntaxNode node)
      {
         //var span = node.FullSpan;
         //node = Formatter.Format(node, span, new CustomWorkspace()) as SyntaxNode;
         var ws = new CustomWorkspace();
         var options = ws.Options;
         // options = options.WithChangedOption(CSharpFormattingOptions.)
         node = Formatter.Format(node, new CustomWorkspace());
         return node;
      }

      private IRoot LoadFromInternal(SyntaxTree tree, string filePath)
      {
         //corporation.CheckContainer();
         var compilation = CSharpCompilation.Create("MyCompilation",
                                        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                        syntaxTrees: new[] { tree },
                                        references: new[] { MetadataReference.CreateFromAssembly(typeof(object).Assembly) });
         var model = compilation.GetSemanticModel(tree);
         var root = corporation.Create(tree.GetCompilationUnitRoot(), null, model).FirstOrDefault() as IRoot;
         //var root = corporation.CreateFrom<IRoot>(tree.GetCompilationUnitRoot(), null, model).FirstOrDefault();
         root.FilePath = filePath;
         return root;
      }

      private IRootGroup LoadGroupFromInternal(Compilation compilation, params SyntaxTree[] trees)
      {
         //corporation.CheckContainer();
         if (compilation == null)
         {
            compilation = CSharpCompilation.Create("MyCompilation",
                                          options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                          syntaxTrees: trees,
                                          references: new[] { MetadataReference.CreateFromAssembly(typeof(object).Assembly) });
         }
         var group = corporation.CreateCompilation(compilation, null, null);

         // TODO: Loop through to create trees in the Root factory
         //foreach (var tree in trees)
         //{
         //   var model = compilation.GetSemanticModel(tree);
         //   var root = corporation.Create(tree.GetCompilationUnitRoot(), null, model).FirstOrDefault() as IRoot;
         //   group.Roots.AddOrMove(root);
         //}

         // TODO: Set the filepath from the SyntaxTree FilePath in the root factory
         //root.FilePath = filePath;
         return group;
      }

      internal IEnumerable<SyntaxNode> GetSyntaxGroup(IDom item)
      {
         var nodes = corporation.GetSyntaxNodes(item);
         return nodes;
      }

      public bool ContainerCheck()
      {
         // TODO: Create reality check on load
         return true;
      }
   }
}