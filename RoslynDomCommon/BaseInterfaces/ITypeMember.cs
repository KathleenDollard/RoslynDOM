using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ITypeMember : IDom,IMember, IHasAttributes ,  IHasAccessModifier  
    {
        MemberType MemberType { get; }
    }
    public interface ITypeMember<T> : ITypeMember, IDom<T>
        where T : ITypeMember<T>
    {
    }
}
