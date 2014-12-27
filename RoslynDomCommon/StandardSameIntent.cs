using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace RoslynDom.Common
{
   public static class StandardSameIntent
   {
      public static bool CheckSameIntent<T>(T one, T other)
            where T : class, IDom
      {
         var checker = new Checker();
         return checker.Check(one, other);
      }

      private class Checker
      {

         internal bool Check<T>(T one, T other)
                  where T : class, IDom
         {
            if (one == null && other == null) return true;
            if (one == null && other != null) return false;
            if (one != null && other == null) return false; //confused about why this is showing uncovered Same_intent_with_nulls seems to test

            if (!CheckSpecial(one, other)) return false;
            if (!CheckCharacteristics(one, other)) return false;
            if (!CheckBase(one, other)) return false;
            if (!CheckEntities(one, other)) return false;
            if (!CheckStatements(one, other)) return false;

            return true;
         }

         private bool CheckSpecial<T>(T one, T other)
                     where T : class, IDom
         {
            // Special interfaces
            if (!Check<T, IHasAttributes>(one, other,
                (x, y) => CheckChildrenAnyOrder(x.Attributes, y.Attributes)))
               return false;
            if (!Check<T, IAttribute>(one, other,
                (x, y) => CheckChildrenAnyOrder(x.AttributeValues, y.AttributeValues)))
               return false;
            // need to compare the boxed value types with Equals
            if (!Check<T, IAttributeValue>(one, other,
                (x, y) => x.ValueType == y.ValueType && x.Value.Equals(y.Value)
                && x.Type == y.Type))
               return false;
            // TODO: Add SameIntent system that supports comparing comments and structured docs
            //if (!Check<T, IHasStructuredDocumentation>(one, other,
            //    (x, y) => Check(x.StructuredDocumentation, y.StructuredDocumentation) && x.Description == y.Description))
            //    return false;
            //if (!Check<T, IComment>(one, other,
            //   (x, y) => x.Text == y.Text && x.IsMultiline == y.IsMultiline))
            //    return false;
            return true;
         }

         private bool CheckCharacteristics<T>(T one, T other)
                      where T : class, IDom
         {
            if (!Check<T, ICanBeStatic>(one, other,
                (x, y) => x.IsStatic == y.IsStatic))
               return false;
            if (!Check<T, IHasAccessModifier>(one, other,
                (x, y) => x.AccessModifier == y.AccessModifier))
               return false;
            if (!Check<T, IHasCondition>(one, other,
                (x, y) => Check(x.Condition, y.Condition)))
               return false;
            // if (!Check<T, IHasGroup> not tested, groups aren't important for same intent
            if (!Check<T, IHasImplementedInterfaces>(one, other,
                (x, y) => CheckChildrenAnyOrder(x.ImplementedInterfaces, y.ImplementedInterfaces)))
               return false;
            if (!Check<T, IHasName>(one, other,
                (x, y) => x.Name == y.Name))
               return false;
            // TODO: Should anything in namespace be checked?
            //if (!Check<T, IHasNamespace>(one, other,
            //    (x, y) => x.Namespace == y.Namespace && x.QualifiedName == y.QualifiedName))
            //    return false;
            if (!Check<T, IHasParameters>(one, other,
                   (x, y) => CheckChildrenAnyOrder(x.Parameters, y.Parameters)))
               return false;
            if (!Check<T, IHasReturnType>(one, other,
                (x, y) => Check(x.ReturnType, y.ReturnType)))
               return false;
            if (!Check<T, IHasTypeParameters>(one, other,
                (x, y) => CheckChildrenAnyOrder(x.TypeParameters, y.TypeParameters)))
               return false;
            if (!Check<T, ILoop>(one, other,
                 (x, y) => x.TestAtEnd == y.TestAtEnd))
               return false;
            if (!Check<T, ICanBeNew>(one, other,
                (x, y) => x.IsNew == y.IsNew))
               return false;
            return true;
         }

         private bool CheckBase<T>(T one, T other)
                     where T : class, IDom
         {
            // Base
            if (!Check<T, IExpression>(one, other,
                (x, y) => x.InitialExpressionString == y.InitialExpressionString
                && x.InitialExpressionLanguage == y.InitialExpressionLanguage
                && x.ExpressionType == y.ExpressionType))
               return false;
            if (!Check<T, INestedContainer>(one, other,
                (x, y) => CheckChildrenAnyOrder(x.Types, y.Types)))
               return false;
            if (!Check<T, IPropertyOrMethod>(one, other,
                (x, y) => x.IsAbstract == y.IsAbstract && x.IsVirtual == y.IsVirtual
                && x.IsOverride == y.IsOverride && x.IsSealed == y.IsSealed))
               return false;
            if (!Check<T, IStatementBlock>(one, other,
                 (x, y) => x.HasBlock == y.HasBlock
                 && CheckChildrenInOrder(x.Statements, y.Statements)))
               return false;
            if (!Check<T, IStemContainer>(one, other,
                (x, y) => CheckChildrenAnyOrder(x.StemMembers, y.StemMembers)
                && CheckChildrenAnyOrder(x.UsingDirectives, y.UsingDirectives)))
               return false;
            if (!Check<T, ITypeMemberContainer>(one, other,
               (x, y) => CheckChildrenAnyOrder(x.Members, y.Members)))
               return false;

            return true;
         }

         private bool CheckEntities<T>(T one, T other)
                     where T : class, IDom
         {
            // entity interfaces
            if (!Check<T, IArgument>(one, other,
                  (x, y) => x.IsOut == y.IsOut
                  && x.IsRef == y.IsRef && Check(x.ValueExpression, y.ValueExpression)))
               return false;
            if (!Check<T, IClass>(one, other,
                (x, y) => x.IsAbstract == y.IsAbstract
                && x.IsSealed == y.IsSealed && Check(x.BaseType, y.BaseType)))
               return false;
            if (!Check<T, IConstructor>(one, other,
              (x, y) => x.ConstructorInitializerType == y.ConstructorInitializerType
               && CheckChildrenInOrder(x.InitializationArguments, y.InitializationArguments)))
               return false;
            if (!Check<T, IEnum>(one, other,
                 (x, y) => Check(x.UnderlyingType, y.UnderlyingType)
                 && CheckChildrenInOrder(x.Members, y.Members)))
               return false;
            if (!Check<T, IEnumMember>(one, other,
                 (x, y) => Check(x.Expression, y.Expression)))
               return false;
            if (!Check<T, IField>(one, other,
                 (x, y) => x.IsReadOnly == y.IsReadOnly
                          && x.IsVolatile == y.IsVolatile
                          && x.IsConstant == y.IsConstant
                          && Check(x.Initializer, y.Initializer)))
               return false;
            if (!Check<T, IMethod>(one, other,
                 (x, y) => x.IsExtensionMethod == y.IsExtensionMethod))
               return false;
            if (!Check<T, IParameter>(one, other,
                       (x, y) => Check(x.Type, y.Type)
                       && x.IsOut == y.IsOut
                       && x.IsRef == y.IsRef
                       && x.IsParamArray == y.IsParamArray
                       && x.IsOptional == y.IsOptional
                       && CheckObject(x.DefaultValue, y.DefaultValue)
                       && x.DefaultValueType == y.DefaultValueType
                       && x.Ordinal == y.Ordinal))
               return false;
            if (!Check<T, IProperty>(one, other,
                       (x, y) => x.CanGet == y.CanGet && x.CanSet == y.CanSet
                       && Check(x.PropertyType, y.PropertyType)
                       && Check(x.GetAccessor, y.GetAccessor)
                       && Check(x.SetAccessor, y.SetAccessor)))
               return false;
            if (!Check<T, IReferencedType>(one, other,
                      (x, y) => x.DisplayAlias == y.DisplayAlias
                      && x.IsArray == y.IsArray
                      && CheckChildrenInOrder(x.TypeArguments, y.TypeArguments)))
               return false;
            if (!Check<T, ITypeParameter>(one, other,
                     (x, y) => x.Variance == y.Variance && x.HasConstructorConstraint == y.HasConstructorConstraint
                     && x.HasReferenceTypeConstraint == y.HasReferenceTypeConstraint
                      && x.HasValueTypeConstraint == y.HasValueTypeConstraint
                      && CheckChildrenAnyOrder(x.ConstraintTypes, y.ConstraintTypes)))
               return false;
            if (!Check<T, IUsingDirective>(one, other,
                  (x, y) => x.Alias == y.Alias))
               return false;
            return true;
         }

         private bool CheckStatements<T>(T one, T other)
                      where T : class, IDom
         {
            // Statement interfaces

            // TODO: Support arguments when you support ObjectCreationExpressions
            //if (!Check<T, IArgument>(one, other,
            //    (x, y) => x.Name == y.Name
            //      && x.IsRef == y.IsRef && x.IsOut == y.IsOut
            //      && Check(x.ValueExpression, y.ValueExpression)))
            //    return false;
            if (!Check<T, IAssignmentStatement>(one, other,
                (x, y) => Check(x.Expression, y.Expression)
                && Check(x.Left, y.Left)
                && x.Operator == y.Operator))
               return false;
            if (!Check<T, IBlockStatement>(one, other,
                (x, y) => CheckChildrenInOrder(x.Statements, y.Statements)))
               return false;
            if (!Check<T, ICheckedStatement>(one, other,
                (x, y) => x.Unchecked == y.Unchecked))
               return false;
            if (!Check<T, IDeclarationStatement>(one, other,
                 (x, y) => x.IsConst == y.IsConst))
               return false;
            if (!Check<T, IForEachStatement>(one, other,
                (x, y) => Check(x.Variable, y.Variable)))
               return false;
            if (!Check<T, IForStatement>(one, other,
                (x, y) => Check(x.Variable, y.Variable)
                && Check(x.Incrementor, y.Incrementor)))
               return false;
            if (!Check<T, IIfStatement>(one, other,
                (x, y) => CheckChildrenInOrder(x.Elses, y.Elses)
                && Check(x.Else, y.Else)))
               return false;
            if (!Check<T, IInvocationStatement>(one, other,
                (x, y) => Check(x.Invocation, y.Invocation)))
               return false;
            if (!Check<T, ILockStatement>(one, other,
                (x, y) => Check(x.Expression, y.Expression)))
               return false;
            // TODO: Implement ObjectCreation expressions
            //if (!Check<T, IObjectCreationExpression>(one, other,
            //    (x, y) => Check(x.Type, y.Type)
            //    && CheckChildrenInOrder(x.Arguments, y.Arguments)))
            //    return false;
            if (!Check<T, IReturnStatement>(one, other,
                (x, y) => Check(x.Return, y.Return)))
               return false;
            // TODO: not currently in use, not certain it's needed
            //if (!Check<T, ISpecialStatement>(one, other,
            //    (x, y) => x.SpecialStatementKind == y.SpecialStatementKind))
            //    return false;
            if (!Check<T, IThrowStatement>(one, other,
                (x, y) => Check(x.ExceptionExpression, y.ExceptionExpression)))
               return false;
            if (!Check<T, ITryStatement>(one, other,
                (x, y) => Check(x.Finally, y.Finally)
                && CheckChildrenInOrder(x.Catches, y.Catches)))
               return false;
            if (!Check<T, ICatchStatement>(one, other,
                (x, y) => Check(x.Variable, y.Variable)
                && Check(x.ExceptionType, y.ExceptionType)))
               return false;
            if (!Check<T, IUsingStatement>(one, other,
                (x, y) => Check(x.Expression, y.Expression)
                && Check(x.Variable, y.Variable)))
               return false;
            if (!Check<T, IVariable>(one, other,
                (x, y) => x.IsImplicitlyTyped == y.IsImplicitlyTyped
                && x.IsAliased == y.IsAliased
                && Check(x.Type, y.Type)
                && Check(x.Initializer, y.Initializer)))
               return false;
            return true;

         }

         private bool Check<T, TCheck>(T one, T other,
             Func<TCheck, TCheck, bool> check)
             where TCheck : class, IDom
             where T : class, IDom
         {
            if (one == null && other == null) return true;
            if (one == null && other != null) return false;
            if (one != null && other == null) return false;

            if (one.GetType() != other.GetType()) return false;

            if (!typeof(TCheck).IsAssignableFrom(one.GetType())) return true;

            var oneAs = one as TCheck;
            var otherAs = other as TCheck;

            return check(oneAs, otherAs);
         }

         private bool CheckObject(object one, object other)
         {
            if (one == null && other == null) return true;
            if (one == null && other != null) return false;
            if (one != null && other == null) return false;

            return one.Equals(other);
         }
         private bool CheckChildrenInOrder<T>(IEnumerable<T> oneList, IEnumerable<T> otherList)
                     where T : class, IDom
         {
            if (oneList.Count() != otherList.Count()) return false;
            var otherArray = otherList.ToArray();
            var oneArray = oneList.ToArray();
            for (int i = 0; i < oneArray.Length; i++)
            {
               if (!Check(oneArray[i], otherArray[i])) return false;

            }
            return true;
         }

         private bool CheckChildrenAnyOrder<T>(IEnumerable<T> oneList, IEnumerable<T> otherList)
                     where T : class, IDom
         {
            if (oneList == null && otherList == null) return true;
            if (oneList == null && otherList != null) return false;
            if (oneList != null && otherList == null) return false;

            if (oneList.Count() != otherList.Count()) return false;
            foreach (var item in oneList)
            {
               var otherItem = otherList.Where(x => item.Matches(x)).FirstOrDefault();
               if (otherItem == null) return false;
               if (!Check(item, otherItem)) return false;
            }
            return true;
         }

      }
   }
}
