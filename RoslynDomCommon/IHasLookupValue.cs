namespace RoslynDom.Common
{
   public interface IHasLookupValue
   {
      object GetValue(string key);

      T GetValue<T>(string key);

      bool HasValue(string key);

      bool TryGetValue<T>(string key, out T value);
   }
}