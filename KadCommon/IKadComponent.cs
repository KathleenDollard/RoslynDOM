using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace KadGen.Common
{

    public interface IKadComponent
    {
        void BeginProcessing();
        void Process();
        void EndProcessing();
    }

    public interface IKadComponentMetadata
    {
        string KadTask { get; }
        string KadGroup { get; }
        int Priority { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class KadComponentAttribute : ExportAttribute, IKadComponentMetadata
    {
        public KadComponentAttribute(string kadTask, string kadGroup, int priority)
        {
            this.KadTask = kadTask;
            this.KadGroup = kadGroup;
            this.Priority = priority;
        }

        public string KadTask{get; private set;}
        public string KadGroup{get; private set;}
        public int Priority{get; private set;}
    }

}
