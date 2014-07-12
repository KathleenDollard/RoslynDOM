using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    // With apologies for the FactoryFactory naming, but at least it's clear
    public interface IRDomFactoryFactory
    // Currently just used for convention based DI discover
    { }
    public interface IRDomFactoryFactory<T>
    // Currently just used for convention based DI discover
    { }

    public class RDomFactoryFactory<TKind> : IRDomFactoryFactory<TKind>, IRDomFactoryFactory
        where TKind : class, IDom
    {
        private IEnumerable<IRDomFactory<TKind>> _factories;

        public RDomFactoryFactory()
        {
        }

        //public RDomFactoryFactory(IEnumerable<IRDomFactory<TKind>> factories)
        //{
        //    _factories = factories;
        //}

        //public TKind GetStatement(SyntaxNode syntax)
        //{
        //    var candidates = _factories
        //                        .Where(x => x.CanCreateFrom(syntax))
        //                        .OrderBy(x => x.Priority);
        //    if (!candidates.Any()) return null;
        //    return candidates.First().CreateFrom(syntax);
        //}
    }

}
