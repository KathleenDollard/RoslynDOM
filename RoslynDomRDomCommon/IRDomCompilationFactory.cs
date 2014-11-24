using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public interface IRDomCompilationFactory
   {
      RDomPriority Priority { get; }
      IRootGroup CreateFrom(Compilation compilation, bool skipDetail);
      IEnumerable<SyntaxTree> GetCompilation(IRootGroup rootGroup);
   }
}
