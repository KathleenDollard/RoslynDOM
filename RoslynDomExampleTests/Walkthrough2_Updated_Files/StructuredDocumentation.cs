using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructuredDocumentation : RDomBase<IStructuredDocumentation, ISymbol>, IStructuredDocumentation, IHasSameIntentMethod
,INotifyPropertyChanged.INotifyPropertyChanged    {
        public RDomStructuredDocumentation(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
        {     }

           public RDomStructuredDocumentation(RDomStructuredDocumentation oldRDom)
            :  base(oldRDom)
        { }
        public string Description { get{return_Description;}set{setProperty(ref _Description, value);}}
        public string Document { get{return_Document;}set{setProperty(ref _Document, value);}}
eventPropertyChangedEventHandler .PropertyChangedEventHandler PropertyChanged;    }
}
