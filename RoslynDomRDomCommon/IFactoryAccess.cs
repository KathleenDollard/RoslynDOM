using Microsoft.CodeAnalysis;

namespace RoslynDom.Common
{
   public interface IFactoryAccess
   {
      IRoot Load(SyntaxTree tree);

      IRoot LoadFromFile(string fileName);

      IRoot Load(string code);

      IRoot Load(Document document);

      IRootGroup LoadFromFiles(params string[] fileNames);

      IRootGroup LoadGroup(Project project);

      IRootGroup LoadGroup(params string[] codeStrings);

      IRootGroup LoadGroup(params SyntaxTree[] trees);

      IRootGroup Load(Compilation compilation);

      SyntaxNode GetSyntaxNode(IDom item);

      SyntaxTree GetSyntaxTree(IRoot root);

      string GetSourceCode(IDom item);

      IExpression ParseExpression(string expressionAsString);
   }
}