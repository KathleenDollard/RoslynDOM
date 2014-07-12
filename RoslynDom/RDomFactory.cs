using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomFactory
    {
        public static IRoot GetRootFromFile(string fileName)
        {
            var code = File.ReadAllText(fileName);
            return GetRootFromString(code);
        }

        public static IRoot GetRootFromString(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            return GetRootFromSyntaxTree(tree);
        }

        public static IRoot GetRootFromDocument(Document document)
        {
            if (document == null) { throw new InvalidOperationException(); }
            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
            return GetRootFromSyntaxTree(tree);
        }

        public static IRoot GetRootFromSyntaxTree(SyntaxTree tree)
        {
            var container = (new RDomFactoryBootstrapper()).ConfigureContainer();
            var publicAnnotationFactory = container.ResolveAll<IPublicAnnotationFactory>().FirstOrDefault();
            //var stemMemberFactories = container.ResolveAll<IRDomFactory<IStemMember>>();
            //var memberFactories = container.ResolveAll<IRDomFactory<IMember>>();
            var statementFactories = container.ResolveAll<IRDomFactory<IStatement>>();
            var statementHelper = container.Resolve<RDomStatementFactoryHelper>();
            var root = RDomFactory2.MakeRoot(tree, container);
            return root;
        }

    }

}
