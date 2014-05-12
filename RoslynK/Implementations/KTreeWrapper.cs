using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RoslynK.Implementations
{
    public class KTreeWrapper : KBase<SyntaxTree>, ITreeWrapper
    {
        private KRoot _root;

        internal KTreeWrapper(SyntaxTree rawItem,
            KRoot root)
            : base(rawItem)
        {
            _root = root;
        }

        public override string Name
        {
            get { return "TreeWrapper"; }
        }

        public override string QualifiedName
        {
            get { return Name; }
        }

        public override string BestInContextName
        {
            get { return Name; }
        }

        public IRoot Root
        {
            get
            {
                return _root;
            }
        }
    }
}
