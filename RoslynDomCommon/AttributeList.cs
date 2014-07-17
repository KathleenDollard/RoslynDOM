using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace RoslynDom.Common
{
    public class AttributeList : IEnumerable<IAttribute>
    {
        // TODO: Add the move semantics
        private IList<IAttribute> _attributes = new List<IAttribute>();

        public void RemoveAttribute(IAttribute attribute)
        { _attributes.Remove(attribute); }

        public void AddOrMoveAttribute(IAttribute attribute)
        { _attributes.Add(attribute); }

        public void AddOrMoveAttributeRange(IEnumerable<IAttribute> attributes)
        {
            foreach (var attribute in attributes)
            { AddOrMoveAttribute(attribute); }
        }

        public IEnumerable<IAttribute> Attributes
        { get { return _attributes; } }

        public AttributeList Copy()
        {
            var list = new AttributeList();
            foreach (var attribute in _attributes )
            {
                list.AddOrMoveAttribute(attribute.Copy());
            }
            return list;
        }

        public IEnumerator<IAttribute> GetEnumerator()
        { return _attributes.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return _attributes.GetEnumerator(); }
    }
}
