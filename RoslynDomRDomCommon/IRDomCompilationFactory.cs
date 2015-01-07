using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;

namespace RoslynDom
{
   public interface IRDomCompilationFactory
   {
      RDomPriority Priority { get; }

      IRootGroup CreateFrom(Compilation compilation, bool skipDetail);

      IEnumerable<SyntaxTree> GetCompilation(IRootGroup rootGroup);
   }
}