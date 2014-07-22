using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{

    public interface IVariable : IDom, IHasName
    {
        IExpression Initializer { get; set; }
        IReferencedType Type { get; set; }
        bool IsImplicitlyTyped { get; set; }
        bool IsConst { get; set; }
    }

    public interface IVariableDeclaration : IVariable, IDom<IVariableDeclaration>, IMisc
    {

    }
}
