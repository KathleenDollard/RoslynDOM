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
    public class RDomRoot : RDomBaseStemContainer<IRoot, CompilationUnitSyntax, ISymbol>, IRoot
    {

        internal RDomRoot(CompilationUnitSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings,
            params PublicAnnotation[] publicAnnotations)
        : base(rawItem, members, usings, publicAnnotations)
        {
            Initialize();
        }

        internal RDomRoot(RDomRoot oldRDom)
             : base(oldRDom)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            Name = "Root";
        }

        protected override bool CheckSameIntent(IRoot other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            // Base class checks classes, etc
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(NonemptyNamespaces, other.NonemptyNamespaces)) return false;
            return true;
        }

        public IEnumerable<INamespace> AllChildNamespaces
        {
            get
            {
                return RoslynDomUtilities.GetAllChildNamespaces(this);
            }
        }

        public bool HasSyntaxErrors
        {
            get
            {
                return TypedSyntax.GetDiagnostics().Count() > 0;
            }
        }

         public IEnumerable<INamespace> NonemptyNamespaces
        {
            get
            {
                return RoslynDomUtilities.GetNonEmptyNamespaces(this);
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

        public IEnumerable<IEnum > RootEnums
        {
            get
            {
                var rootenums = from x in NonemptyNamespaces
                                  from y in x.Enums
                                  select y;
                return Enums.Union(rootenums);
            }
        }

        public IEnumerable<IInterface > RootInterfaces
        {
            get
            {
                var rootinterfaces = from x in NonemptyNamespaces
                                  from y in x.Interfaces 
                                  select y;
                return Interfaces.Union(rootinterfaces);
            }
        }

        public IEnumerable<IStructure > RootStructures
        {
            get
            {
                var rootstructures= from x in NonemptyNamespaces
                                  from y in x.Structures
                                  select y;
                return Structures.Union(rootstructures);
            }
        }
    }
}
