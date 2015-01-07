using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IWorker
   {
      RDomPriority Priority { get; }
   }

   public interface ICorporationWorker : IWorker
   {
      RDomCorporation Corporation { get; set; }
   }

   public interface ICreateFromWorker : ICorporationWorker
   {
      void StandardInitialize<T>(T item, SyntaxNode syntaxNode, IDom parent, SemanticModel model, OutputContext context)
          where T : class, IDom;

      // void InitializePublicAnnotations(IDom item, SyntaxNode syntaxNode, IDom parent, SemanticModel model);
      void InitializeStatements(IStatementBlock statementBlock, SyntaxNode syntaxNode, IDom parent, SemanticModel model);

      IEnumerable<IDom> CreateInvalidMembers(SyntaxNode syntaxNode, IDom parent, SemanticModel model);

      Tuple<string, string, string> ExtractComment(string fullLine);
   }

   public interface IBuildSyntaxWorker : ICorporationWorker
   {
   }
}