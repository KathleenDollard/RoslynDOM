namespace RoslynDom.Common
{
    public class SameIntent_IExpression : ISameIntent<IExpression>
    {

        public bool SameIntent(IExpression one, IExpression other, bool skipPublicAnnotations)
        {
            if (one.ExpressionType != other.ExpressionType) return false;
            if (one.Expression != other.Expression) return false;
            return true;
        }
    }
}
