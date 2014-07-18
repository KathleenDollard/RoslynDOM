using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System.Linq;

namespace RoslynDom
{
    public class RDomMethod : RDomBase<IMethod, IMethodSymbol>, IMethod
    {
        private IList<IParameter> _parameters = new List<IParameter>();
        private IList<ITypeParameter> _typeParameters = new List<ITypeParameter>();
        private IList<IStatement> _statements = new List<IStatement>();
        private AttributeList _attributes = new AttributeList();

        public RDomMethod(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomMethod(RDomMethod oldRDom)
             : base(oldRDom)
        {
                        Attributes.AddOrMoveAttributeRange( oldRDom.Attributes.Select(x=>x.Copy()));
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            foreach (var parameter in newParameters)
            { AddParameter(parameter); }
            var newTypeParameters = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
            foreach (var typeParameter in newTypeParameters)
            { AddTypeParameter(typeParameter); }
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            foreach (var statement in newStatements)
            { AddStatement(statement); }

            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsAbstract = oldRDom.IsAbstract;
            IsVirtual = oldRDom.IsVirtual;
            IsOverride = oldRDom.IsOverride;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
            IsExtensionMethod = oldRDom.IsExtensionMethod;

        }
        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }
        public IReferencedType ReturnType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsSealed { get; set; }
        public bool IsStatic { get; set; }

        public bool IsExtensionMethod { get; set; }

        public void RemoveTypeParameter(ITypeParameter typeParameter)
        { _typeParameters.Remove(typeParameter); }

        public void AddTypeParameter(ITypeParameter typeParameter)
        { _typeParameters.Add(typeParameter); }

        public IEnumerable<ITypeParameter> TypeParameters
        { get { return _typeParameters; } }

        public void RemoveParameter(IParameter parameter)
        { _parameters.Remove(parameter); }

        public void AddParameter(IParameter parameter)
        { _parameters.Add(parameter); }

        public IEnumerable<IParameter> Parameters
        { get { return _parameters; } }

        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }

        public MemberKind MemberKind
        { get { return MemberKind.Method; } }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            {
                return ReturnType.QualifiedName;
            }
            return base.RequestValue(name);
        }
    }
}
