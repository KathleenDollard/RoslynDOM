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
        { }

        internal RDomRoot(RDomRoot oldRDom)
             : base(oldRDom)
        { }

        public override bool SameIntent(IRoot other, bool includePublicAnnotations)
        {
            // Base class checks classes, etc
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(NonEmptyNamespaces, other.NonEmptyNamespaces)) return false;
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

        public override string Name
        {
            get { return "Root"; }
        }

        public IEnumerable<INamespace> NonEmptyNamespaces
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
                var classes = Classes;
                var rootclasses = from x in NonEmptyNamespaces
                                  from y in x.Classes
                                  select y;
                return classes.Union(rootclasses);
            }
        }

        public IEnumerable<IEnum > RootEnums
        {
            get
            {
                var enums = Enums;
                var rootenums = from x in NonEmptyNamespaces
                                  from y in x.Enums
                                  select y;
                return rootenums.Union(rootenums);
            }
        }

        public IEnumerable<IInterface > RootInterfaces
        {
            get
            {
                var interfaces = Interfaces;
                var rootinterfaces = from x in NonEmptyNamespaces
                                  from y in x.Interfaces 
                                  select y;
                return interfaces.Union(rootinterfaces);
            }
        }

        public IEnumerable<IStructure > RootStructures
        {
            get
            {
                var structures = Structures;
                var rootstructures= from x in NonEmptyNamespaces
                                  from y in x.Structures
                                  select y;
                return structures.Union(rootstructures);
            }
        }
    }
}
