using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RoslynDom.Common
{
    public class Worker
    {
        internal Worker(ICreateFromWorker createFromWorker, IBuildSyntaxWorker buildSyntaxWorker)
        {
            CreateFromWorker = createFromWorker;
            BuildSyntaxWorker = buildSyntaxWorker;
        }

        public ICreateFromWorker CreateFromWorker { get; private set; }
        public IBuildSyntaxWorker BuildSyntaxWorker { get; private set; }

        public RDomPriority Priority
        { get { return RDomPriority.Normal; } }

    }
}
