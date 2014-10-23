using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IStemContainer : INestedContainer, IContainer
   {
      IEnumerable<IUsingDirective> UsingDirectives { get; }
      RDomCollection<IStemMemberCommentWhite> StemMembersAll { get; }
      IEnumerable<IStemMember> StemMembers { get; }

      /// <summary>
      /// Namespaces that contain code.
      /// <para/>
      ///  Namespace nesting is expanded in RoslynDom
      /// and a grouping marker allows recreation of your layout. This property returns
      /// only the namespaces that actually contain code. In most files, this will be the
      /// singe namespace that contains code. 
      /// </summary>
      /// <returns>
      /// Non-empty namespaces, regardless of how you layout your code
      /// </returns>
      IEnumerable<INamespace> Namespaces { get; }

      /// <summary>
      /// Namespaces that are direct descendants, regardless of how you layout your code.
      /// <para/>
      /// Namespace nesting is expanded in RoslynDom so each namespace contains exactly one part
      /// of a complex namespace name N1.N2.. The child namespaces property would return only N1.
      /// </summary>
      /// <returns>
      /// Namespaces that are direct descendants, regardless of how you layout your code
      /// </returns>
      IEnumerable<INamespace> ChildNamespaces { get; }

      /// <summary>
      /// Namespaces that are descendants, regardless of how you layout your code.
      /// <para/>
      /// Namespace nesting is expanded in RoslynDom so each namespace contains exactly one part
      /// of a complex namespace name N1, N2, N3... In that case, Descendant namespaces would 
      /// be an IEnumerable of N1, N2, N3. 
      /// </summary>
      /// <returns>
      /// Namespaces that are descendants, regardless of how you layout your code
      /// </returns>
      IEnumerable<INamespace> DescendantNamespaces { get; }

      IUsingDirective AddUsingDirective(IUsingDirective usingDirective);
      IUsingDirective AddUsingDirective(string usingName);
      IEnumerable<IUsingDirective> AddUsingDirectives(params IUsingDirective[] usingDirective);
      IEnumerable<IUsingDirective> AddUsingDirectives(params string[] usingName);

      INamespace AddNamespace(INamespace nspace);
      INamespace AddNamespace(string namespaceName);
      IEnumerable<INamespace> AddNamespaces(params INamespace[] namespaces);
      IEnumerable<INamespace> AddNamespaces(params string[] namespaceName);

      //IType AddType(IType Type);
      //IType AddType(string typeName);
      //IEnumerable<IType> AddTypes(params IType[] Type);
      //IEnumerable<IType> AddTypes(params string[] typeName);
   }
}