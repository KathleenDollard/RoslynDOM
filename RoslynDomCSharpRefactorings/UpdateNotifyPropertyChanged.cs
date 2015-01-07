using RoslynDom.Common;
using System;
using System.Linq;

namespace RoslynDom.CSharp
{
   [UpdateRefactoring("INotifyPropertyChanged")]
   public class UpdateNotifyPropertyChanged : IUpdateRefactoring<IProperty>
   {
      public bool NeedsChange(IProperty prop)
      {
         if (prop.DeclaredAccessModifier != AccessModifier.Public) return false;
         if (!(prop.CanGet && prop.CanSet)) return false;
         if (prop.GetAccessor.Statements.Count() != 1) return true;
         if (prop.SetAccessor.Statements.Count() != 1) return true;
         var returnStatement = prop.GetAccessor.Statements.First() as IReturnStatement;
         if (returnStatement == null) return true;
         if (returnStatement.Return.InitialExpressionLanguage != ExpectedLanguages.CSharp) { throw new InvalidOperationException(); }
         var fieldName = returnStatement.Return.InitialExpressionString;
         var setStatement = prop.SetAccessor.Statements.First() as IInvocationStatement;
         var expected = string.Format("SetProperty(ref {0}, value)", fieldName);
         if (setStatement.Invocation.InitialExpressionLanguage != ExpectedLanguages.CSharp) { throw new InvalidOperationException(); }
         if (setStatement.Invocation.InitialExpressionString.NormalizeWhitespace() != expected.NormalizeWhitespace()) return true;
         return false;
      }

      public bool MakeChange(IProperty prop)
      {
         string fieldName = AddFieldForProperty(prop);
         UpdatePropertyGet(prop, fieldName);
         UpdatePropertySet(prop, fieldName);
         return true;
      }

      private static string AddFieldForProperty(IProperty prop)
      {
         // Add the field without further checks because the programmer will find and resolve
         // things like naming collisions
         var parent = prop.Parent as ITypeMemberContainer;
         var fieldName = (prop.Name.StartsWith("_") ? "" : "_") + StringUtilities.CamelCase(prop.Name);
         var field = new RDomField(fieldName, prop.ReturnType, declaredAccessModifier: AccessModifier.Private);
         FixWhitespace(field, prop);
         field.Whitespace2Set.Add(new Whitespace2(prop.Whitespace2Set.First().Copy()));
         parent.MembersAll.InsertOrMoveBefore(prop, field);
         return fieldName;
      }

      private static void FixWhitespace(RDomField field, IProperty prop)
      {
         // TODO: This is rather detailed because of featuresnot yet in the whitespace system
         var leading = prop.Whitespace2Set[LanguageElement.Public].LeadingWhitespace;
         field.Whitespace2Set[LanguageElement.Private] = new Whitespace2(LanguageElement.Private, leading, " ", "");
      }

      private static void UpdatePropertyGet(IProperty prop, string fieldName)
      {
         var retExpression = RDom.CSharp.ParseExpression(fieldName);
         var statement = new RDomReturnStatement(retExpression, true);
         prop.GetAccessor.StatementsAll.AddOrMove(statement);
         prop.GetAccessor.EnsureNewLineAfter();
      }

      private static void UpdatePropertySet(IProperty prop, string fieldName)
      {
         var expression = (IInvocationExpression)RDom.CSharp.ParseExpression(string.Format("SetProperty(ref {0}, value)", fieldName));
         var statement = new RDomInvocationStatement(expression, true);
         prop.SetAccessor.StatementsAll.AddOrMove(statement);
         prop.GetAccessor.EnsureNewLineAfter();
      }
   }
}