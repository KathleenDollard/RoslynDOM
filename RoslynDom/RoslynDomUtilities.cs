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
    internal static class RoslynDomUtilities
    {

        //internal static IEnumerable<IAttribute> AttributesFrom(this IDom item)
        //{
        //    var retList = new List<IAttribute>();
        //    if (!(item is IHasAttributes)) return retList;

        //    var rdomBase = item as RDomBase;
        //    var attributeData = rdomBase.Symbol.GetAttributes();
        //    foreach (var data in attributeData)
        //    {
        //        var attributeValues = new List<IAttributeValue>();
        //        retList.Add(new RDomAttribute((AttributeSyntax)data.ApplicationSyntaxReference.GetSyntax(), attributeValues));
        //    }
        //    return retList;
        //}

        //private static IEnumerable<IAttribute> AttributesFromInternal(AttributeListSyntax list)
        //{
        //    var retList = new List<IAttribute>();
        //    foreach (var attrib in list.ChildNodes().OfType<AttributeSyntax>())
        //    {
        //        retList.Add(new RDomAttribute(attrib));
        //    }
        //    return retList;
        //}

        internal static IEnumerable<ITypeParameter> TypeParametersFrom(this INamedTypeSymbol rDomType)
        {
            return TypeParametersFrom(rDomType.TypeParameters);
        }

        internal static IEnumerable<ITypeParameter> TypeParametersFrom(this IMethodSymbol rDomType)
        {
            return TypeParametersFrom(rDomType.TypeParameters);
        }

   
        private static IEnumerable<ITypeParameter> TypeParametersFrom(IEnumerable<ITypeParameterSymbol> typeParameters)
        {
            var retList = new List<ITypeParameter>();
            foreach (var param in typeParameters)
            {
                retList.Add(new RDomTypeParameter(param.DeclaringSyntaxReferences, param));
            }
            //throw new NotImplementedException();
            return retList;
        }

        internal static IEnumerable<IReferencedType > ImpementedInterfacesFrom(this IHasImplementedInterfaces rDomType, bool all)
        {
            var symbol = ((IRoslynDom)rDomType).Symbol as INamedTypeSymbol;
            var retList = new List<IReferencedType>();
            IEnumerable<INamedTypeSymbol> interfaces;
            if (all) { interfaces = symbol.AllInterfaces; }
            else { interfaces = symbol.Interfaces; }
            foreach (var inter in interfaces)
            {
                retList.Add(new RDomReferencedType(inter.DeclaringSyntaxReferences, inter));

            }
            return retList;

        }
    }
}
