namespace RoslynDomCommon
{
   internal interface IRetrieveValue
   {
   }

   internal interface IRetrieveValue<T>
   {
      object RetrieveValue(T item, string name);

      string RetrieveString(T item, string name);

      int RerieveInteger(T item, string name);
   }
}