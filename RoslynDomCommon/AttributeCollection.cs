using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RoslynDom.Common
{
   public class AttributeCollection : IEnumerable<IAttribute>
   {
      // TODO: Add the move semantics
      private IList<IAttribute> _attributes = new List<IAttribute>();

      public void RemoveAttribute(IAttribute attribute)
      { _attributes.Remove(attribute); }

      public void AddOrMoveAttribute(IAttribute attribute)
      { _attributes.Add(attribute); }

      public void AddOrMoveAttributeRange(IEnumerable<IAttribute> attributes)
      {
         if (attributes == null) throw new NotImplementedException();
         foreach (var attribute in attributes)
         { AddOrMoveAttribute(attribute); }
      }

      public IEnumerable<IAttribute> Attributes
      { get { return _attributes; } }

      public IEnumerator<IAttribute> GetEnumerator()
      { return _attributes.GetEnumerator(); }

      [ExcludeFromCodeCoverage]
      IEnumerator IEnumerable.GetEnumerator()
      { return _attributes.GetEnumerator(); }
   }
}