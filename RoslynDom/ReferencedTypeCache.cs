using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace RoslynDom
{
   internal class ReferencedTypeCache
   {
      private Compilation compilation;
      private IFactoryAccess  factoryAccess;
      private Dictionary<string, IType> metadataLookup;
      private Dictionary<SyntaxTree, IRoot> builtTrees; // not sure we'll use this

      internal ReferencedTypeCache(Compilation compilation,IFactoryAccess factoryAccess)
      {
         this.compilation = compilation;
         this.factoryAccess = factoryAccess;
         metadataLookup = new Dictionary<string, IType>();
         builtTrees = new Dictionary<SyntaxTree, IRoot>();
      }

      internal IType FindByMetadataName(string metadataName)
      {
         IType retType;
         if (metadataLookup.TryGetValue(metadataName, out retType))
         { return retType; }
         var symbol = compilation.GetTypeByMetadataName(metadataName);
         if (symbol == null) { return null; }
         var syntaxRef = symbol.DeclaringSyntaxReferences.First();
         var syntaxTree = syntaxRef.SyntaxTree;
         if (builtTrees.ContainsKey (syntaxTree)) { throw new InvalidOperationException(); }
         var root = factoryAccess.Load(syntaxTree);
         var types = root.Descendants
                        .OfType<IType>();
         foreach (var type in types)
         {
            metadataLookup.Add(type.MetadataName, type);
         }
         retType = metadataLookup[metadataName];  // should have it by here
         return retType;
      }
   }
}
