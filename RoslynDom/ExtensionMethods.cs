using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom
{
   public static class ExtensionMethods
   {
      public static void InsertAfterInitialFields(this IClass cl, ITypeMember member)
      {
         IField lastField = null;
         foreach (var m in cl.Members)
         {
            if (m is IField) { lastField = m as IField; }
            else { break; }
         }
         if (lastField == null) cl.MembersAll.InsertOrMove(0, member);
         else { cl.MembersAll.InsertOrMoveAfter(lastField, member); }
      }
   }
}
