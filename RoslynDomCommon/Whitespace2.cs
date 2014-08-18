using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{


    public class Whitespace2Set : IEnumerable<Whitespace2>
    {
        private List<Whitespace2> list = new List<Whitespace2>();

        public Whitespace2Set()
        { }

        public Whitespace2Set(Whitespace2Set old)
        {
            foreach (var item in old.list)
            { list.Add(item.Copy()); }
        }

        public Whitespace2Set Copy()
        { return new Whitespace2Set(this); }

        public IEnumerator<Whitespace2> GetEnumerator()
        { return list.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return list.GetEnumerator(); }

        public void AddRange(IEnumerable<Whitespace2> newItems)
        {
            foreach (var item in newItems)
            {
                this[item.LanguageElement] = item;
            }
        }

        public Whitespace2 this[LanguageElement languageElement]
        {
            get
            {
                return list.Where(x => x.LanguageElement == languageElement).FirstOrDefault();
            }
            set
            {
                var old = this[languageElement];
                var pos = list.IndexOf(old);
                if (pos < 0)
                { list.Add(value); }
                else
                {
                    list.RemoveAt(pos);
                    list.Insert(pos, value);
                }
            }
        }

        public string Report()
        {
            var sb = new StringBuilder();
            foreach (var item in list)
            { sb.AppendLine(item.ToString()); }
            return sb.ToString();
        }
    }

    public class Whitespace2
    {
        public Whitespace2(LanguagePart languagePart, LanguageElement languageElement)
        {
            LanguagePart = languagePart;
            LanguageElement = languageElement;
        }

        public Whitespace2(Whitespace2 old)
        {
            LanguagePart = old.LanguagePart;
            LanguageElement = old.LanguageElement;
            LeadingWhitespace = old.LeadingWhitespace;
            TrailingWhitespace = old.TrailingWhitespace;
            TrailingComment = old.TrailingComment;
        }

        public Whitespace2 Copy()
        { return new Whitespace2(this); }

        public LanguagePart LanguagePart { get; private set; }
        public LanguageElement LanguageElement { get; private set; }
        public string LeadingWhitespace { get; set; }
        public string TrailingWhitespace { get; set; }
        public string TrailingComment { get; set; }

        public override string ToString()
        {
            return string.Format(@"Whitespace: {0} Leading: '{1}' Trailing: '{2}' Comment: '{3}'", LanguageElement, LeadingWhitespace, TrailingWhitespace, TrailingComment);
        }
    }
}
