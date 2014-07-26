namespace RoslynDom.Common
{
    public class SameIntent_IStatement : ISameIntent<IStatement>
    {

        public bool SameIntent(IStatement one, IStatement other, bool skipPublicAnnotations)
        {
            return true;
        }
    }
}
