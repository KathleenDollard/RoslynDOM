using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public class Whitespace2Collection : IEnumerable<Whitespace2>
   {
      private List<Whitespace2> list = new List<Whitespace2>();

      public Whitespace2Collection()
      { }

      public Whitespace2Collection(Whitespace2Collection old)
      {
         if (old == null) throw new NotImplementedException();
         foreach (var item in old.list)
         { list.Add(item.Copy()); }
      }

      public Whitespace2Collection Copy()
      { return new Whitespace2Collection(this); }

      public IEnumerator<Whitespace2> GetEnumerator()
      { return list.GetEnumerator(); }

      IEnumerator IEnumerable.GetEnumerator()
      { return list.GetEnumerator(); }

      public string ForceLeading { get; set; }
      public string ForceTrailing { get; set; }

      public void Add(Whitespace2 item)
      {
         if (item == null) throw new NotImplementedException();
         this[item.LanguagePart, item.LanguageElement] = item;
      }

      public void AddRange(IEnumerable<Whitespace2> newItems)
      {
         if (newItems == null) throw new NotImplementedException();
         foreach (var item in newItems)
         {
            this[item.LanguagePart, item.LanguageElement] = item;
         }
      }

      public Whitespace2 this[LanguageElement languageElement]
      {
         get
         { return this[LanguagePart.Current, languageElement]; }
         set
         { this[LanguagePart.Current, languageElement] = value; }
      }


      public Whitespace2 this[LanguagePart languagePart, LanguageElement languageElement]
      {
         get
         {
            return list
                    .Where(x => x.LanguageElement == languageElement
                      && x.LanguagePart == languagePart)
                    .FirstOrDefault();
         }
         set
         {
            var old = this[languagePart, languageElement];
            var pos = -1;
            if (old != null)
            { pos = list.IndexOf(old); }
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
      public Whitespace2(LanguageElement languageElement)
      {
         LanguagePart = LanguagePart.Current;
         LanguageElement = languageElement;
      }

      public Whitespace2(LanguagePart languagePart, LanguageElement languageElement)
      {
         LanguagePart = languagePart;
         LanguageElement = languageElement;
      }

      public Whitespace2(LanguageElement languageElement,
          string leadingWS, string trailingWS, string trailingComment)
         : this(LanguagePart.Current, languageElement, leadingWS, trailingWS, trailingComment)
      { }

      public Whitespace2(LanguagePart languagePart, LanguageElement languageElement,
            string leadingWS, string trailingWS, string trailingComment)
      {
         LanguagePart = languagePart;
         LanguageElement = languageElement;
         LeadingWhitespace = leadingWS;
         TrailingWhitespace = trailingWS;
         TrailingComment = trailingComment;
      }

      public Whitespace2(Whitespace2 old)
      {
         if (old == null) throw new NotImplementedException();
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
         return string.Format(CultureInfo.InvariantCulture,
             @"Whitespace: {0}/{1} Leading: '{2}' Trailing: '{3}' Comment: '{4}'",
             LanguagePart, LanguageElement, LeadingWhitespace, TrailingWhitespace,
             TrailingComment);
      }
   }
}
