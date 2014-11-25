using Microsoft.CodeAnalysis;

namespace RoslynDom.Common
{
   public interface IFactoryAccess
   {
      IRoot Load(SyntaxTree tree);
   }
}
