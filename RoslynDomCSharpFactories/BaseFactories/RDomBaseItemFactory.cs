using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
    public abstract class RDomBaseItemFactory<T, TSyntax, TKind> : IRDomFactory
        where T : class, TKind  // This isn't stupid - TKind is the broader category, T is the specific 
        where TSyntax : SyntaxNode
        where TKind : class, IDom
    {
        // until move to C# 6 - I want to support name of as soon as possible
        [ExcludeFromCodeCoverage]
        protected static string nameof<T2>(T2 value) { return ""; }

        public RDomBaseItemFactory(RDomCorporation corporation)
        {
            Corporation = corporation;
        }

        protected RDomCorporation Corporation { get; private set; }

        private ICSharpBuildSyntaxWorker _buildSyntaxWorker;
        internal ICSharpBuildSyntaxWorker BuildSyntaxWorker
        {
            get
            {
                if (_buildSyntaxWorker == null) { _buildSyntaxWorker = (ICSharpBuildSyntaxWorker)Corporation.Worker.BuildSyntaxWorker; }
                return _buildSyntaxWorker;
            }

        }

        private ICSharpCreateFromWorker _createFromWorker;
        internal ICSharpCreateFromWorker CreateFromWorker
        {
            get
            {
                if (_createFromWorker == null) { _createFromWorker = (ICSharpCreateFromWorker)Corporation.Worker.CreateFromWorker; }
                return _createFromWorker;
            }

        }

        public virtual RDomPriority Priority
        {
            get
            {
                if (this.GetType().IsConstructedGenericType) { return RDomPriority.Fallback; }
                return RDomPriority.Normal;
            }
        }

        public abstract IEnumerable<SyntaxNode> BuildSyntax(IDom item);

        public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is TSyntax);
        }

        public virtual bool CanBuildSyntax(IDom item)
        {
            // TODO: use this to call correct IStem or IType version of class, interface, structure and enum
            return true;
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
        public IEnumerable<IDom> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model, bool skipWhitespace)
        {
            var ret = new List<TKind>();

            var newItems = CreateListFrom(syntaxNode, parent, model);
            // Whitespace and comments have to appear before new items 
            if (typeof(IStatement).IsAssignableFrom(typeof(T))
               || typeof(ITypeMember).IsAssignableFrom(typeof(T))
               || typeof(IStemMember).IsAssignableFrom(typeof(T)))
            {
                var newItem = newItems.FirstOrDefault();
                if (newItem != null)
                {
                    var whiteComment = CreateFromWorker.GetCommentWhite(syntaxNode, newItem, model);
                    ret.AddRange(whiteComment.OfType<TKind>());
                }
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
            // This can't be tested, but making it abstract means we have untestable code in seven derived classes. 
            Guardian.Assert.NeitherCreateFromNorListOverridden<TKind>(this.GetType(), syntaxNode);
            return null;
        }
    }

    public abstract class RDomRootContainerFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IRoot>
             where T : class, IRoot
             where TSyntax : SyntaxNode
    {
        public RDomRootContainerFactory(RDomCorporation corporation)
            : base(corporation)
        { }
    }

    public abstract class RDomStemMemberFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IStemMemberCommentWhite>
            where T : class, IStemMemberCommentWhite
            where TSyntax : SyntaxNode
    {
        public RDomStemMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }
    }


    public abstract class RDomTypeMemberFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, ITypeMemberCommentWhite>
            where T : class, ITypeMemberCommentWhite
            where TSyntax : SyntaxNode
    {
        public RDomTypeMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }
    }


    public abstract class RDomStatementFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IStatementCommentWhite>
            where T : class, IStatementCommentWhite
            where TSyntax : SyntaxNode
    {
        public RDomStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }
    }


    public abstract class RDomExpressionFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IExpression>
             where T : class, IExpression
             where TSyntax : SyntaxNode
    {
        public RDomExpressionFactory(RDomCorporation corporation)
            : base(corporation)
        { }
    }


    public abstract class RDomMiscFactory<T, TSyntax> : RDomBaseItemFactory<T, TSyntax, IMisc>
             where T : class, IMisc
             where TSyntax : SyntaxNode
    {
        public RDomMiscFactory(RDomCorporation corporation)
            : base(corporation)
        { }
    }

}
