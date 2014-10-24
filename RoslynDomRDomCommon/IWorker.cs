using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RoslynDom.Common
{

   public interface ICreateFromWorker
   {
      RDomPriority Priority { get; }
      void StandardInitialize<T>(T item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
          where T : class, IDom;
      // void InitializePublicAnnotations(IDom item, SyntaxNode syntaxNode, IDom parent, SemanticModel model);
      void InitializeStatements(IStatementBlock statementBlock, SyntaxNode syntaxNode, IDom parent, SemanticModel model);
      IEnumerable<IDom> CreateInvalidMembers(SyntaxNode syntaxNode, IDom parent, SemanticModel model);
   }

   public interface IBuildSyntaxWorker
   {
      RDomPriority Priority { get; }
   }
}
