namespace RoslynDom.Common
{
   public interface IHasAccessModifier : IDom
   {
      /// <summary>
      /// AccessModifier is the actual access modifier for the item. Because of different defaults
      /// in different languages, the actual modifier must be preserved for lanugage interchange
      /// </summary>
      /// <remarks>
      /// Watch for errors in the logic when each is set, for example, in constructors that set
      /// an access modifier, both modifiers must be set. To limit issues, the property setter
      /// for the DeclaredAccessModifier should set the AccessModifier, *** although the reverse
      /// is NOT true ***
      /// </remarks>
      AccessModifier AccessModifier { get; set; }

      /// <summary>
      /// DeclaredAccessModifier is the declared access modifier for the item. Because of different defaults
      /// in different languages, the declared and actual modifiers must be separately preserved for lanugage interchange
      /// </summary>
      /// <remarks>
      /// Watch for errors in the logic when each is set, for example, in constructors that set
      /// an access modifier, both modifiers must be set. To limit issues, the property setter
      /// for the DeclaredAccessModifier should set the AccessModifier, *** although the reverse
      /// is NOT true ***
      /// </remarks>
      AccessModifier DeclaredAccessModifier { get; set; }
   }
}
