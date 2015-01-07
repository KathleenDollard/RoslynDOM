namespace RoslynDom.Common
{
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
       Justification = "The non-generic version of this interface is useful in polymorphism (and I think this rule is stupid)")]
   public interface ISameIntent
   { }

   public interface ISameIntent<T> : ISameIntent
   {
      bool SameIntent(T one, T other);
   }
}