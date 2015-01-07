namespace RoslynDom.Common
{
   public enum AccessModifier
   {
      None = 0,
      Private,
      ProtectedAndInternal,
      ProtectedAndFriend = 2,
      Protected = 3,
      Internal = 4,
      Friend = 4,
      ProtectedOrInternal = 5,
      ProtectedOrFriend = 5,
      Public = 6
   }
}