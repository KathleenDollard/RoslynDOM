using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class RDomFactory
    {
        // This is to test PublicAnnotationFactory before DI transition
        private static PublicAnnotationFactory publicAnnotationFactory = new PublicAnnotationFactory();

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

        public static IRoot GetRootFromSyntaxTree(SyntaxTree tree)
        {
            var root = MakeRoot(tree);
            return root;
        }

        public static IRoot GetRootFromDocument(Document document)
        {
            if (document == null) { throw new InvalidOperationException(); }
            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
            return GetRootFromSyntaxTree(tree);
        }

        #region Private Methods

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
        private static RDomRoot MakeRoot(SyntaxTree tree)
        {
            // why are there attributes on a compilatoin unit?
            var kRoot = tree.GetRoot() as CompilationUnitSyntax;
            var members = ListUtilities.MakeList(kRoot, x => x.Members, x => MakeStemMember(x));
            var usings = ListUtilities.MakeList(kRoot, x => x.Usings, x => MakeUsingDirective(x));
            var publicAnnotations = publicAnnotationFactory.CreateFrom (kRoot).ToArray();
            return new RDomRoot(kRoot, members, usings, publicAnnotations);
        }


        private static IUsing MakeUsingDirective(UsingDirectiveSyntax x)
        {
            var publicAnnotations = publicAnnotationFactory.CreateFrom(x).ToArray();
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

        private static bool DoStatement<T>(StatementSyntax val,
                Func<T, IStatement> doAction, out IStatement retValue)
            where T : class
        {
            var item = val as T;
            retValue = item != null ? doAction(item) : null;
            return (retValue != null);
        }

        private static bool DoStatements<T>(StatementSyntax val,
                Action<T, List<IStatement>> doAction, List<IStatement> list)
            where T : class
        {
            var item = val as T;
            if (item == null) return false;
            var startCount = list.Count();
            doAction(item, list);
            return (list.Count() > startCount);
        }

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
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawNamespace).ToArray();
            return new RDomNamespace(rawNamespace, members, usings, publicAnnotations);
        }

        private static IMember MakeClass(ClassDeclarationSyntax rawClass)
        {
            var members = ListUtilities.MakeList(rawClass, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawClass).ToArray();
            return new RDomClass(rawClass, members, publicAnnotations);
        }

        private static IMember MakeStructure(StructDeclarationSyntax rawStruct)
        {
            var members = ListUtilities.MakeList(rawStruct, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawStruct).ToArray();
            return new RDomStructure(rawStruct, members, publicAnnotations);
        }

        private static IMember MakeInterface(InterfaceDeclarationSyntax rawInterface)
        {
            var members = ListUtilities.MakeList(rawInterface, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawInterface).ToArray();
            return new RDomInterface(rawInterface, members, publicAnnotations);
        }

        private static IMember MakeEnum(EnumDeclarationSyntax rawEnum)
        {
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawEnum).ToArray();
            return new RDomEnum(rawEnum, publicAnnotations);
        }
        private static IMember MakeMethod(MethodDeclarationSyntax rawMethod)
        {
            var parms = ListUtilities.MakeList(rawMethod, x => x.ParameterList.Parameters, x => MakeParameter(x));
            var statements = ListUtilities.MakeList(rawMethod, x => GetStatements(x), x => MakeStatement(x));
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawMethod).ToArray();
            return new RDomMethod(rawMethod, parms, statements, publicAnnotations);
        }

        private static IParameter MakeParameter(ParameterSyntax rawParm)
        {
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawParm).ToArray();
            return new RDomParameter(rawParm, publicAnnotations);
        }

        private static IStatement MakeStatement(StatementSyntax rawStatement)
        {
            IStatement ret;
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoStatement<BlockSyntax>(rawStatement, MakeBlockStatement, out ret)) { }
            else if (DoStatement<IfStatementSyntax>(rawStatement, MakeIfStatement, out ret)) { }
            return ret as IStatement;

            //ForEach,
            //For,
            //Empty,
            //Return,
            //Declaration,
            //Try,

            ////While,   variation of Do
            ////Else,    characteristic of If
            ////Using,   characteristic of Block
            ////Catch,   characteristic of Try
            ////Switch,   // can this be handled as a special case of if?
            //Break,    // probably have to support
            //Continue, // probably have to support
            //Throw,    // probably have to support

            ////Expression statements, // break this appart into two kinds
            //Invocation,
            //Assignment,

            //Special // (platform or lanuguage specific)
            ////Checked (block)
            ////Lock,
            ////Yield, Split into YieldBreak and YieldReturn (expression)

            // Planning to avoid unless someone has a scenario
            //Unsafe,
            //Fixed,
            //Goto,
            //Labeled,
        }

        private static IBlockStatement MakeBlockStatement(BlockSyntax rawBlock)
        {
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawBlock).ToArray();
            var statements = ListUtilities.MakeList(rawBlock, x => x.Statements, x => MakeStatement(x));
            return new RDomBlockStatement(rawBlock, statements, publicAnnotations);
        }

        private static IIfStatement MakeIfStatement(IfStatementSyntax rawIf)
        {
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawIf).ToArray();
            var hasBlock = false;
            var statements = new List<IStatement>();
            var block = rawIf.Statement as BlockSyntax;
            if (block != null)
            {
                hasBlock = true;
                statements.AddRange(ListUtilities.MakeList(rawIf, x => block.Statements, x => MakeStatement(x)));
            }
            else
            { statements.Add(MakeStatement(rawIf.Statement)); }
            IEnumerable<IIfStatement> elses = null;
            if (rawIf.Else != null)
            { elses = MakeElses(rawIf.Else.Statement); }
            return new RDomIfStatement(rawIf, hasBlock, statements, elses, publicAnnotations);
        }

        private static IEnumerable<IIfStatement> MakeElses(StatementSyntax rawElseStatement)
        {
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawElseStatement).ToArray();
            var elses = new List<IIfStatement>();
            var elseStatement = rawElseStatement as IfStatementSyntax;
            if (elseStatement != null)
            {
                elses.Add(MakeIfStatement(elseStatement));
                elses.AddRange(MakeElses(elseStatement.Else.Statement));
            }
            else
            {
                var statements = new List<IStatement>();
                bool hasBlock = false;
                var blockElseStatement = rawElseStatement as BlockSyntax;
                if (blockElseStatement != null)
                {
                    hasBlock = true;
                    statements.AddRange(ListUtilities.MakeList(rawElseStatement, x => blockElseStatement.Statements, x => MakeStatement(x)));
                }
                else
                { statements.Add(MakeStatement(rawElseStatement)); }
                elses.Add(new RDomIfStatement(null, hasBlock, statements, null, publicAnnotations));

            }
            return elses;
        }

        private static IEnumerable<IDeclarationStatement> MakeDeclarationStatement(LocalDeclarationStatementSyntax  rawDeclaration)
        {
            var list = new List<IDeclarationStatement>();
            var declaration = rawDeclaration.Declaration;
            var variables = declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var variable in variables)
            {
                var publicAnnotations = publicAnnotationFactory.CreateFrom(rawDeclaration).ToArray();
                list.Add(new RDomDeclarationStatement(rawDeclaration, declaration, variable, publicAnnotations));
            }
            return list;
        }

        private static IEnumerable<StatementSyntax> GetStatements(MethodDeclarationSyntax rawMethod)
        {
            if (rawMethod.Body == null) return new List<StatementSyntax>();
            return rawMethod.Body.Statements;
        }


        private static IEnumerable<StatementSyntax> GetStatements(StatementSyntax rawStatement)
        {
            var list = new List<StatementSyntax>();
            // This handles method and property
            if (LoadStatementListFromList<BlockSyntax>(rawStatement, x => x.Statements, list)) { return list; }

            //// These have nested block syntax 
            //if (LoadStatementListFromList<BlockSyntax>(rawStatement, x => x.Statements, list)) { return list; }
            //if (LoadStatementListFromList<CheckedStatementSyntax>(rawStatement, x => x.Block.Statements, list)) { return list; }

            //// These have a statement which often, but not always, holds a nested block
            //if (LoadStatementListFromItem<DoStatementSyntax>(rawStatement, x => x.Statement, list)) { return list; }
            //if (LoadStatementListFromItem<ForEachStatementSyntax>(rawStatement, x => x.Statement, list)) { return list; }
            //if (LoadStatementListFromItem<ForStatementSyntax>(rawStatement, x => x.Statement, list)) { return list; }
            //if (LoadStatementListFromItem<LockStatementSyntax>(rawStatement, x => x.Statement, list)) { return list; }
            //if (LoadStatementListFromItem<WhileStatementSyntax>(rawStatement, x => x.Statement, list)) { return list; }
            //if (LoadStatementListFromItem<UsingStatementSyntax>(rawStatement, x => x.Statement, list)) { return list; }

            //// These have more than one statement block
            //if (LoadStatementListFromItem<IfStatementSyntax>(rawStatement, x => x.Statement, list)) { return list; } // if has a second block for else
            //if (LoadStatementListFromList<TryStatementSyntax>(rawStatement, x => x.Block.Statements, list)) { return list; }

            //// These do not have nested blocksyntax or statements
            //// BreakStatementSyntax     
            //// ContinueStatementSyntax  
            //// EmptyStatementSyntax     
            //// ExpressionStatementSyntax
            //// LocalDeclarationStatement
            //// ReturnStatementSyntax    
            //// ThrowStatementSyntax     
            //// YieldStatementSyntax

            //// This is a very weird special case
            //// SwitchStatementSyntax

            //// These are items I currently do not plan to support - although need to confirm that switch doesn't use labeled statements
            //// FixedStatementSyntax     
            //// GotoStatementSyntax      
            //// LabeledStatementSyntax   
            //// UnsafeStatementSyntax

            return list;
        }

        private static bool LoadStatementListFromList<T>(
                StatementSyntax rawStatement,
                Func<T, IEnumerable<StatementSyntax>> getList,
                List<StatementSyntax> list)
            where T : StatementSyntax
        {
            var typed = rawStatement as T;
            if (typed == null) return false;
            list.AddRange(getList(typed));
            return true;
        }

        private static bool LoadStatementListFromItem<T>(StatementSyntax rawStatement, Func<T, StatementSyntax> getItem, List<StatementSyntax> list)
             where T : StatementSyntax
        {
            var typed = rawStatement as T;
            if (typed == null) return false;
            list.Add(getItem(typed));
            return true;
        }

        private static IMember MakeProperty(PropertyDeclarationSyntax rawProperty)
        {
            // VB will have property parameters
            var parms = new List<IParameter>();
            var getAccessor = MakeAccessor(rawProperty.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.GetAccessorDeclaration).FirstOrDefault());
            var setAccessor = MakeAccessor(rawProperty.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.SetAccessorDeclaration).FirstOrDefault());
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawProperty).ToArray();
            return new RDomProperty(rawProperty, parms, getAccessor, setAccessor, publicAnnotations);
        }

        private static IAccessor MakeAccessor(AccessorDeclarationSyntax rawAccessor)
        {
            if (rawAccessor == null) return null;
            var statements = ListUtilities.MakeList(rawAccessor, x => GetStatements(rawAccessor.Body), x => MakeStatement(x));
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawAccessor).ToArray();
            return new RDomPropertyAccessor(rawAccessor, statements, publicAnnotations);
        }

        private static IMember MakeInvalidMember(SyntaxNode rawItem)
        {
            var publicAnnotations = publicAnnotationFactory.CreateFrom(rawItem).ToArray();
            return new RDomInvalidTypeMember(rawItem, publicAnnotations);
        }

        private static void MakeFields(FieldDeclarationSyntax rawField, List<IMember> list)
        {
            var declarators = rawField.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var publicAnnotations = publicAnnotationFactory.CreateFrom(rawField).ToArray();
                list.Add(new RDomField(rawField, decl, publicAnnotations));
            }
        }

        #endregion
        
    }
}
