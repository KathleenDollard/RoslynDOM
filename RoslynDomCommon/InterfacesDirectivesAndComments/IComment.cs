using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComment : ICommentWhite<IComment>, IStemMemberCommentWhite, ITypeMemberCommentWhite, IStatementCommentWhite
    {
        string Text { get; set; }
        bool IsMultiline { get; set; }
    }
}
