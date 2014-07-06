using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SameIntentFactory
    {
        // This is a totally butt ugly implementation, temporary because I don't want to introduce 
        // DI until I better understand overall needs
        public ISameIntent<T> SameIntent<T>()
        {
            if (typeof(T) == typeof(IRoot))
            { return (ISameIntent<T>)new SameIntentRoot(); }
            if (typeof(T) == typeof(INamespace))
            { return (ISameIntent<T>)new SameIntentNamespace(); }
        }
    }
}
