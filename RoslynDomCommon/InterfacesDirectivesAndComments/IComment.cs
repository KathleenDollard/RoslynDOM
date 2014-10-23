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
    public interface IComment : IDetail<IComment>, IStemMemberAndDetail, ITypeMemberAndDetail, IStatementAndDetail
    {
        string Text { get; set; }
        bool IsMultiline { get; set; }
    }
}
