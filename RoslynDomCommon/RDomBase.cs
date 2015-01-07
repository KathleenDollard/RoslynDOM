using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoslynDom
{
   /// <summary>
   /// Base class for Roslyn Dom navigation tree
   /// </summary>
   /// <remarks>
   /// Initialize must be called near end of the constructor. Existing RDom impelementations all do this.
   /// </remarks>
   public abstract class RDomBase : IDom, INotifyPropertyChanged
   {
      protected RDomBase()
      { Whitespace2Set = new Whitespace2Collection(); }

      protected RDomBase(IDom oldIDom)
         : this()
      {
         if (oldIDom == null) throw new NotImplementedException();
         var oldRDom = (RDomBase)oldIDom;
         Whitespace2Set = oldIDom.Whitespace2Set.Copy();
         var oldAsHasName = oldIDom as IHasName;
         var thisAsHasName = this as IHasName;
         if (oldAsHasName != null && thisAsHasName != null)
         { thisAsHasName.Name = oldAsHasName.Name; }

         var thisAsHasStructuredDocs = this as IHasStructuredDocumentation;
         if (thisAsHasStructuredDocs != null)
         {
            var otherAsHasStructuredDocs = (IHasStructuredDocumentation)oldIDom;
            thisAsHasStructuredDocs.StructuredDocumentation = otherAsHasStructuredDocs.StructuredDocumentation;
            thisAsHasStructuredDocs.Description = otherAsHasStructuredDocs.Description;
         }
      }

      /// <summary>
      ///
      /// </summary>
      /// <returns></returns>
      /// <remarks>
      /// Return type is object, not SyntaxNode to match interface
      /// </remarks>
      public abstract object RawItem { get; }

      public abstract object OriginalRawItem { get; }

      // TODO: Return the parent set to hidden
      public virtual IDom Parent { get; set; }

      public override string ToString()
      {
         var ret = base.ToString() + " : ";
         var thisHasNamespace = this as IHasNamespace;
         if (thisHasNamespace != null) return ret + thisHasNamespace.QualifiedName;
         var thisHasName = this as IHasName;
         if (thisHasName != null) return ret + thisHasName.Name;
         if (this is IStatement) return ret;
         if (this is IExpression) return ret + this.RawItem.ToString();

         return ret;
      }

      public virtual string ReportHierarchy()
      {
         return ReportHierarchy(false);
      }

      public virtual string ReportHierarchy(bool includeWhitespace)
      {
         var sb = new StringBuilder();
         var spaces = 2;
         var indentToAdd = new string(' ', spaces);
         var indent = "";
         var reversedAncestors = this.Ancestors.Reverse();
         foreach (var ancestor in reversedAncestors)
         {
            sb.AppendLine(indent + ancestor.ToString());
            indent += indentToAdd;
         }
         sb.AppendLine(indent + this.ToString());
         AppendChildHierarchy(this, sb, indent + indentToAdd, indentToAdd);
         return sb.ToString();
      }

      private static void AppendChildHierarchy(IDom item, StringBuilder sb, string indent, string indentToAdd)
      {
         foreach (var child in item.Children)
         {
            sb.AppendLine(indent + child.ToString());
            AppendChildHierarchy(child, sb, indent + indentToAdd, indentToAdd);
         }
      }

      public abstract object RequestValue(string propertyName);

      public object RequestValue(string propertyName, bool searchUpLogicalTree)
      {
         var ret = RequestValue(propertyName);
         var parent = this.Parent;
         while (ret == null && parent != null)
         {
            ret = parent.RequestValue(propertyName);
            parent = parent.Parent;
         }
         return ret;
      }

      public virtual bool Matches(IDom other)
      {
         var thisAsHasName = this as IHasName;
         var otherAsHasName = other as IHasName;
         if (thisAsHasName != null && otherAsHasName != null)
         { return thisAsHasName.Name == otherAsHasName.Name; }
         return false; // we can't test here
      }

      public bool SameIntent<TLocal>(TLocal other)
             where TLocal : class
      {
         return SameIntentInternal(other);
      }

      protected abstract bool SameIntentInternal<TLocal>(TLocal other)
                       where TLocal : class;

      public IEnumerable<IDom> Descendants
      {
         get
         {
            var list = new List<IDom>();
            var children = this.Children;
            foreach (var child in children)
            { list.AddRange(child.DescendantsAndSelf); }
            return list;
         }
      }

      public IEnumerable<IDom> DescendantsAndSelf
      {
         get
         {
            var list = new List<IDom>();
            list.Add(this);
            list.AddRange(Descendants);
            return list;
         }
      }

      public virtual IEnumerable<IDom> Children
      { get { return new List<IDom>(); } }

      public virtual IEnumerable<IDom> Ancestors
      {
         get
         {
            if (Parent == null) { return new List<IDom>(); } // top/end of recursion
            return Parent.AncestorsAndSelf;
         }
      }

      public IEnumerable<IDom> AncestorsAndSelf
      {
         get
         {
            var list = Ancestors.ToList();
            list.Insert(0, this);
            return list;
         }
      }

      public IEnumerable<IRoot> Roots
      {
         get
         {
            var rootGroup = this.Ancestors.OfType<IRootGroup>().FirstOrDefault();
            if (rootGroup != null) { return rootGroup.Roots; }
            return Ancestors.OfType<IRoot>();
         }
      }

      public Whitespace2Collection Whitespace2Set { get; private set; }

      public bool NeedsFormatting { get; set; }

      public virtual void EnsureNewLineAfter()
      { Whitespace2Set.ForceTrailing = "\r\n"; }

      public virtual void EnsureLeading(string whitespace)
      { Whitespace2Set.ForceLeading = whitespace; }

      public event PropertyChangedEventHandler PropertyChanged;

      protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
      {
         if (!EqualityComparer<T>.Default.Equals(field, value))
         {
            field = value;
            // TODO: Update to null conditionals in C# 6
            var handler = PropertyChanged;
            if (handler != null)
            {
               handler(this, new PropertyChangedEventArgs(name));
            }
         }
      }
   }
}