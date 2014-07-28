using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;
using Microsoft.CodeAnalysis;

namespace RoslynDom.CSharp
{
    internal static class Mappings
    {
        private static List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>> LiteralKindMap = new List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>>()
        {
            Tuple.Create(SyntaxKind.StringLiteralToken,    SyntaxKind.StringLiteralExpression,    LiteralKind.String),
            Tuple.Create( SyntaxKind.NumericLiteralToken,   SyntaxKind.NumericLiteralExpression,   LiteralKind.Numeric),
            Tuple.Create( SyntaxKind.TrueKeyword,           SyntaxKind.TrueKeyword,                LiteralKind.Boolean),
            Tuple.Create( SyntaxKind.FalseKeyword,          SyntaxKind.FalseKeyword,               LiteralKind.Boolean),
        };

        public static LiteralKind LiteralKindFromSyntaxKind(SyntaxKind kind)
        {
            foreach (var tuple in LiteralKindMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item3; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxKindFromLiteralKind(LiteralKind literalKind, object value)
        {
            if (literalKind == LiteralKind.Boolean)
            {
                if ((bool)value) { return SyntaxKind.TrueLiteralExpression; }
                return SyntaxKind.FalseLiteralExpression;
            }
            foreach (var tuple in LiteralKindMap)
            {
                if (tuple.Item3 == literalKind)
                { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<SyntaxKind, Operator>> operatorMap = new List<Tuple<SyntaxKind, Operator>>()
        {
             Tuple.Create( SyntaxKind.SimpleAssignmentExpression, Operator.Equals ),
             Tuple.Create( SyntaxKind.AddAssignmentExpression, Operator.AddAssignment),
             Tuple.Create( SyntaxKind.SubtractAssignmentExpression, Operator.SubtractAssignment),
             Tuple.Create( SyntaxKind.MultiplyAssignmentExpression, Operator.MultiplyAssignment),
             Tuple.Create( SyntaxKind.DivideAssignmentExpression, Operator.DivideAssignment),
             Tuple.Create( SyntaxKind.ModuloAssignmentExpression, Operator.ModuloAssignment),
             Tuple.Create( SyntaxKind.AndAssignmentExpression, Operator.AndAssignment),
             Tuple.Create( SyntaxKind.ExclusiveOrAssignmentExpression, Operator.ExclusiveOrAssignment),
             Tuple.Create( SyntaxKind.OrAssignmentExpression, Operator.OrAssignment),
             Tuple.Create( SyntaxKind.LeftShiftAssignmentExpression, Operator.LeftShiftAssignment),
             Tuple.Create( SyntaxKind.RightShiftAssignmentExpression, Operator.RightShiftAssignment)
        };

        public static Operator GetOperatorFromCSharpKind(SyntaxKind kind)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind GetSyntaxKindFromOperator(Operator op)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item2 == op) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<Accessibility, AccessModifier>> accessModifierMap = new List<Tuple<Accessibility, AccessModifier>>()
        {
             Tuple.Create(Accessibility. Private, AccessModifier. Private),
             Tuple.Create(Accessibility. ProtectedAndInternal, AccessModifier. ProtectedAndInternal),
             Tuple.Create(Accessibility. ProtectedAndFriend , AccessModifier. ProtectedAndFriend ),
             Tuple.Create(Accessibility. Protected , AccessModifier. Protected ),
             Tuple.Create(Accessibility. Internal , AccessModifier. Internal ),
             Tuple.Create(Accessibility. Friend , AccessModifier. Friend ),
             Tuple.Create(Accessibility. NotApplicable, AccessModifier. NotApplicable),
             Tuple.Create(Accessibility. ProtectedOrInternal , AccessModifier. ProtectedOrInternal ),
             Tuple.Create(Accessibility. ProtectedOrFriend , AccessModifier. ProtectedOrFriend),
             Tuple.Create(Accessibility. Public , AccessModifier. Public )
        };
        public static Accessibility GetAccessibilityFromAccessModifier(AccessModifier accessModifier)
        {
            foreach (var tuple in accessModifierMap)
            {
                if (tuple.Item2 == accessModifier) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }

        public static AccessModifier GetAccessModifierFromAccessibility(Accessibility accessibility)
        {
            foreach (var tuple in accessModifierMap)
            {
                if (tuple.Item1 == accessibility) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

    }
}
