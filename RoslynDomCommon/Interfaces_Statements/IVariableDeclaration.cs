namespace RoslynDom.Common
{
   public interface IVariable : IDom, IHasName, IMisc
   {
      IExpression Initializer { get; set; }

      IReferencedType Type { get; set; }

      bool IsImplicitlyTyped { get; set; }

      bool IsAliased { get; set; }

      VariableKind VariableKind { get; }
   }

   public interface IVariableDeclaration : IVariable, IDom<IVariableDeclaration>, IMisc
   {
   }
}