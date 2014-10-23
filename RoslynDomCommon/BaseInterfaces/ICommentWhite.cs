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
    public interface ICommentWhite : IStemMemberCommentWhite, ITypeMemberCommentWhite, IStatementCommentWhite, IMisc
    {
    }

   /// <summary>
   /// 
   /// </summary>
   public interface ICommentWhite<T> : ICommentWhite, IDom<T>
         where T : ICommentWhite<T>
   {
   }
}
