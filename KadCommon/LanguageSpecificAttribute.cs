using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace KadGen.Common
{

    public enum Language
    {
        CSharp = 0,
        VisualBasic,
        Xml
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class LanguageSpecificAttribute : Attribute
    {

        public static string GetValueForLanguage(Language language, object value, IFormatProvider formatProvider)
        {
            if (value == null) { return string.Empty;  }
            // UNTESTED: Need to add tests for the format provider being specified to the transform
            var cultureInfo = formatProvider as CultureInfo;
            var ret = value.ToString();
            if (language == Language.CSharp)
            { return (cultureInfo == null) ? ret.ToLower(CultureInfo.InvariantCulture ) : ret.ToLower(cultureInfo); }
            return ret;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LanguageSpecificValueAttribute : Attribute
    {
        public LanguageSpecificValueAttribute(Language language, object value)
        {
            this.Language = language;
            this.Value = value;
        }

        public Language Language { get; private set; }
        public object Value { get; private set; }

    }


}
