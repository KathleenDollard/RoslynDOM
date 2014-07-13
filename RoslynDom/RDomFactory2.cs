using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class RDomFactory2
    {
        // This is to test PublicAnnotationFactory before DI transition
        //private static IPublicAnnotationFactory _publicAnnotationFactory ;
        //private static RDomStatementFactoryHelper _statementFactoryHelper;

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        /// <remarks>
        /// I'm not currently supporting attributes because I don't understand what attributes
        /// are on the compilation unit. Probably assembly attributes, but I want to get it first <br/>
        /// <br/>
        /// I currnelty think I will NOT support externs. They feel rare to me and something that the
        /// people who understand them can drop to the raw item to work with <br/>
        /// </remarks>
        internal static RDomRoot MakeRoot(SyntaxTree tree)
        {
            //_publicAnnotationFactory =  container.ResolveAll<IPublicAnnotationFactory>().FirstOrDefault();
            //_statementFactoryHelper = RDomFactoryHelper.RDomStatementFactoryHelper ;
            // why are there attributes on a compilatoin unit?
            var kRoot = tree.GetRoot() as CompilationUnitSyntax;
            var members = ListUtilities.MakeList(kRoot, x => x.Members, x => MakeStemMember(x));
            var usings = ListUtilities.MakeList(kRoot, x => x.Usings, x => MakeUsingDirective(x));
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(kRoot).ToArray();
            return new RDomRoot(kRoot, members, usings, publicAnnotations);
        }

        #region Private Methods

        private static IUsing MakeUsingDirective(UsingDirectiveSyntax x)
        {
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(x).ToArray();
            return new RDomUsing(x, publicAnnotations);
        }

        private static bool DoMember<T>(MemberDeclarationSyntax val,
                Func<T, IMember> doAction, out IMember retValue)
            where T : class
        {
            var item = val as T;
            retValue = item != null ? doAction(item) : null;
            return (retValue != null);
        }

        private static bool DoMembers<T>(MemberDeclarationSyntax val,
                Action<T, List<IMember>> doAction, List<IMember> list)
            where T : class
        {
            var item = val as T;
            if (item == null) return false;
            var startCount = list.Count();
            doAction(item, list);
            return (list.Count() > startCount);
        }

        //private static bool DoStatement<T>(StatementSyntax val,
        //        Func<T, IStatement> doAction, out IStatement retValue)
        //    where T : class
        //{
        //    var item = val as T;
        //    retValue = item != null ? doAction(item) : null;
        //    return (retValue != null);
        //}

        //private static bool DoStatements<T>(StatementSyntax val,
        //        Action<T, List<IStatement>> doAction, List<IStatement> list)
        //    where T : class
        //{
        //    var item = val as T;
        //    if (item == null) return false;
        //    var startCount = list.Count();
        //    doAction(item, list);
        //    return (list.Count() > startCount);
        //}

        /// <summary>
        /// Creates namespace and class
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private static IStemMember MakeStemMember(MemberDeclarationSyntax rawMember)
        {
            IMember ret;
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMember<NamespaceDeclarationSyntax>(rawMember, MakeNamespace, out ret)) { }
            else if (DoMember<ClassDeclarationSyntax>(rawMember, MakeClass, out ret)) { }
            else if (DoMember<InterfaceDeclarationSyntax>(rawMember, MakeInterface, out ret)) { }
            else if (DoMember<StructDeclarationSyntax>(rawMember, MakeStructure, out ret)) { }
            else if (DoMember<EnumDeclarationSyntax>(rawMember, MakeEnum, out ret)) { }
            return ret as IStemMember;
        }

        /// <summary>
        /// Creates methods, properties and nested types
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private static IEnumerable<ITypeMember> MakeTypeMembers(MemberDeclarationSyntax rawMember)
        {
            IMember item;
            var retList = new List<IMember>();
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMember<MethodDeclarationSyntax>(rawMember, MakeMethod, out item)) { }
            else if (DoMember<PropertyDeclarationSyntax>(rawMember, MakeProperty, out item)) { }
            else if (DoMember<ClassDeclarationSyntax>(rawMember, MakeClass, out item)) { }
            else if (DoMember<InterfaceDeclarationSyntax>(rawMember, MakeInterface, out item)) { }
            else if (DoMember<StructDeclarationSyntax>(rawMember, MakeStructure, out item)) { }
            else if (DoMember<EnumDeclarationSyntax>(rawMember, MakeEnum, out item)) { }
            else if (DoMembers<FieldDeclarationSyntax>(rawMember, MakeFields, retList)) { }
            else if (DoMember<SyntaxNode>(rawMember, MakeInvalidMember, out item)) { }
            else throw new InvalidOperationException();
            if (item != null) retList.Add(item);
            return retList.OfType<ITypeMember>();
        }

        private static IMember MakeNamespace(NamespaceDeclarationSyntax rawNamespace)
        {
            var members = ListUtilities.MakeList(rawNamespace, x => x.Members, x => MakeStemMember(x));
            var usings = ListUtilities.MakeList(rawNamespace, x => x.Usings, x => MakeUsingDirective(x));
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawNamespace).ToArray();
            return new RDomNamespace(rawNamespace, members, usings, publicAnnotations);
        }

        private static IMember MakeClass(ClassDeclarationSyntax rawClass)
        {
            var members = ListUtilities.MakeList(rawClass, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawClass).ToArray();
            return new RDomClass(rawClass, members, publicAnnotations);
        }

        private static IMember MakeStructure(StructDeclarationSyntax rawStruct)
        {
            var members = ListUtilities.MakeList(rawStruct, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawStruct).ToArray();
            return new RDomStructure(rawStruct, members, publicAnnotations);
        }

        private static IMember MakeInterface(InterfaceDeclarationSyntax rawInterface)
        {
            var members = ListUtilities.MakeList(rawInterface, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawInterface).ToArray();
            return new RDomInterface(rawInterface, members, publicAnnotations);
        }

        private static IMember MakeEnum(EnumDeclarationSyntax rawEnum)
        {
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawEnum).ToArray();
            return new RDomEnum(rawEnum, publicAnnotations);
        }

        private static IMember MakeMethod(MethodDeclarationSyntax rawMethod)
        {
            var parms = ListUtilities.MakeList(rawMethod, x => x.ParameterList.Parameters, x => MakeParameter(x));
            var statements = ListUtilities.MakeList(rawMethod, x => GetStatements(x), x => MakeStatement(x));
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawMethod).ToArray();
            return new RDomMethod(rawMethod, parms, statements, publicAnnotations);
        }

        private static IParameter MakeParameter(ParameterSyntax rawParm)
        {
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawParm).ToArray();
            return new RDomParameter(rawParm, publicAnnotations);
        }

        private static IEnumerable<IStatement> MakeStatement(StatementSyntax rawStatement)
        {
           return  RDomFactoryHelper.RDomStatementFactoryHelper.MakeItem(rawStatement);
        }

         private static IEnumerable<StatementSyntax> GetStatements(MethodDeclarationSyntax rawMethod)
        {
            if (rawMethod.Body == null) return new List<StatementSyntax>();
            return rawMethod.Body.Statements;
        }

        private static IEnumerable<StatementSyntax> GetStatements(BlockSyntax rawStatement)
        {
            if (rawStatement == null) return new List<StatementSyntax>();
            return rawStatement.Statements;
        }

         private static IMember MakeProperty(PropertyDeclarationSyntax rawProperty)
        {
            // VB will have property parameters
            var parms = new List<IParameter>();
            var getAccessor = MakeAccessor(rawProperty.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.GetAccessorDeclaration).FirstOrDefault());
            var setAccessor = MakeAccessor(rawProperty.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.SetAccessorDeclaration).FirstOrDefault());
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawProperty).ToArray();
            return new RDomProperty(rawProperty, parms, getAccessor, setAccessor, publicAnnotations);
        }

        private static IAccessor MakeAccessor(AccessorDeclarationSyntax rawAccessor)
        {
            if (rawAccessor == null) return null;
            var statements = ListUtilities.MakeList(rawAccessor, x => GetStatements(rawAccessor.Body), x => MakeStatement(x));
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawAccessor).ToArray();
            return new RDomPropertyAccessor(rawAccessor, statements, publicAnnotations);
        }

        private static IMember MakeInvalidMember(SyntaxNode rawItem)
        {
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawItem).ToArray();
            return new RDomInvalidTypeMember(rawItem, publicAnnotations);
        }

        private static void MakeFields(FieldDeclarationSyntax rawField, List<IMember> list)
        {
            var declarators = rawField.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(rawField).ToArray();
                list.Add(new RDomField(rawField, decl, publicAnnotations));
            }
        }

        #endregion

    }
}
