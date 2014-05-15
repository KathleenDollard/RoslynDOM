using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class RoslynDomUtilities
    {

        public static IEnumerable<IAttribute> AttributesFrom(this IRoslynDom item)
        {
            if (!(item is IHasAttributes)) throw new InvalidOperationException();

            var retList = new List<IAttribute>();
            var rdomBase = item as RDomBase;
            var attributeData = rdomBase.Symbol.GetAttributes();
            foreach (var data in attributeData)
            {
                retList.Add(new RDomAttribute((AttributeSyntax)data.ApplicationSyntaxReference.GetSyntax()));
            }
            return retList;
        }

        private static IEnumerable<IAttribute> AttributesFromInternal(AttributeListSyntax list)
        {
            var retList = new List<IAttribute>();
            foreach (var attrib in list.ChildNodes().OfType<AttributeSyntax>())
            {
                retList.Add(new RDomAttribute(attrib));
            }
            return retList;
        }

        internal static string OriginalNameFrom(this IRoslynDom item)
        {
            var name = item.Name;
            if (!name.Contains(".")) return name;
            return name.SubstringAfterLast(".");
        }

    }
}
