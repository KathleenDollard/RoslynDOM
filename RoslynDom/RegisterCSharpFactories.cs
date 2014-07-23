using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class CSharpFactory
    {
        public static void Register()
        {
            RDomFactoryHelper.Register<IRoot>(new RDomRootFactoryHelper());
            RDomFactoryHelper.Register<IStemMemberCommentWhite>(new RDomStemMemberFactoryHelper());
            RDomFactoryHelper.Register<ITypeMemberCommentWhite>(new RDomTypeMemberFactoryHelper());
            RDomFactoryHelper.Register<IStatementCommentWhite>(new RDomStatementFactoryHelper());
            RDomFactoryHelper.Register<IExpression>(new RDomExpressionFactoryHelper());
            RDomFactoryHelper.Register<IMisc>(new RDomMiscFactoryHelper());
        }
    }

    public class RDomRootFactoryHelper : RDomFactoryHelper<IRoot>
    { internal RDomRootFactoryHelper() { } }

    public class RDomStemMemberFactoryHelper : RDomFactoryHelper<IStemMemberCommentWhite >
    { internal RDomStemMemberFactoryHelper() { } }

    public class RDomTypeMemberFactoryHelper : RDomFactoryHelper<ITypeMemberCommentWhite>
    { internal RDomTypeMemberFactoryHelper() { } }

    public class RDomStatementFactoryHelper : RDomFactoryHelper<IStatementCommentWhite>
    { internal RDomStatementFactoryHelper() { } }

    public class RDomExpressionFactoryHelper : RDomFactoryHelper<IExpression>
    { internal RDomExpressionFactoryHelper() { } }

    public class RDomMiscFactoryHelper : RDomFactoryHelper<IMisc>
    { internal RDomMiscFactoryHelper() { } }

}
