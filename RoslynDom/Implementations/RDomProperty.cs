using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomProperty : RDomBase<IProperty, PropertyDeclarationSyntax, IPropertySymbol>, IProperty
    {
        private IList<IParameter> _parameters = new List<IParameter>();

        internal RDomProperty(
             PropertyDeclarationSyntax rawItem,
             IEnumerable<IParameter> parameters,
             IAccessor getAccessor,
             IAccessor setAccessor,
             params PublicAnnotation[] publicAnnotations)
           : base(rawItem, publicAnnotations)
        {
            foreach (var parameter in parameters)
            { AddParameter(parameter); }
            GetAccessor = getAccessor;
            SetAccessor = setAccessor;
            Initialize();
        }

        internal RDomProperty(RDomProperty oldRDom)
             : base(oldRDom)
        {
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            foreach (var parameter in newParameters)
            { AddParameter(parameter); }
            AccessModifier = oldRDom.AccessModifier;
            GetAccessor = oldRDom.GetAccessor == null ? null : oldRDom.GetAccessor.Copy();
            SetAccessor = oldRDom.SetAccessor == null ? null : oldRDom.SetAccessor.Copy();
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
            AccessModifier = GetAccessibility();
            // TODO: Get and set accessibility
            PropertyType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
            IsAbstract = Symbol.IsAbstract;
            IsVirtual = Symbol.IsVirtual;
            IsOverride = Symbol.IsOverride;
            IsSealed = Symbol.IsSealed;
            IsStatic = Symbol.IsStatic;
            var propSymbol = Symbol as IPropertySymbol;
            if (propSymbol == null) throw new InvalidOperationException();
            CanGet = (!propSymbol.IsWriteOnly); // or check whether getAccessor is null
            CanSet = (!propSymbol.IsReadOnly); // or check whether setAccessor is null
        }

        public void RemoveParameter(IParameter parameter)
        { _parameters.Remove(parameter); }

        public void AddParameter(IParameter parameter)
        { _parameters.Add(parameter); }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public IReferencedType PropertyType { get; set; }

        public AccessModifier AccessModifier { get; set; }
        public IAccessor GetAccessor { get; set; }
        public IAccessor SetAccessor { get; set; }

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
        { get { return _parameters; } }

        public MemberKind MemberKind
        { get { return MemberKind.Property; } }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            { return ReturnType.QualifiedName; }
            return base.RequestValue(name);
        }
    }
}
