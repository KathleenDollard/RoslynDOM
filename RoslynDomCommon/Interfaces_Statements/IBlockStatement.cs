﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IBlockStatement : IStatement,  IDom<IBlockStatement>
    {
        RDomCollection<IStatementCommentWhite> Statements { get; }
    }
}
