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
        private VariableDeclaratorSyntax _varSyntax;

        internal RDomField(FieldDeclarationSyntax rawItem, VariableDeclaratorSyntax varSyntax) : base(rawItem)
        { _varSyntax = varSyntax; }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return this.AttributesFrom();
            }
        }


        private VariableDeclaratorSyntax variableDeclaration
        {
            get { return _varSyntax; }
        }

        public override string Name
        {
            get { return variableDeclaration.Identifier.NameFrom(); }
        }

        public override string QualifiedName
        {
            get
            {
                // TODO: Manage static member's qualified names
                throw new InvalidOperationException("You can't get qualified name for an instance field");
            }
        }

        public override ISymbol Symbol
        {
            get
            {
                return base.GetSymbol(_varSyntax);
            }
        }

    }
}
