using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Field assignments in the form "Type x, y, z" are not yet supported 
    /// and when they are they will be loaded as separate fields (rather 
    /// obviously). At that point, the variable declaration will need to be held in
    /// the class. 
    /// </remarks>
    public class RDomField : RDomSyntaxNodeBase<FieldDeclarationSyntax>, IField
    {
        internal RDomField(FieldDeclarationSyntax rawItem) : base(rawItem) { }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private VariableDeclaratorSyntax variableDeclaration
        {
            get { return TypedRawItem.Declaration.Variables.First(); }
        }
        public override string Name
        {
            get { return variableDeclaration.Identifier.NameFrom(); }
        }

        public override string QualifiedName
        {
            get { return variableDeclaration.Identifier.QualifiedNameFrom(); }
        }

        public override string BestInContextName
        {
            get { return variableDeclaration.Identifier.QualifiedNameFrom(); }
        }
    }
}
