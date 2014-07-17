using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomRoot : RDomBaseStemContainer<IRoot, ISymbol>, IRoot
    {
        public RDomRoot(SyntaxNode rawItem, SemanticModel model)
           : base(rawItem, model)
        { }

        internal RDomRoot(RDomRoot oldRDom)
           : base(oldRDom)
        { }

        public bool HasSyntaxErrors
        {
            get
            {
                return TypedSyntax.GetDiagnostics().Count() > 0;
            }
        }

        public IEnumerable<IClass> RootClasses
        {
            get
            {
                var rootclasses = from x in NonemptyNamespaces
                                  from y in x.Classes
                                  select y;
                return Classes.Union(rootclasses);
            }
        }

        public IEnumerable<IEnum> RootEnums
        {
            get
            {
                var rootenums = from x in NonemptyNamespaces
                                from y in x.Enums
                                select y;
                return Enums.Union(rootenums);
            }
        }

        public IEnumerable<IInterface> RootInterfaces
        {
            get
            {
                var rootinterfaces = from x in NonemptyNamespaces
                                     from y in x.Interfaces
                                     select y;
                return Interfaces.Union(rootinterfaces);
            }
        }

        public IEnumerable<IStructure> RootStructures
        {
            get
            {
                var rootstructures = from x in NonemptyNamespaces
                                     from y in x.Structures
                                     select y;
                return Structures.Union(rootstructures);
            }
        }
    }
}
