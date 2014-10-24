namespace RoslynDom.Common
{
    public interface IHasSameIntentMethod 
    {
        bool SameIntent<T>(T other)
                where T : class;
    }
}
