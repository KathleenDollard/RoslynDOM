using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
   public abstract class RDomBaseStemContainer<T, TSymbol> : RDomBase<T, TSymbol>, IStemContainer
       where T : class, IDom<T>
       where TSymbol : ISymbol
   {
      private RDomCollection<IStemMemberAndDetail> _members;

      internal RDomBaseStemContainer(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability",
          "CA1502:AvoidExcessiveComplexity",
          Justification = "This method is very complex, but is a core method, is short, and follows a pattern, so complexity is OK")]
      internal RDomBaseStemContainer(T oldIDom)
           : base(oldIDom)
      {
         Initialize();
         var oldRDom = oldIDom as RDomBaseStemContainer<T, TSymbol>;
         RDomCollection<IStemMemberAndDetail>.Copy(oldRDom.StemMembersAll, _members);
         //_members = oldRDom.StemMembersAll.Copy(this);
      }

      private void Initialize()
      {
         _members = new RDomCollection<IStemMemberAndDetail>(this);
      }

      public bool AddOrMoveMember(IDom item)
      { return _members.AddOrMove(item); }

      public bool RemoveMember(IDom item)
      { return _members.Remove(item); }

      public bool InsertOrMoveMember(int index, IDom item)
      { return _members.InsertOrMove(index, item); }

      public IEnumerable<IDom> GetMembers()
      { return StemMembersAll.ToList(); }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(_members);
            return list;
         }
      }

      public string Namespace
      { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

      public string QualifiedName
      { get { return RoslynUtilities.GetQualifiedName(this); } }

      public void ClearStemMembers()
      { _members.Clear(); }

      public IUsingDirective AddUsingDirective(IUsingDirective usingDirective)
      {
         StemMembersAll.AddOrMove(usingDirective);
         return usingDirective;
      }

      public IUsingDirective AddUsingDirective(string usingName)
      {
         var usingDirective = new RDomUsingDirective(usingName);
         return AddUsingDirective(usingDirective);
      }

      public IEnumerable<IUsingDirective> AddUsingDirectives(params IUsingDirective[] usingDirectives)
      {
         StemMembersAll.AddOrMoveRange(usingDirectives);
         return usingDirectives;
      }

      public IEnumerable<IUsingDirective> AddUsingDirectives(params string[] usingNames)
      {
         var usingDirectives = usingNames.Select(n => new RDomUsingDirective(n)).ToArray();
         return AddUsingDirectives(usingDirectives);
      }

      public INamespace AddNamespace(INamespace nspace)
      {
         StemMembersAll.AddOrMove(nspace);
         return nspace;
      }

      public INamespace AddNamespace(string namespaceName)
      {
         var nspace = new RDomNamespace(namespaceName);
         return AddNamespace(nspace);
      }

      public IEnumerable<INamespace> AddNamespaces(params INamespace[] namespaces)
      {
         StemMembersAll.AddOrMoveRange(namespaces);
         return namespaces;
      }

      public IEnumerable<INamespace> AddNamespaces(params string[] namespaceName)
      {
         var namespaces = namespaceName.Select(n => new RDomNamespace(n)).ToArray();
         return AddNamespaces(namespaces);
      }

      public RDomCollection<IStemMemberAndDetail> StemMembersAll
      { get { return _members; } }

      public IEnumerable<IStemMember> StemMembers
      { get { return _members.OfType<IStemMember>().ToList(); } }

      public IEnumerable<INamespace> DescendantNamespaces
      { get { return RoslynDomUtilities.GetDescendantNamespaces(this); } }

      public IEnumerable<INamespace> Namespaces
      { get { return RoslynDomUtilities.GetNonEmptyNamespaces(this); } }

      public IEnumerable<INamespace> ChildNamespaces
      { get { return StemMembers.OfType<INamespace>().ToList(); } }

      public IEnumerable<IClass> Classes
      { get { return StemMembers.OfType<IClass>().ToList(); } }

      public IEnumerable<IInterface> Interfaces
      { get { return StemMembers.OfType<IInterface>().ToList(); } }

      public IEnumerable<IStructure> Structures
      { get { return StemMembers.OfType<IStructure>().ToList(); } }

      public IEnumerable<IEnum> Enums
      { get { return StemMembers.OfType<IEnum>().ToList().ToList(); } }

      public IEnumerable<IUsingDirective> UsingDirectives
      { get { return StemMembers.OfType<IUsingDirective>().ToList().ToList(); } }

      public IEnumerable<IType> Types
      { get { return StemMembers.OfType<IType>().ToList().ToList(); } }
   }
}
