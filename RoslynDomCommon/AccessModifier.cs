using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum AccessModifier
    {
        NotApplicable = 0,
        Private,
        ProtectedAndInternal,
        ProtectedAndFriend = 2,
        Protected = 3,
        Internal = 4,
        Friend = 4,
        ProtectedOrInternal = 5,
        ProtectedOrFriend = 5,
        Public = 6
    }
}
