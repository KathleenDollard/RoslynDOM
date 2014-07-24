using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomProperty : RDomBase<IProperty, IPropertySymbol>, IProperty
    {
        private RDomList<IParameter> _parameters;
        private AttributeList _attributes = new AttributeList();

        public RDomProperty(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomProperty(RDomProperty oldRDom)
            : base(oldRDom)
        {
            Initialize();
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            Parameters.AddOrMoveRange(newParameters);
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
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
            _parameters = new RDomList<IParameter>(this);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = base.Children.ToList();
                // TODO: Add accessors after switching to accessor approach. 
                if (GetAccessor != null)
                { list.Add(GetAccessor); }
                if (SetAccessor != null)
                { list.Add(SetAccessor); }
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = base.Descendants.ToList();
                // TODO: Add accessors after switching to accessor approach.
                if (GetAccessor != null)
                { list.AddRange(GetAccessor.DescendantsAndSelf); }
                if (SetAccessor != null)
                { list.AddRange(SetAccessor.DescendantsAndSelf); }

                return list;
            }
        }

        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }


        public AttributeList Attributes
        { get { return _attributes; } }

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
        public RDomList<IParameter> Parameters
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
        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }
    }
}
