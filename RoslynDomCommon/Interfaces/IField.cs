namespace RoslynDom.Common
{
    public interface IField : 
        ITypeMember<IField>, 
        IHasReturnType, 
        ICanBeStatic, 
        IHasStructuredDocumentation , 
        ICanBeNew
    {
        IExpression Initializer { get; set; }
        bool IsReadOnly { get; set; }
        bool IsVolatile { get; set; }
        bool IsConstant { get; set; }
    }
}