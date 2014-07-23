using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
    public abstract class RDomBaseItemFactory<T, TSyntax, TKind> : IRDomFactory<TKind>
        where T : class, TKind  // This isn't stupid - TKind is the broader category, T is the specific 
        where TSyntax : SyntaxNode
        where TKind : class, IDom
    {
        public virtual FactoryPriority Priority
        {
            get
            {
                if (this.GetType().IsConstructedGenericType) { return FactoryPriority.Fallback; }
                return FactoryPriority.Normal;
            }
        }

        public abstract IEnumerable<SyntaxNode> BuildSyntax(TKind item);

        public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is TSyntax);
        }

        /// <summary>
        /// This is the key method for creating new RoslynDom elements. You can create new factories
        /// by overriding the CreateListFrom or CreateItemFrom methods depending on whether you are 
        /// creating a list or single element respectively 
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <param name="parent"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <remarks>
        /// Lists are created when multiple items are declared on a line. This allows simple access to 
        /// variables and fields in the predominate case with only one variable per line. Currently this is not 
        /// recreated on output but the design intent was to have variable groups with arbitrarily defined
        /// IDs and probably a position in the list. These two properties would be added to Field and Declaration
        /// RDom items, and possibly interfaces. This would be sufficient for recreation of lists
        /// <para/>
        /// Use CanCreate and Priority to control how your factory is selected. Highest priority wins and 
        /// you can add to the enum values (as in Normal + 1)
        /// </remarks>
        public IEnumerable<TKind> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = new List<TKind>();

            var newItems = CreateListFrom(syntaxNode, parent, model);
            // Whitespace and comments have to appear before list items in the string
            var newItem = newItems.FirstOrDefault();
            if (newItem != null)
            {
                var whiteComment = RDomFactoryHelper.GetCommentWhite(syntaxNode, newItem, model);
                ret.AddRange(whiteComment.OfType<TKind>());
            }
            ret.AddRange(newItems);
            return ret;
        }

        protected virtual IEnumerable<TKind> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return new TKind[] { CreateItemFrom(syntaxNode, parent, model) };
        }

        protected virtual TKind CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as TSyntax;
            var newItem = Activator.CreateInstance(
                        typeof(T),
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                        new object[] { syntax, parent, model }, null);
            var itemAsT = newItem as T;
            InitializeItem(itemAsT, syntax, model);
            return itemAsT;
        }

        public virtual void InitializeItem(T newItem, TSyntax syntax, SemanticModel model)
        { return; }

        public virtual void Initialize(T newItem, TSyntax syntaxNode, SemanticModel model, string name)
        {
            var itemAsHasName = newItem as IHasName;
            if (itemAsHasName != null)
            { itemAsHasName.Name = name; }

            var itemHasAttributes = newItem as IHasAttributes;
            if (itemHasAttributes != null)
            {
                var attributes = RDomFactoryHelper.CreateAttributeFrom(syntaxNode, newItem, model);
                itemHasAttributes.Attributes.AddOrMoveAttributeRange(attributes);
            }
        }
    }

    public abstract class RDomRootContainerFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IRoot>
             where T : class, IRoot
             where TSyntax : SyntaxNode
    { }

    public abstract class RDomStemMemberFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IStemMemberCommentWhite>
            where T : class, IStemMemberCommentWhite
            where TSyntax : SyntaxNode
    { }


    public abstract class RDomTypeMemberFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, ITypeMemberCommentWhite>
            where T : class, ITypeMemberCommentWhite
            where TSyntax : SyntaxNode
    { }


    public abstract class RDomStatementFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IStatementCommentWhite>
            where T : class, IStatementCommentWhite
            where TSyntax : SyntaxNode
    { }


    public abstract class RDomExpressionFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IExpression>
             where T : class, IExpression
             where TSyntax : SyntaxNode
    { }


    public abstract class RDomMiscFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IMisc>
             where T : class, IMisc
             where TSyntax : SyntaxNode
    { }
}
