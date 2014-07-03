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
    public class RDomProperty : RDomBase<IProperty, PropertyDeclarationSyntax, IPropertySymbol>, IProperty
    {
        private IEnumerable<IParameter> _parameters;

        internal RDomProperty(
             PropertyDeclarationSyntax rawItem,
             IEnumerable<IParameter> parameters,
             params PublicAnnotation[] publicAnnotations)
           : base(rawItem, publicAnnotations)
        {
            _parameters = parameters;
            Initialize();
        }

        internal RDomProperty(RDomProperty oldRDom)
             : base(oldRDom)
        {
            var newParameters = RDomBase<IParameter>
                         .CopyMembers(oldRDom._parameters.Cast<RDomParameter>());
            _parameters = newParameters;
            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsAbstract = oldRDom.IsAbstract;
            IsVirtual = oldRDom.IsVirtual;
            IsOverride = oldRDom.IsOverride;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
            CanGet = oldRDom.CanGet;
            CanSet = oldRDom.CanSet;
        }
        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = (AccessModifier)Symbol.DeclaredAccessibility;
            PropertyType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
            IsAbstract = Symbol.IsAbstract;
            IsVirtual = Symbol.IsVirtual;
            IsOverride = Symbol.IsOverride;
            IsSealed = Symbol.IsSealed;
            IsStatic = Symbol.IsStatic;
            CanGet = (!((IPropertySymbol)Symbol).IsWriteOnly); ;
            CanSet = (!((IPropertySymbol)Symbol).IsReadOnly); ;
        }

        public override bool SameIntent(IProperty other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (AccessModifier != other.AccessModifier) return false;
            if (PropertyType.QualifiedName  != other.PropertyType.QualifiedName ) return false;
            if (IsAbstract != other.IsAbstract) return false;
            if (IsSealed != other.IsSealed) return false;
            if (IsStatic != other.IsStatic) return false;
            if (IsVirtual != other.IsVirtual) return false;
            if (CanGet != other.CanGet) return false;
            if (CanSet != other.CanSet) return false;
            if (!CheckSameIntentChildList(Parameters, other.Parameters)) return false;
            return true;
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public IReferencedType PropertyType { get; set; }

        public AccessModifier AccessModifier { get; set; }

        public IReferencedType ReturnType
        {
            get { return PropertyType; }
            set { PropertyType = value; }
        }

        public bool IsAbstract { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsOverride { get; set; }

        public bool IsSealed { get; set; }

        public bool IsStatic { get; set; }

        public bool CanGet { get; set; }

        public bool CanSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This is to support VB, C# does not have parameters on properties. Property parameters
        /// in VB are generally used for indexing, which is managed by "default" in C#
        /// <br/>
        /// Can't test until VB is active
        /// </remarks>
        public IEnumerable<IParameter> Parameters
        // This is for VB, wihch I have not yet implemented, but don't want things crashing so will ignore
        { get { return new List<IParameter>(); } }

        public MemberType MemberType
        { get { return MemberType.Property; } }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            { return ReturnType.QualifiedName; }
            return base.RequestValue(name);
        }
    }
}
