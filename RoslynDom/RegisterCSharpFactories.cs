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
            //RDomFactoryHelper.RegisterPublicAnnotationFactory(new PublicAnnotationFactory());
            RDomFactoryHelper.Register<IRoot>(new RDomRootFactoryHelper());
            RDomFactoryHelper.Register<IStemMember>(new RDomStemMemberFactoryHelper());
            RDomFactoryHelper.Register<ITypeMember>(new RDomTypeMemberFactoryHelper());
            RDomFactoryHelper.Register<IStatement>(new RDomStatementFactoryHelper());
            RDomFactoryHelper.Register<IExpression>(new RDomExpressionFactoryHelper());
            RDomFactoryHelper.Register<IMisc>(new RDomMiscFactoryHelper());
        }
    }

    public class RDomRootFactoryHelper : RDomFactoryHelper<IRoot>
    { internal RDomRootFactoryHelper() { } }

    public class RDomStemMemberFactoryHelper : RDomFactoryHelper<IStemMember>
    { internal RDomStemMemberFactoryHelper() { } }

    public class RDomTypeMemberFactoryHelper : RDomFactoryHelper<ITypeMember>
    { internal RDomTypeMemberFactoryHelper() { } }

    public class RDomStatementFactoryHelper : RDomFactoryHelper<IStatement>
    { internal RDomStatementFactoryHelper() { } }

    public class RDomExpressionFactoryHelper : RDomFactoryHelper<IExpression>
    { internal RDomExpressionFactoryHelper() { } }

    public class RDomMiscFactoryHelper : RDomFactoryHelper<IMisc>
    { internal RDomMiscFactoryHelper() { } }

}
