namespace RoslynDom.Common
{
   public interface IForStatement : ILoop<IForStatement>
   {
      IVariableDeclaration Variable { get; set; }

      IExpression Incrementor { get; set; }
   }
}