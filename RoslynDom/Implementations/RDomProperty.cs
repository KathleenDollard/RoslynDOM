using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomProperty : RDomBase<IProperty, IPropertySymbol>, IProperty
    {
        private IList<IParameter> _parameters = new List<IParameter>();
        private IList<IStatement> _getStatements = new List<IStatement>();
        private IList<IStatement> _setStatements = new List<IStatement>();
        private AttributeList _attributes = new AttributeList();

        public RDomProperty(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomProperty(RDomProperty oldRDom)
            : base(oldRDom)
        {
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            foreach (var parameter in newParameters)
            { AddParameter(parameter); }
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

        public void RemoveParameter(IParameter parameter)
        { _parameters.Remove(parameter); }

        public void AddParameter(IParameter parameter)
        { _parameters.Add(parameter); }

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

        public void RemoveGetStatement(IStatement statement)
        { _getStatements.Remove(statement); }

        public void AddGetStatement(IStatement statement)
        { _getStatements.Add(statement); }

        public IEnumerable<IStatement> GetStatements
        { get { return _getStatements; } }

        public void RemoveSetStatement(IStatement statement)
        { _setStatements.Remove(statement); }

        public void AddSetStatement(IStatement statement)
        { _setStatements.Add(statement); }

        public IEnumerable<IStatement> SetStatements
        { get { return _setStatements; } }

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
