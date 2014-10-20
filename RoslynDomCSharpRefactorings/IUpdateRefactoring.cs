using RoslynDom.Common;

namespace RoslynDom.CSharp
{
public interface IUpdateRefactoring<T>
      where T : IDom
{
   bool MakeChange(T prop);
   bool NeedsChange(T prop);
}
}