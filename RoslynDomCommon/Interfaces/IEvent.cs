using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IEvent :
             ICanBeStatic,
             ITypeMember,
             IOOTypeMember,
             ICanBeNew,
             IDom<IEvent>

    {
         IReferencedType Type { get; set; }
        // not yet supporting event accessors
    }
}