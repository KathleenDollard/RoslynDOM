using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IDestructor : ITypeMember<IDestructor>,  IStatementContainer, IHasParameters 
    {
        // This actually doesn't have scope in C#, but I didn't want to pull scope off type members
        // particularly since destructors don't exist in all languages
    }
}